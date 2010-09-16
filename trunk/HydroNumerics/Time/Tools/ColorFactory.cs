using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HydroNumerics.Time.Tools
{
    public class ColorFactory
    {
        Color[] colors;
        int currentColorIndex;

        public ColorFactory()
        {
            colors = new Color[] { Color.Blue, Color.DarkGreen, Color.DarkViolet, Color.Black, Color.Brown, Color.DarkOrange, Color.DarkOrchid, Color.DarkGoldenrod, Color.Crimson, Color.BurlyWood };
            
            currentColorIndex = colors.Length;
        }

        public ColorFactory(Color[] colors)
        {
            this.colors = colors;
            currentColorIndex = colors.Length;
        }
        /// <summary>
        /// returns a color that is different from the color that was returned the last time this metod was invoked.
        /// </summary>
        /// <returns></returns>
        public Color GetNextColor()
        {
            if (currentColorIndex >= colors.Length-1)
            {
                currentColorIndex = 0;
            }
            else
            {
                currentColorIndex++;
            }
            return colors[currentColorIndex];
        }
    }
}
