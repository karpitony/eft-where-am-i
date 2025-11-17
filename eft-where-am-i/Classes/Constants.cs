namespace eft_where_am_i.Classes
{
    public class Constants
    {   
        public const string HIDE_SHOW_PANNE_BUTTON_SELECTOR = "#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > div.mr-15 > button";

        public const string FULL_SCREEN_BUTTON_SELECTOR = "#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > button";

        public const string WHERE_AM_I_BUTTON_SELECTOR = "#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > div.d-flex.ml-15.fs-0 > button";

        /*
         * The following code (ADD_DIRECTION_INDICATORS_SCRIPT) is from 'Tarkov-Client' by 'byeong1'
         * and is licensed under the MIT License.
         * GitHub: https://github.com/byeong1/Tarkov-Client
         *
         * MIT License
         *
         * Copyright (c) 2025 byeong1
         *
         * Permission is hereby granted, free of charge, to any person obtaining a copy
         * of this software and associated documentation files (the "Software"), to deal
         * in the Software without restriction, including without limitation the rights
         * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
         * copies of the Software, and to permit persons to whom the Software is
         * furnished to do so, subject to the following conditions:
         *
         * The above copyright notice and this permission notice shall be included in all
         * copies or substantial portions of the Software.
         *
         * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
         * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
         * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
         * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
         * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
         * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
         * SOFTWARE.
         */
        /// <summary>
        /// 방향 표시기를 추가하는 스크립트
        /// </summary>
        public const string ADD_DIRECTION_INDICATORS_SCRIPT =
            @"     
(function () {
    'use strict';

    // 사용자가 제공한 SVG 데이터 URL 직접 사용
    const svgDataUrl = 'data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4IDEwIj48ZyB0cmFuc2Zvcm09InRyYW5zbGF0ZSg0LCA0KSBzY2FsZSgwLjcpIHRyYW5zbGF0ZSgtNCwgLTQpIj48cG9seWdvbiBwb2ludHM9IjQsMCA3LjUsOCAwLjUsOCIgZmlsbD0iIzhhMmJlMiIgc3Ryb2tlPSIjNzBhODAwIiBzdHJva2Utd2lkdGg9IjAuNSIvPjwvZz48L3N2Zz4=';

    function injectStyle() {
        const style = document.createElement('style');
        style.id = 'triangle-indicator-style';
        style.textContent = `
        .triangle-indicator {
            position: absolute !important;
            top: 0% !important;
            left: 50% !important;
            width: 25px !important;
            height: 60px !important;
            background-image: url('${svgDataUrl}') !important;
            background-repeat: no-repeat !important;
            background-size: 100% 100% !important;
            pointer-events: none !important;
            z-index: 9999 !important;
            transform: translate(-50%, -65%) !important;
            transform-origin: 50% 100% !important;
            transition: transform 0.1s ease !important;
        }`;

        const existingStyle = document.getElementById('triangle-indicator-style');
        if (existingStyle) existingStyle.remove();
        document.head.appendChild(style);
    }

    function addTriangleToMarker(marker) {
        if (marker.querySelector('.triangle-indicator')) {
            return;
        }

        const triangle = document.createElement('div');
        triangle.className = 'triangle-indicator';

        // 마커가 relative position을 가지도록 설정
        marker.style.position = 'relative';

        const inputEl = marker.querySelector('input[type=text]');
        if(inputEl){
            const updateArrow = () => {
                const val = inputEl.value.trim();
                if(!val) return;

                // 로그 형태: posX,posY,posZ_quatX,quatY,quatZ,quatW_speed
                const parts = val.split('_');
                if(parts.length < 3) return;
                const quat = parts[2].split(',').map(Number);
                if(quat.length < 4) return;

                // 쿼터니언 → forward → deg
                const x=quat[0], y=quat[1], z=quat[2], w=quat[3];
                const fx = 2*(x*z + w*y);
                const fz = 1 - 2*(x*x + y*y);
                const deg = Math.atan2(fx, fz) * 180 / Math.PI;

                triangle.style.transform = `translate(-50%, -65%) rotate(${deg}deg)`;
            };

            inputEl.addEventListener('input', updateArrow);
            updateArrow(); // 초기값 적용
        }

        marker.appendChild(triangle);
    }

    function initMarkers() {
        const markers = document.querySelectorAll('.marker');
        if (markers.length === 0) {
            // .marker가 없으면 다른 선택자 시도
            const altMarkers = document.querySelectorAll('#map > div');
            altMarkers.forEach(addTriangleToMarker);
        } else {
            markers.forEach(addTriangleToMarker);
        }
    }

    injectStyle();

    const container = document.querySelector('#map') || document.querySelector('#map-layer') || document.body;
    const observer = new MutationObserver(mutations => {
        for (const mutation of mutations) {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(node => {
                    if (!(node instanceof HTMLElement)) return;

                    if (node.classList && node.classList.contains('marker')) {
                        addTriangleToMarker(node);
                    } else {
                        node.querySelectorAll('.marker, #map > div').forEach(addTriangleToMarker);
                    }
                });
            }
        }
    });

    observer.observe(container, {
        childList: true,
        subtree: true,
    });

    // 2초 후에 마커 초기화 시도
    setTimeout(initMarkers, 2000);
})();";
    }
}
