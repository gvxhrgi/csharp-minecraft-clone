using OpenTK.Graphics.OpenGL4;

namespace OpenTK_Minecraft_Clone.Graphics
{
    internal class ShaderProgram
    {
        public int ID;
        public ShaderProgram(string vertexShaderSource, string fragmentShaderSource)
        {
            // Create Shader Program
            ID = GL.CreateProgram();

            // Create Vertex Shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderSource));
            GL.CompileShader(vertexShader);

            // Create Fragment Shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderSource));
            GL.CompileShader(fragmentShader);

            // Attach Shaders To Program
            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);

            // Link Program To OpenGL
            GL.LinkProgram(ID);

            // Delete Shaders
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }
        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";
            // Get the base directory where the executable is running from
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Combine with the shaders path
            string fullPath = Path.Combine(baseDirectory, "Shaders", filePath);

            try
            {
                using (StreamReader reader = new StreamReader(fullPath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load shader at {fullPath}: {ex.Message}");
            }

            return shaderSource;
        }

        public void Bind()
        {
            GL.UseProgram(ID);
        }
        public void Unbind()
        {
            GL.UseProgram(0);
        }
        public void Delete()
        {
            GL.DeleteShader(ID);
        }
    }
}