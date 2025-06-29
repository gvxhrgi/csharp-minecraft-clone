
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK_Minecraft_Clone.Graphics;

namespace OpenTK_Minecraft_Clone.World
{
    internal class Chunk
    {
        private List<Vector3> chunkVerts;
        private List<Vector2> chunkUVs;
        private List<uint> chunkIndices;

        const int SIZE = 16;
        const int HEIGHT = 32;
        public Vector3 position;

        VAO chunkVAO;
        VBO chunkVertexVBO;
        VBO chunkUVVBO;
        IBO chunkIBO;

        Texture texture;

        public Block[,,] chunkBlocks = new Block[SIZE, HEIGHT, SIZE];

        public uint indexCount;

        public Chunk(Vector3 position)
        {
            this.position = position;

            chunkVerts = new List<Vector3>();
            chunkUVs = new List<Vector2>();
            chunkIndices = new List<uint>();

            float[,] heightMap = GenerateChunk();

            GenerateBlocks(heightMap);
            GenFaces(heightMap);
            BuildChunk();
        }

        public float[,] GenerateChunk()
        {
            float[,] heightMap = new float[SIZE, SIZE];
            SimplexNoise.Noise.Seed = 1561895;
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    heightMap[x, z] = SimplexNoise.Noise.CalcPixel2D(x, z, 0.01f);
                }
            }

            return heightMap;
        }
        public void GenerateBlocks(float[,] heightMap)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int columnHeight = (int)(heightMap[x, z] / 10);
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        if (y < columnHeight)
                        {
                            chunkBlocks[x, y, z] = new Block(new Vector3(x, y, z), BlockType.DIRT); ;
                        }
                        else
                        {
                            chunkBlocks[x, y, z] = new Block(new Vector3(x, y, z), BlockType.EMPTY);
                        }
                    }
                }
            }
        }
        public void GenFaces(float[,] heightMap)
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int columnHeight = (int)(heightMap[x, z] / 10);
                    for (int y = 0; y < columnHeight; y++)
                    {
                        //left face
                        int numFaces = 0;
                        if (x > 0)
                        {
                            if (chunkBlocks[x - 1, y, z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x, y, z], Faces.LEFT);
                                numFaces++;
                            }
                        }
                        else
                        {
                            IntegrateFace(chunkBlocks[x, y, z], Faces.LEFT);
                            numFaces++;
                        }
                        //right face
                        if (x < SIZE - 1)
                        {
                            if (chunkBlocks[x + 1, y, z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x, y, z], Faces.RIGHT);
                                numFaces++;
                            }
                        }
                        else
                        {
                            IntegrateFace(chunkBlocks[x, y, z], Faces.RIGHT);
                            numFaces++;
                        }
                        //top face
                        if (y < columnHeight - 1)
                        {
                            if (chunkBlocks[x, y + 1, z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x, y, z], Faces.TOP);
                                numFaces++;
                            }
                        }
                        else
                        {
                            IntegrateFace(chunkBlocks[x, y, z], Faces.TOP);
                            numFaces++;
                        }
                        //bottom face
                        if (y > 0)
                        {
                            if (chunkBlocks[x, y - 1, z].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x, y, z], Faces.BOTTOM);
                                numFaces++;
                            }
                        }
                        else
                        {
                            IntegrateFace(chunkBlocks[x, y, z], Faces.BOTTOM);
                            numFaces++;
                        }
                        // //front face
                        if (z < SIZE - 1)
                        {
                            if (chunkBlocks[x, y, z + 1].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x, y, z], Faces.FRONT);
                                numFaces++;
                            }
                        }
                        else
                        {
                            IntegrateFace(chunkBlocks[x, y, z], Faces.FRONT);
                            numFaces++;
                        }
                        //back face
                        if (z > 0)
                        {
                            if (chunkBlocks[x, y, z - 1].type == BlockType.EMPTY)
                            {
                                IntegrateFace(chunkBlocks[x, y, z], Faces.BACK);
                                numFaces++;
                            }
                        }
                        else
                        {
                            IntegrateFace(chunkBlocks[x, y, z], Faces.BACK);
                            numFaces++;
                        }
                        AddIndices(numFaces);
                    }
                }
            }
        }
        public void IntegrateFace(Block block, Faces face)
        {
            var faceData = block.GetFace(face);
            chunkVerts.AddRange(faceData.vertices);
            chunkUVs.AddRange(faceData.uv);
        }

        public void BuildChunk()
        {
            chunkVAO = new VAO();
            chunkVAO.Bind();

            chunkVertexVBO = new VBO(chunkVerts);
            chunkVertexVBO.Bind();
            chunkVAO.LinkToVAO(0, 3, chunkVertexVBO);

            chunkUVVBO = new VBO(chunkUVs);
            chunkUVVBO.Bind();
            chunkVAO.LinkToVAO(1, 2, chunkUVVBO);

            chunkIBO = new IBO(chunkIndices);

            texture = new Texture("dirt.jpg");
        }

        public void Render(ShaderProgram program)
        {
            program.Bind();
            chunkVAO.Bind();
            chunkIBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void AddIndices(int amtFaces)
        {
            for (int i = 0; i < amtFaces; i++)
            {
                chunkIndices.Add(0 + indexCount);
                chunkIndices.Add(1 + indexCount);
                chunkIndices.Add(2 + indexCount);
                chunkIndices.Add(2 + indexCount);
                chunkIndices.Add(3 + indexCount);
                chunkIndices.Add(0 + indexCount);
                indexCount += 4;
            }
        }

        public void Delete()
        {
            chunkVAO.Delete();
            chunkVertexVBO.Delete();
            chunkUVVBO.Delete();
            chunkIBO.Delete();
            texture.Delete();
        }
    }
}