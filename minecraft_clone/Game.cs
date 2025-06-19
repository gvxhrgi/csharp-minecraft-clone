using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;

namespace OpenTK_Minecraft_Clone
{
    internal class Game : GameWindow
    {
        float[] vertices =
        {
            -0.5f, 0.5f, 0f, // Top Left Vertex
            0.5f, 0.5f, 0f, // Top Right Vertex
            0.5f, -0.5f, 0f, // Bottom Right Vertex
            -0.5f, -0.5f, 0f // Bottom Left Vertex
        };

        float[] texCoords =
        {
            0f, 1f,
            1f, 1f,
            1f, 0f,
            0f, 0f
        };

        uint[] indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        //Render Pipelines

        int vao;
        int shaderProgram;
        int vbo;
        int textureVBO;
        int ebo;
        int textureID;

        // Constants
        int width, height;
        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.width = width;
            this.height = height;
            CenterWindow(new Vector2i(width, height));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Generate VAO
            vao = GL.GenVertexArray();

            // Bind VAO
            GL.BindVertexArray(vao);

            // ---- Vertices VBO ----

            vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Put Vertex VBO In Slot 0 Of Our VAO
            // Point Slot (0) Of The VAO To The Currently Bound VBO (vbo)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            // Enable The Slot
            GL.EnableVertexArrayAttrib(vao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // ---- Texture VBO ----
            textureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);

            // Put Texture VBO In Slot 1 Of Our VAO
            // Point Slot (1) Of The VAO To The Currently Bound VBO (vbo)
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            // Enable The Slot
            GL.EnableVertexArrayAttrib(vao, 1);

            // Unbind VBO and VAO 
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // Bind EBO
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Unbind EBO
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Create Shader Program
            shaderProgram = GL.CreateProgram();

            // Create Vertex Shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
            GL.CompileShader(vertexShader);

            // Create Fragment Shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
            GL.CompileShader(fragmentShader);

            // Attach Shaders To Program
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            // Link Program To OpenGL
            GL.LinkProgram(shaderProgram);

            // Delete Shaders
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // ----- Textures -----
            textureID = GL.GenTexture();

            // Activate Texture In Unit
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            // Texture Parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            // Load Image
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/dirt.jpg"), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);

            // Unbind The Texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteTexture(textureID);
            GL.DeleteProgram(shaderProgram);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.6f, 0.3f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw The Triangle
            GL.UseProgram(shaderProgram);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.BindTexture(TextureTarget.Texture2D, textureID);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            // GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        // Load Shader Source File
        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed To Load Shader Source File: " + ex.Message);
            }

            return shaderSource;
        }
    }
}