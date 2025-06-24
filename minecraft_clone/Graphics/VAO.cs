using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Minecraft_Clone.Graphics
{
    internal class VAO
    {
        public int ID;
        public VAO()
        {
            ID = GL.GenVertexArray();
            GL.BindVertexArray(ID);
        }
        public void LinkToVAO(int location, int size, VBO vbo)
        {
            Bind();
            vbo.Bind();
            GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(location);
            vbo.Unbind();
        }

        public void Bind()
        {
            GL.BindVertexArray(ID);
        }
        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
        public void Delete()
        {
            GL.DeleteVertexArray(ID);
        }
    }
}