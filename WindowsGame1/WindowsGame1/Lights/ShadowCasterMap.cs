using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shadows2D
{
    public enum PrecisionSettings
    {
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh
    }

    public class ShadowCasterMap
    {
        public RenderTarget2D Map;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public readonly PrecisionSettings PrecisionSettings;
        private float precisionRatio;
        private Vector2 pixelSizeHLSL;

        public ShadowCasterMap(PrecisionSettings precision, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            this.PrecisionSettings = precision;
            switch (this.PrecisionSettings)
            {
                case PrecisionSettings.VeryLow:
                    this.precisionRatio = 0.1f;
                    break;
                case PrecisionSettings.Low:
                    this.precisionRatio = 0.25f;
                    break;
                case PrecisionSettings.Normal:
                    this.precisionRatio = 0.5f;
                    break;
                case PrecisionSettings.High:
                    this.precisionRatio = 0.8f;
                    break;
                case PrecisionSettings.VeryHigh:
                    this.precisionRatio = 1f;
                    break;
            }
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.Map = new RenderTarget2D(graphics.GraphicsDevice, (int)(this.graphics.PreferredBackBufferWidth * this.precisionRatio), (int)(this.graphics.PreferredBackBufferHeight * this.precisionRatio));
            this.pixelSizeHLSL = new Vector2(1f / (float)this.Map.Width, 1f / (float)this.Map.Height);
        }

        public float PrecisionRatio
        {
            get { return this.precisionRatio; }
        }

        public Vector2 Size
        {
            get { return new Vector2(this.Map.Width, this.Map.Height); }
        }

        public Vector2 PixelSizeHLSL
        {
            get { return this.pixelSizeHLSL; }
        }

        public void StartGeneratingShadowCasteMap(bool blackInsteadOfWhiteBg)
        {
            this.graphics.GraphicsDevice.SetRenderTarget(this.Map);
            //
            if (blackInsteadOfWhiteBg)
                this.graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            else
                this.graphics.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
            //
            this.spriteBatch.Begin();
        }

        public void AddShadowCaster(Texture2D texture, Vector2 position, float width, float height)
        {
            width *= this.precisionRatio;
            height *= this.precisionRatio;

            position.X *= this.precisionRatio;
            position.Y *= this.precisionRatio;

            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);

            this.spriteBatch.Draw(texture, destination, Color.Black);
        }

        public void EndGeneratingShadowCasterMap()
        {
            this.spriteBatch.End();
            this.graphics.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
