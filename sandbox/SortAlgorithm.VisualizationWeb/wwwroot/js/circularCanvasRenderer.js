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
     * @param {Object} bufferArrays - バッファー配列（BufferId -> 配列）
     * @param {boolean} showCompletionHighlight - 完了ハイライトを表示するか
     */
    render: function(array, compareIndices, swapIndices, readIndices, writeIndices, isSortCompleted, bufferArrays, showCompletionHighlight) {
        if (!this.canvas || !this.ctx) {
            console.error('Canvas not initialized');
            return;
        }
        
        // デフォルト値を設定
        isSortCompleted = isSortCompleted || false;
        bufferArrays = bufferArrays || {};
        showCompletionHighlight = showCompletionHighlight !== undefined ? showCompletionHighlight : false;
        
        const rect = this.canvas.getBoundingClientRect();
        const width = rect.width;
        const height = rect.height;
        const arrayLength = array.length;
        
        // バッファー配列の数を取得
        const bufferCount = Object.keys(bufferArrays).length;
        
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
        
        // バッファー配列が表示されている場合のみ同心円リングとして配置
        const showBuffers = bufferCount > 0 && !isSortCompleted;
        let mainMinRadius, mainMaxRadius;
        let ringWidth = 0;
        
        if (showBuffers) {
            // バッファーがある場合: リング分割
            const totalRings = 1 + bufferCount; // メイン + バッファー
            ringWidth = (maxRadius * 0.8) / totalRings; // 各リングの幅
            mainMinRadius = minRadius;
            mainMaxRadius = minRadius + ringWidth;
        } else {
            // バッファーがない場合: メイン配列が外側まで広がる
            mainMinRadius = minRadius;
            mainMaxRadius = maxRadius; // 最大半径まで使用
        }
        
        // 最大値を取得
        const maxValue = Math.max(...array);
        
        // Set を使って高速な存在チェック
        const compareSet = new Set(compareIndices);
        const swapSet = new Set(swapIndices);
        const readSet = new Set(readIndices);
        const writeSet = new Set(writeIndices);
        
        // 各要素を円周上に配置
        const angleStep = (2 * Math.PI) / arrayLength;
        
        // メイン配列の線を描画
        for (let i = 0; i < arrayLength; i++) {
            const value = array[i];
            const angle = i * angleStep - Math.PI / 2; // -90度から開始（12時の位置）
            
            // 値に応じた半径（メイン配列のリング内）
            const radius = mainMinRadius + (value / maxValue) * (mainMaxRadius - mainMinRadius);
            
            // 終点の座標
            const endX = centerX + Math.cos(angle) * radius;
            const endY = centerY + Math.sin(angle) * radius;
            
            // 開始点の座標（メイン配列リングの内側）
            const startX = centerX + Math.cos(angle) * mainMinRadius;
            const startY = centerY + Math.sin(angle) * mainMinRadius;
            
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
        
        // バッファー配列を同心円リングとして描画（ソート完了時は非表示）
        if (showBuffers) {
            const bufferIds = Object.keys(bufferArrays).sort((a, b) => parseInt(a) - parseInt(b));
            
            for (let bufferIndex = 0; bufferIndex < bufferIds.length; bufferIndex++) {
                const bufferId = bufferIds[bufferIndex];
                const bufferArray = bufferArrays[bufferId];
                
                if (!bufferArray || bufferArray.length === 0) continue;
                
                // このバッファー配列のリング範囲
                const ringIndex = bufferIndex + 1; // メイン配列の外側
                const bufferMinRadius = minRadius + ringIndex * ringWidth;
                const bufferMaxRadius = bufferMinRadius + ringWidth;
                
                // バッファー配列の最大値
                const bufferMaxValue = Math.max(...bufferArray);
                const bufferLength = bufferArray.length;
                const bufferAngleStep = (2 * Math.PI) / bufferLength;
                
                // バッファー配列の線を描画
                for (let i = 0; i < bufferLength; i++) {
                    const value = bufferArray[i];
                    const angle = i * bufferAngleStep - Math.PI / 2;
                    
                    // 値に応じた半径（バッファーリング内）
                    const radius = bufferMinRadius + (value / bufferMaxValue) * (bufferMaxRadius - bufferMinRadius);
                    
                    // 終点の座標
                    const endX = centerX + Math.cos(angle) * radius;
                    const endY = centerY + Math.sin(angle) * radius;
                    
                    // 開始点の座標（バッファーリングの内側）
                    const startX = centerX + Math.cos(angle) * bufferMinRadius;
                    const startY = centerY + Math.sin(angle) * bufferMinRadius;
                    
                    // バッファー配列は薄いシアン色で表示
                    const bufferColor = '#06B6D4';
                    
                    // 線の太さを配列サイズに応じて調整
                    let lineWidth;
                    if (bufferLength <= 64) {
                        lineWidth = 3;
                    } else if (bufferLength <= 256) {
                        lineWidth = 2;
                    } else if (bufferLength <= 1024) {
                        lineWidth = 1.5;
                    } else {
                        lineWidth = 1;
                    }
                    
                    // 線を描画
                    this.ctx.strokeStyle = bufferColor;
                    this.ctx.lineWidth = lineWidth;
                    this.ctx.beginPath();
                    this.ctx.moveTo(startX, startY);
                    this.ctx.lineTo(endX, endY);
                    this.ctx.stroke();
                }
                
                // バッファーIDラベルを表示（円の外側）
                const labelAngle = -Math.PI / 2; // 12時の位置
                const labelRadius = bufferMaxRadius + 15;
                const labelX = centerX + Math.cos(labelAngle) * labelRadius;
                const labelY = centerY + Math.sin(labelAngle) * labelRadius;
                
                this.ctx.fillStyle = '#888';
                this.ctx.font = '12px monospace';
                this.ctx.textAlign = 'center';
                this.ctx.fillText(`Buf#${bufferId}`, labelX, labelY);
            }
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
