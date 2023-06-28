using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Caffeinated3D.Rendering
{
    public class SkyboxC3D
    {
        private Model _skybox;
        private TextureCube _textureCube;
        private SkyboxShaderC3D _skyboxShader;
        private EffectManagerC3D _effectManager;
        private float _skyboxSize;

        public SkyboxC3D(Model model, TextureCube texture, SkyboxShaderC3D shader)
        {
            _skybox = model;
            _textureCube = texture;
            _skyboxShader = shader;
            _effectManager = new EffectManagerC3D();
            _effectManager.AddEffect(_skyboxShader);
            _skyboxSize = 10.0f;
        }

        public void Draw()
        {
            _effectManager.SetEffectParams();

            foreach(Effect effect in _effectManager.Effects)
            {
                foreach(EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    foreach(ModelMesh mesh in _skybox.Meshes)
                    {
                        mesh.Draw();
                    }
                }
            }
        }

        public void DrawWithoutEffectManager(Effect effect, Matrix world, Matrix view, Matrix projection, Vector3 cameraPos)
        {
            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach(ModelMesh mesh in _skybox.Meshes)
                {
                    foreach(ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = effect;
                        part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(_skyboxSize) * Matrix.CreateTranslation(cameraPos));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyboxTexture"].SetValue(_textureCube);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPos);
                    }

                    mesh.Draw();
                }
            }
        }

    }
}
