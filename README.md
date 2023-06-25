# Caffeinated3D
#### Useful classes and utilites for working with Monogame/XNA framework in the third dimension.
This is mostly a personal project, but I will try to keep it neat and tidy for my own sanity as well as anyone reading!

### Quick Note On Shaders
Code in the Shaders directory is written in HLSL, these files can be renamed/converted to .fx and then compiled via Monogame's mgfx compiler.
The output of shader compilation is a binary .xnb file that can be added directly into your project using the Microsoft.XNA.Framework.Graphics.Effect class's
constructor.  Here is an example.

byte[] diffuseShaderBytecode = File.ReadAllBytes(projectDir + "\\Shaders\\BasicDiffuse.mgfx");
_basicDiffuseShader = new Effect(GraphicsDevice, diffuseShaderBytecode);


