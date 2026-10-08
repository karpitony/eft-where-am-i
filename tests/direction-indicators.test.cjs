const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const vm = require('node:vm');

// Run the exact script injected by WebView2, without a browser or NuGet test dependencies.
const constants = fs.readFileSync(path.join(__dirname, '../eft-where-am-i/Classes/Constants.cs'), 'utf8');
const script = constants.match(/ADD_DIRECTION_INDICATORS_SCRIPT\s*=\s*@"([\s\S]*?)";/)[1].replace(/""/g, '"');
const screenshot = (quaternion = '0, 0, 0, 1') => `2026-10-08[12-00]_10, 20, 30_${quaternion}_0`;

class Context
{
    constructor()
    {
        this.matrix = { a: 1, b: 0, c: 0, d: 1, e: 0, f: 0 };
        this.stack = [];
        this.visibleStrokes = [];
        this.strokeCount = 0;
    }

    setTransform(...args)
    {
        this.matrix = args.length === 1 ? { ...args[0] } : Object.fromEntries(['a', 'b', 'c', 'd', 'e', 'f'].map((key, i) => [key, args[i]]));
    }

    getTransform() { return { ...this.matrix }; }
    save() { this.stack.push({ ...this.matrix }); }
    restore() { this.matrix = this.stack.pop(); }
    clearRect() { this.visibleStrokes = []; }
    beginPath() { this.points = []; }
    closePath() {}
    moveTo(x, y) { this.points.push([x, y]); }
    lineTo(x, y) { this.points.push([x, y]); }
    arc(...args) { this.points.push({ arc: args }); }
    fill() {}

    stroke()
    {
        this.strokeCount++;
        this.visibleStrokes.push({ matrix: this.getTransform(), points: [...this.points] });
    }

    translate(x, y)
    {
        const m = this.matrix;
        m.e += m.a * x + m.c * y;
        m.f += m.b * x + m.d * y;
    }

    rotate(angle)
    {
        const m = { ...this.matrix }, c = Math.cos(angle), s = Math.sin(angle);
        Object.assign(this.matrix, { a: m.a * c + m.c * s, b: m.b * c + m.d * s, c: m.c * c - m.a * s, d: m.d * c - m.b * s });
    }
}

function environment(map = 'customs', rotation = 0)
{
    const canvases = [], markers = [], overlays = [], observers = [];
    class Element
    {
        constructor(tagName = 'div')
        {
            this.tagName = tagName;
            this.className = '';
            this.style = { setProperty(key, value, priority) { this[key] = value; this.priority = priority; } };
            this.children = [];
            this.classList = { contains: name => this.className.split(' ').includes(name) };
            this.width = 1600;
            this.height = 1200;
            this.clientWidth = 800;
            this.clientHeight = 600;
            this.offsetLeft = 0;
            this.offsetTop = 0;
            this.context = new Context();
        }

        getContext() { return this.context; }
        setAttribute() {}
        getAttribute() { return this.textContent; }
        appendChild(child) { this.children.push(child); }
        insertAdjacentElement(position, overlay) { overlays.push(overlay); }
        remove() {}
        matches(selector) { return selector === 'canvas.players-canvas' && this.className === 'players-canvas'; }
        querySelector(selector) { return this.children.find(child => selector === '.triangle-indicator' && child.className === 'triangle-indicator') || null; }
        querySelectorAll() { return []; }
    }
    const input = new Element('input');
    input.value = screenshot();
    input.handlers = new Map();
    input.addEventListener = (event, handler) => input.handlers.set(event, handler);
    input.removeEventListener = event => input.handlers.delete(event);
    const rotationButton = new Element('button');
    rotationButton.textContent = `지도 회전: ${rotation}°`;
    const document = {
        body: new Element(),
        head: new Element(),
        createElement: tagName => new Element(tagName),
        getElementById: () => null,
        querySelector: selector => selector.startsWith('#map-position-file') ? input : selector === '.panel_top .toolbar-rotation' ? rotationButton : null,
        querySelectorAll: selector => selector === '.marker' ? markers : selector === 'canvas.players-canvas' ? canvases : []
    };
    const sandbox = vm.createContext({
        document, window: {}, location: { pathname: `/maps/${map}` }, HTMLElement: Element,
        getComputedStyle: () => ({ zIndex: '7' }), setTimeout() {},
        MutationObserver: class { constructor(callback) { this.callback = callback; observers.push(this); } observe() {} disconnect() {} }
    });
    function addCanvas()
    {
        const canvas = new Element('canvas');
        canvas.className = 'players-canvas';
        canvases.push(canvas);
        return canvas;
    }
    return { sandbox, canvases, markers, overlays, observers, input, rotationButton, addCanvas, Element, inject: () => vm.runInContext(script, sandbox) };
}

// Representative native players-canvas frame: DPR transform, own dot, then optional squad dots.
function renderDots(canvas, positions = [[150, 250]], dpr = 2)
{
    const context = canvas.context;
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.setTransform(dpr, 0, 0, dpr, 0, 0);
    for (const [x, y] of positions)
    {
        context.save();
        context.translate(x, y);
        context.beginPath();
        context.arc(0, 0, 9, 0, Math.PI * 2);
        context.fill();
        context.stroke();
        context.restore();
    }
}

function heading(overlay)
{
    const { a, b } = overlay.context.visibleStrokes.at(-1).matrix;
    return (Math.atan2(b, a) * 180 / Math.PI + 360) % 360;
}

function approximately(actual, expected)
{
    assert.ok(Math.abs(actual - expected) < 0.001, `${actual} should equal ${expected}`);
}

test('Canvas marker shows heading at the native dot and follows pan, resize and DPR', () => {
    const env = environment();
    const canvas = env.addCanvas();
    env.inject();
    renderDots(canvas);
    const overlay = env.overlays[0];
    assert.equal(canvas.context.strokeCount, 1);
    assert.equal(overlay.context.visibleStrokes.length, 1);
    approximately(heading(overlay), 180);
    assert.equal(overlay.context.visibleStrokes[0].matrix.e, 300);
    assert.equal(overlay.context.visibleStrokes[0].matrix.f, 500);
    canvas.width = 900;
    canvas.clientWidth = 900;
    renderDots(canvas, [[400, 300]], 1);
    assert.equal(overlay.width, 900);
    assert.equal(overlay.style.width, '900px');
    assert.equal(overlay.context.visibleStrokes[0].matrix.e, 400);
    assert.equal(overlay.context.visibleStrokes[0].matrix.f, 300);
});

test('heading changes at the same position without requiring a native redraw', () => {
    const env = environment();
    const canvas = env.addCanvas();
    env.inject();
    renderDots(canvas);
    env.input.value = screenshot('0, 0.70710678, 0, 0.70710678');
    env.input.handlers.get('input')();
    approximately(heading(env.overlays[0]), 270);
    assert.equal(canvas.context.strokeCount, 1);
    env.input.value = 'invalid filename';
    env.input.handlers.get('change')();
    assert.equal(env.overlays[0].context.visibleStrokes.length, 0);
});

test('map transform and user view rotation are applied exactly once', () => {
    for (const [map, expected] of [['factory', 270], ['lab', 90], ['reserve', 165], ['customs', 180]])
    {
        const env = environment(map, 90);
        const canvas = env.addCanvas();
        env.inject();
        renderDots(canvas);
        approximately(heading(env.overlays[0]), (expected + 90) % 360);
        env.rotationButton.textContent = 'Rotate map: 180°';
        renderDots(canvas);
        approximately(heading(env.overlays[0]), (expected + 180) % 360);
    }
});

test('re-injection is idempotent and squad dots do not receive the own heading', () => {
    const env = environment();
    const canvas = env.addCanvas();
    env.inject();
    const wrappedStroke = canvas.context.stroke;
    env.inject();
    assert.equal(env.overlays.length, 1);
    assert.equal(canvas.context.stroke, wrappedStroke);
    renderDots(canvas, [[150, 250], [500, 400]]);
    assert.equal(canvas.context.strokeCount, 2);
    assert.equal(env.overlays[0].context.visibleStrokes.length, 1);
    assert.equal(env.overlays[0].context.visibleStrokes[0].matrix.e, 300);
    canvas.context.clearRect(0, 0, canvas.width, canvas.height);
    assert.equal(env.overlays[0].context.visibleStrokes.length, 0);
});

test('native directional marker is kept without adding an arrow to a later squad dot', () => {
    const env = environment();
    const canvas = env.addCanvas();
    env.inject();
    const context = canvas.context;
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.beginPath();
    context.moveTo(50, 3);
    context.lineTo(40, 30);
    context.lineTo(60, 30);
    context.stroke();
    context.beginPath();
    context.arc(50, 50, 16, 0, Math.PI * 2);
    context.stroke();
    context.beginPath();
    context.arc(0, 0, 9, 0, Math.PI * 2);
    context.stroke();
    assert.equal(context.strokeCount, 3);
    assert.equal(env.overlays[0].context.visibleStrokes.length, 0);
});

test('SPA canvas replacement binds the new renderer', () => {
    const env = environment();
    env.inject();
    const canvas = env.addCanvas();
    env.observers.at(-1).callback([{ type: 'childList', addedNodes: [canvas] }]);
    renderDots(canvas);
    approximately(heading(env.overlays[0]), 180);
});

test('legacy DOM markers retain their heading and inherit view rotation from the parent', () => {
    const env = environment('factory', 90);
    const marker = new env.Element();
    marker.className = 'marker';
    env.markers.push(marker);
    env.inject();
    assert.equal(marker.children.length, 1);
    assert.equal(marker.children[0].style.transform, 'translate(-50%, -100%) rotate(270deg)');
    assert.equal(marker.children[0].style.priority, 'important');
});
