using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace OpenTK_Minecraft_Clone.Graphics
{
    internal class Texture
    {
        public int ID;
        public Texture(string filepath)
        {
            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Load Image
            StbImage.stbi_set_flip_vertically_on_load(1);
            string texturePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Textures", filepath);
            ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead(texturePath), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);

            Unbind();
        }
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public void Delete()
        {
            GL.DeleteTexture(ID);
        }
    }
}