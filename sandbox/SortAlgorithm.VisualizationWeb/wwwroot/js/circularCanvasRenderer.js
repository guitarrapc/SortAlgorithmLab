// Canvas 2D 円形レンダラー - 高速円形ビジュアライゼーション

window.circularCanvasRenderer = {
    canvas: null,
    ctx: null,
    animationFrameId: null,
    
    // 色定義（操作に基づく）
    colors: {
        normal: null,           // HSLで動的に計算
        compare: '#A855F7',     // 紫
        swap: '#EF4444',        // 赤
        write: '#F97316',       // 橙
        read: '#FBBF24',        // 黄
        sorted: '#10B981'       // 緑 - ソート完了
    },
    
    /**
     * Canvasを初期化
     * @param {string} canvasId - Canvas要素のID
     */
    initialize: function(canvasId) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) {
            console.error('Canvas element not found:', canvasId);
            return false;
        }
        
        this.ctx = this.canvas.getContext('2d', {
            alpha: false,           // 透明度不要（高速化）
            desynchronized: true    // 非同期描画（高速化）
        });
        
        // 高DPI対応
        const dpr = window.devicePixelRatio || 1;
        const rect = this.canvas.getBoundingClientRect();
        this.canvas.width = rect.width * dpr;
        this.canvas.height = rect.height * dpr;
        this.ctx.scale(dpr, dpr);
        
        console.log('Circular Canvas initialized:', rect.width, 'x', rect.height, 'DPR:', dpr);
        return true;
    },
    
    /**
     * リサイズ処理
     */
    resize: function() {
        if (!this.canvas) return;
        
        const dpr = window.devicePixelRatio || 1;
        const rect = this.canvas.getBoundingClientRect();
        this.canvas.width = rect.width * dpr;
        this.canvas.height = rect.height * dpr;
        this.ctx.scale(dpr, dpr);
    },
    
    /**
     * 値をHSL色に変換（0-maxValue -> 0-360度）
     * @param {number} value - 要素の値
     * @param {number} maxValue - 配列の最大値
     * @returns {string} HSL色文字列
     */
    valueToHSL: function(value, maxValue) {
        const hue = (value / maxValue) * 360;
        return `hsl(${hue}, 70%, 60%)`;
    },
    
    /**
     * 円形ビジュアライゼーションを描画
     * @param {number[]} array - 描画する配列
     * @param {number[]} compareIndices - 比較中のインデックス
     * @param {number[]} swapIndices - スワップ中のインデックス
     * @param {number[]} readIndices - 読み取り中のインデックス
     * @param {number[]} writeIndices - 書き込み中のインデックス
     * @param {boolean} isSortCompleted - ソートが完了したかどうか
     */
    render: function(array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted) {
        if (!this.canvas || !this.ctx) {
            console.error('Canvas not initialized');
            return;
        }
        
        // デフォルト値を設定
        isSortCompleted = isSortCompleted || false;
        
        const rect = this.canvas.getBoundingClientRect();
        const width = rect.width;
        const height = rect.height;
        const arrayLength = array.length;
        
        // 背景をクリア（黒）
        this.ctx.fillStyle = '#1A1A1A';
        this.ctx.fillRect(0, 0, width, height);
        
        // 配列が空の場合は何もしない
        if (arrayLength === 0) return;
        
        // 円の中心と半径を計算
        const centerX = width / 2;
        const centerY = height / 2;
        const maxRadius = Math.min(width, height) * 0.45; // 90%の直径を使用（余白考慮）
        const minRadius = maxRadius * 0.2; // 内側の空白（ドーナツ型）
        
        // 最大値を取得
        const maxValue = Math.max(...array);
        
        // Set を使って高速な存在チェック
        const compareSet = new Set(compareIndices);
        const swapSet = new Set(swapIndices);
        const readSet = new Set(readIndices);
        const writeSet = new Set(writeIndices);
        
        // 各要素を円周上に配置
        const angleStep = (2 * Math.PI) / arrayLength;
        
        // 線を描画（中心から外側へ）
        for (let i = 0; i < arrayLength; i++) {
            const value = array[i];
            const angle = i * angleStep - Math.PI / 2; // -90度から開始（12時の位置）
            
            // 値に応じた半径（0 = minRadius, maxValue = maxRadius）
            const radius = minRadius + (value / maxValue) * (maxRadius - minRadius);
            
            // 終点の座標
            const endX = centerX + Math.cos(angle) * radius;
            const endY = centerY + Math.sin(angle) * radius;
            
            // 開始点の座標（内側の円周上）
            const startX = centerX + Math.cos(angle) * minRadius;
            const startY = centerY + Math.sin(angle) * minRadius;
            
            // 色を決定（優先度順）
            let color;
            if (isSortCompleted) {
                // ソート完了時はすべて緑色
                color = this.colors.sorted;
            } else if (swapSet.has(i)) {
                color = this.colors.swap;
            } else if (compareSet.has(i)) {
                color = this.colors.compare;
            } else if (writeSet.has(i)) {
                color = this.colors.write;
            } else if (readSet.has(i)) {
                color = this.colors.read;
            } else {
                // 通常時は値に基づくHSLグラデーション
                color = this.valueToHSL(value, maxValue);
            }
            
            // 線の太さを配列サイズに応じて調整
            let lineWidth;
            if (arrayLength <= 64) {
                lineWidth = 3;
            } else if (arrayLength <= 256) {
                lineWidth = 2;
            } else if (arrayLength <= 1024) {
                lineWidth = 1.5;
            } else {
                lineWidth = 1;
            }
            
            // 線を描画
            this.ctx.strokeStyle = color;
            this.ctx.lineWidth = lineWidth;
            this.ctx.beginPath();
            this.ctx.moveTo(startX, startY);
            this.ctx.lineTo(endX, endY);
            this.ctx.stroke();
        }
        
        // 中心円を描画（オプション、視覚的なアクセント）
        this.ctx.fillStyle = '#2A2A2A';
        this.ctx.beginPath();
        this.ctx.arc(centerX, centerY, minRadius, 0, 2 * Math.PI);
        this.ctx.fill();
        
        // 描画完了
        this.isRendering = false;
        this.pendingRenderData = null;
    },
    
    /**
     * クリーンアップ
     */
    dispose: function() {
        if (this.animationFrameId) {
            cancelAnimationFrame(this.animationFrameId);
            this.animationFrameId = null;
        }
        this.canvas = null;
        this.ctx = null;
    }
};

// ウィンドウリサイズ時の処理
window.addEventListener('resize', () => {
    if (window.circularCanvasRenderer.canvas) {
        window.circularCanvasRenderer.resize();
    }
});
