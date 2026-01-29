// Canvas 2D レンダラー - 高速バーチャート描画

window.canvasRenderer = {
    canvas: null,
    ctx: null,
    animationFrameId: null,
    
    // 色定義
    colors: {
        normal: '#3B82F6',      // 青
        compare: '#A855F7',     // 紫
        swap: '#EF4444',        // 赤
        write: '#F97316',       // 橙
        read: '#FBBF24'         // 黄
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
        
        console.log('Canvas initialized:', rect.width, 'x', rect.height, 'DPR:', dpr);
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
     * バーチャートを描画
     * @param {number[]} array - 描画する配列
     * @param {number[]} compareIndices - 比較中のインデックス
     * @param {number[]} swapIndices - スワップ中のインデックス
     * @param {number[]} readIndices - 読み取り中のインデックス
     * @param {number[]} writeIndices - 書き込み中のインデックス
     */
    render: function(array, compareIndices, swapIndices, readIndices, writeIndices) {
        if (!this.canvas || !this.ctx) {
            console.error('Canvas not initialized');
            return;
        }
        
        const rect = this.canvas.getBoundingClientRect();
        const width = rect.width;
        const height = rect.height;
        const arrayLength = array.length;
        
        // 背景をクリア（黒）
        this.ctx.fillStyle = '#1A1A1A';
        this.ctx.fillRect(0, 0, width, height);
        
        // 配列が空の場合は何もしない
        if (arrayLength === 0) return;
        
        // バーの幅と隙間を計算
        const minBarWidth = 1.0;
        let gapRatio;
        if (arrayLength <= 256) {
            gapRatio = 0.15;
        } else if (arrayLength <= 1024) {
            gapRatio = 0.10;
        } else {
            gapRatio = 0.05;
        }
        
        const requiredWidth = Math.max(width, arrayLength * minBarWidth / (1.0 - gapRatio));
        const totalBarWidth = requiredWidth / arrayLength;
        const barWidth = totalBarWidth * (1.0 - gapRatio);
        const gap = totalBarWidth * gapRatio;
        
        // 最大値を取得
        const maxValue = Math.max(...array);
        
        // Set を使って高速な存在チェック
        const compareSet = new Set(compareIndices);
        const swapSet = new Set(swapIndices);
        const readSet = new Set(readIndices);
        const writeSet = new Set(writeIndices);
        
        // スケール調整（横スクロール対応）
        const scale = Math.min(1.0, width / requiredWidth);
        this.ctx.save();
        if (scale < 1.0) {
            // 横スクロールが必要な場合は左寄せ
            this.ctx.scale(scale, 1.0);
        }
        
        // バーを描画（一括描画で高速化）
        for (let i = 0; i < arrayLength; i++) {
            const value = array[i];
            const barHeight = (value / maxValue) * (height - 20);
            const x = i * totalBarWidth + (gap / 2);
            const y = height - barHeight;
            
            // 色を決定（優先度順）
            let color;
            if (swapSet.has(i)) {
                color = this.colors.swap;
            } else if (compareSet.has(i)) {
                color = this.colors.compare;
            } else if (writeSet.has(i)) {
                color = this.colors.write;
            } else if (readSet.has(i)) {
                color = this.colors.read;
            } else {
                color = this.colors.normal;
            }
            
            this.ctx.fillStyle = color;
            this.ctx.fillRect(x, y, barWidth, barHeight);
        }
        
        this.ctx.restore();
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
    if (window.canvasRenderer.canvas) {
        window.canvasRenderer.resize();
    }
});
