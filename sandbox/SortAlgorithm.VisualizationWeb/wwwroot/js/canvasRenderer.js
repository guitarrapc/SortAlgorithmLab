// Canvas 2D レンダラー - 高速バーチャート描画（複数Canvas対応）

window.canvasRenderer = {
instances: new Map(), // Canvas ID -> インスタンスのマップ
    
// デバッグ用：FPS計測
renderCounts: new Map(),
lastFpsLogs: new Map(),
    
// 色定義
colors: {
    normal: '#3B82F6',      // 青
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
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error('Canvas element not found:', canvasId);
            return false;
        }
        
        const ctx = canvas.getContext('2d', {
            alpha: false,           // 透明度不要（高速化）
            desynchronized: true    // 非同期描画（高速化）
        });
        
        // 高DPI対応
        const dpr = window.devicePixelRatio || 1;
        const rect = canvas.getBoundingClientRect();
        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;
        ctx.scale(dpr, dpr);
        
        // インスタンスを保存
        this.instances.set(canvasId, { canvas, ctx });
        
        console.log('Canvas initialized:', canvasId, rect.width, 'x', rect.height, 'DPR:', dpr);
        return true;
    },
    
    /**
     * リサイズ処理
     */
    resize: function() {
        // すべてのCanvasをリサイズ
        this.instances.forEach((instance, canvasId) => {
            const { canvas, ctx } = instance;
            if (!canvas) return;
            
            const dpr = window.devicePixelRatio || 1;
            const rect = canvas.getBoundingClientRect();
            canvas.width = rect.width * dpr;
            canvas.height = rect.height * dpr;
            ctx.scale(dpr, dpr);
            
            console.log('Canvas resized:', canvasId, rect.width, 'x', rect.height);
        });
    },
    
    /**
     * バーチャートを描画
     * @param {string} canvasId - Canvas要素のID
     * @param {number[]} array - 描画する配列
     * @param {number[]} compareIndices - 比較中のインデックス
     * @param {number[]} swapIndices - スワップ中のインデックス
     * @param {number[]} readIndices - 読み取り中のインデックス
     * @param {number[]} writeIndices - 書き込み中のインデックス
     * @param {boolean} isSortCompleted - ソートが完了したかどうか
     * @param {Object} bufferArrays - バッファー配列（BufferId -> 配列）
     * @param {boolean} showCompletionHighlight - 完了ハイライトを表示するか
     */
    render: function(canvasId, array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight) {
        const instance = this.instances.get(canvasId);
        if (!instance) {
            console.error('Canvas instance not found:', canvasId);
            return;
        }
        
        const { canvas, ctx } = instance;
        if (!canvas || !ctx) {
            console.error('Canvas not initialized:', canvasId);
            return;
        }
        
        // 🔍 デバッグ：render() 呼び出し回数をカウント
        if (!this.renderCounts.has(canvasId)) {
            this.renderCounts.set(canvasId, 0);
            this.lastFpsLogs.set(canvasId, Date.now());
        }
        this.renderCounts.set(canvasId, this.renderCounts.get(canvasId) + 1);
        
        const now = Date.now();
        const lastLog = this.lastFpsLogs.get(canvasId);
        const elapsed = (now - lastLog) / 1000;
        
        if (elapsed >= 1.0) {
            const fps = this.renderCounts.get(canvasId) / elapsed;
            console.log(`[JS Canvas] ${canvasId.substring(0, 12)}... JS render() FPS: ${fps.toFixed(1)}`);
            this.renderCounts.set(canvasId, 0);
            this.lastFpsLogs.set(canvasId, now);
        }
        
        // デフォルト値を設定
        isSortCompleted = isSortCompleted || false;
        bufferArrays = bufferArrays || {};
        showCompletionHighlight = showCompletionHighlight !== undefined ? showCompletionHighlight : false;
        
        const rect = canvas.getBoundingClientRect();
        const width = rect.width;
        const height = rect.height;
        const arrayLength = array.length;
        
        // バッファー配列の数を取得
        const bufferCount = Object.keys(bufferArrays).length;
        
        // 背景をクリア（黒）
        ctx.fillStyle = '#1A1A1A';
        ctx.fillRect(0, 0, width, height);
        
        // 配列が空の場合は何もしない
        if (arrayLength === 0) return;
        
        // バッファー配列が表示されている場合のみ画面を分割
        const showBuffers = bufferCount > 0 && !isSortCompleted;
        const totalSections = showBuffers ? (1 + bufferCount) : 1;
        const sectionHeight = height / totalSections;
        const mainArrayY = showBuffers ? (sectionHeight * bufferCount) : 0; // バッファー表示時は下部、非表示時は画面全体
        
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
        ctx.save();
        if (scale < 1.0) {
            // 横スクロールが必要な場合は左寄せ
            ctx.scale(scale, 1.0);
        }
        
        // メイン配列のバーを描画（一括描画で高速化）
        for (let i = 0; i < arrayLength; i++) {
            const value = array[i];
            const barHeight = (value / maxValue) * (sectionHeight - 20);
            const x = i * totalBarWidth + (gap / 2);
            const y = mainArrayY + (sectionHeight - barHeight);
            
            // 色を決定（優先度順）
            let color;
            if (showCompletionHighlight) {
                // ソート完了ハイライト表示中はすべて緑色
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
                color = this.colors.normal;
            }
            
            ctx.fillStyle = color;
            ctx.fillRect(x, y, barWidth, barHeight);
        }
        
        ctx.restore();
        
        // バッファー配列を描画（ソート完了時は非表示）
        if (showBuffers) {
            const bufferIds = Object.keys(bufferArrays).sort((a, b) => parseInt(a) - parseInt(b));
            
            for (let bufferIndex = 0; bufferIndex < bufferIds.length; bufferIndex++) {
                const bufferId = bufferIds[bufferIndex];
                const bufferArray = bufferArrays[bufferId];
                const bufferY = bufferIndex * sectionHeight;
                
                if (!bufferArray || bufferArray.length === 0) continue;
                
                // バッファー配列の最大値
                const bufferMaxValue = Math.max(...bufferArray);
                const bufferLength = bufferArray.length;
                
                // バッファー配列用のバー幅計算（メイン配列と同じロジック）
                const bufferRequiredWidth = Math.max(width, bufferLength * minBarWidth / (1.0 - gapRatio));
                const bufferTotalBarWidth = bufferRequiredWidth / bufferLength;
                const bufferBarWidth = bufferTotalBarWidth * (1.0 - gapRatio);
                const bufferGap = bufferTotalBarWidth * gapRatio;
                
                // バッファー配列のスケール
                const bufferScale = Math.min(1.0, width / bufferRequiredWidth);
                ctx.save();
                if (bufferScale < 1.0) {
                    ctx.scale(bufferScale, 1.0);
                }
                
                // バッファー配列のバーを描画
                for (let i = 0; i < bufferLength; i++) {
                    const value = bufferArray[i];
                    const barHeight = (value / bufferMaxValue) * (sectionHeight - 20);
                    const x = i * bufferTotalBarWidth + (bufferGap / 2);
                    const y = bufferY + (sectionHeight - barHeight);
                    
                    // バッファー配列は薄いシアン色で表示
                    ctx.fillStyle = '#06B6D4';
                    ctx.fillRect(x, y, bufferBarWidth, barHeight);
                }
                
                ctx.restore();
                
                // バッファーIDラベルを表示
                ctx.fillStyle = '#888';
                ctx.font = '12px monospace';
                ctx.fillText(`Buffer #${bufferId}`, 10, bufferY + 20);
            }
        }
        
        // メイン配列ラベルを表示（バッファーが表示されている場合のみ）
        if (showBuffers) {
            ctx.fillStyle = '#888';
            ctx.font = '12px monospace';
            ctx.fillText('Main Array', 10, mainArrayY + 20);
        }
    },
    
    /**
     * クリーンアップ
     */
    dispose: function() {
        // すべてのインスタンスをクリア
        this.instances.clear();
    }
};

// ウィンドウリサイズ時の処理
window.addEventListener('resize', () => {
    if (window.canvasRenderer.canvas) {
        window.canvasRenderer.resize();
    }
});
