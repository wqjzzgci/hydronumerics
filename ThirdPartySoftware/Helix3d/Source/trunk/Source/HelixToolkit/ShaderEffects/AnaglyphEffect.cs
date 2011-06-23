using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace HelixToolkit
{
    public enum AnaglyphMethod
    {
        True = 0,
        Gray = 1,
        Color = 2,
        HalfColor = 3,
        Optimized = 4,
        Dubois = 5
    }

    /// <summary>
    /// Anaglyph blending effect
    /// 
    /// Usage: 
    /// 1. Add the effect to the LEFT EYE UIElement. 
    /// 2. Set RightInput to a VisualBrush of the RIGHT EYE UIElement.
    /// 
    /// See AnalgyphView3D for an example on the usage.
    /// </summary>
    public class AnaglyphEffect : ShaderEffect
    {
        private const string EffectFile = "ShaderEffects/AnaglyphEffect.ps";

        static AnaglyphEffect()
        {
            Assembly a = typeof (AnaglyphEffect).Assembly;
            string assemblyShortName = a.ToString().Split(',')[0];
            string uriString = "pack://application:,,,/" + assemblyShortName + ";component/" + EffectFile;
            Shader.UriSource = new Uri(uriString);
        }

        public AnaglyphEffect()
        {
            PixelShader = Shader;

            // Update each DependencyProperty that's registered with a shader register.  This
            // is needed to ensure the shader gets sent the proper default value.

            UpdateShaderValue(MethodProperty);
            UpdateShaderValue(LeftInputProperty);
            UpdateShaderValue(RightInputProperty);
        }

        public static readonly DependencyProperty MethodProperty =
            DependencyProperty.Register("Method", typeof (AnaglyphMethod), typeof (AnaglyphEffect),
                                        new UIPropertyMetadata(AnaglyphMethod.Gray, AnaglyphMethodChanged));

        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty LeftInputProperty =
            RegisterPixelShaderSamplerProperty("LeftInput", typeof (AnaglyphEffect), 0);

        // Brush-valued properties turn into sampler-property in the shader.
        // This helper sets "ImplicitInput" as the default, meaning the default
        // sampler is whatever the rendering of the element it's being applied to is.
        public static readonly DependencyProperty RightInputProperty =
            RegisterPixelShaderSamplerProperty("RightInput", typeof (AnaglyphEffect), 1);

        /// <summary>
        /// This property is mapped to the Method variable within the pixel shader. 
        /// </summary>
        private static readonly DependencyProperty ShaderMethodProperty = DependencyProperty.Register("ShaderMethod",
                                                                                                      typeof(float),
                                                                                                      typeof (
                                                                                                          AnaglyphEffect
                                                                                                          ),
                                                                                                      new UIPropertyMetadata
                                                                                                          (1.0f,
                                                                                                           PixelShaderConstantCallback
                                                                                                               (0)));

        /// <summary>
        /// This property is mapped to the Method variable within the pixel shader. 
        /// </summary>
        private static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset",
                                                                                                      typeof(float),
                                                                                                      typeof(
                                                                                                          AnaglyphEffect
                                                                                                          ),
                                                                                                      new UIPropertyMetadata
                                                                                                          (0.0f,PixelShaderConstantCallback(1)));

        public AnaglyphMethod Method
        {
            get { return (AnaglyphMethod) GetValue(MethodProperty); }
            set { SetValue(MethodProperty, value); }
        }

        public Brush LeftInput
        {
            get { return (Brush) GetValue(LeftInputProperty); }
            set { SetValue(LeftInputProperty, value); }
        }

        public Brush RightInput
        {
            get { return (Brush) GetValue(RightInputProperty); }
            set { SetValue(RightInputProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Method variable within the shader.
        /// </summary>
        private float ShaderMethod
        {
            set { SetValue(ShaderMethodProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Method variable within the shader.
        /// </summary>
        public  float Offset
        {
            set { SetValue(OffsetProperty, value); }
            get { return (float)GetValue(OffsetProperty); }
        }

        private static void AnaglyphMethodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ef = (AnaglyphEffect) d;
            ef.ShaderMethod = (int) ef.Method;
        }

        private static readonly PixelShader Shader = new PixelShader();
    }
}