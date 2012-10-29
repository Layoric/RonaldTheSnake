//-----------------------------------------------------------------------------
// ParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace RonaldTheSnake
{
    /// <summary>
    /// A relatively simple particle system.  We recycle particles instead of creating
    /// and destroying them as we need more.  "Effects" are created via factory methods
    /// on ParticleSystem, rather than a data driven model due to the relatively low
    /// number of effects.
    /// </summary>
    public class ParticleSystem : IXmlSerializable
    {
        Random random;

        SpriteBatch spriteBatch;

        List<Particle> particles;

        Dictionary<string, Texture2D> textureDictionary;

        Vector2 gravity = new Vector2(0.0f, 0.05f);

        /// <summary>
        /// This constructor should only be used by the XML serializer.
        /// </summary>
        public ParticleSystem()
        {
            random = new Random();

            particles = new List<Particle>();
        }

        public ParticleSystem(ContentManager content, SpriteBatch spriteBatch)
        {
            random = new Random();

            particles = new List<Particle>();

            InitializeAssets(content, spriteBatch);
        }

        /// <summary>
        /// Initializes the particle system's assets. Call this method before attempting to use the particle system.
        /// </summary>
        /// <param name="content">The content manager to use for loading particle textures.</param>
        /// <param name="spriteBatch">The sprite batch to use for drawing the particles on screen.</param>
        public void InitializeAssets(ContentManager content, SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;

            textureDictionary = new Dictionary<string, Texture2D>();

            //textureDictionary["tank_tire"] = content.Load<Texture2D>("tank_tire");
            //textureDictionary["tank_top"] = content.Load<Texture2D>("tank_top");
            //textureDictionary["fire"] = content.Load<Texture2D>("fire");
            textureDictionary["smoke"] = content.Load<Texture2D>("smoke");
        }

        /// <summary>
        /// Sets the textures for all particles according to the texture name in their TextureName field.
        /// </summary>
        public void SetParticleTextures()
        {
            foreach (Particle particle in particles)
            {
                particle.Texture = textureDictionary[particle.TextureName];
            }
        }

        /// <summary>
        /// Update all active particles.
        /// </summary>
        /// <param name="elapsed">The amount of time elapsed since last Update.</param>
        public void Update(float elapsed)
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                particles[i].Life -= elapsed;
                if (particles[i].Life <= 0.0f)
                {
                    continue;
                }
                particles[i].Velocity = particles[i].Velocity + gravity;
                particles[i].Position += particles[i].Velocity * elapsed;
                particles[i].Rotation += particles[i].RotationRate * elapsed;
                particles[i].Alpha += particles[i].AlphaRate * elapsed;
                particles[i].Scale += particles[i].ScaleRate * elapsed;

                if (particles[i].Alpha <= 0.0f)
                    particles[i].Alpha = 0.0f;                                    
            }
        }

        /// <summary>
        /// Draws the particles.
        /// </summary>
        public void Draw()
        {
            for (int i = 0; i < particles.Count; ++i)
            {
                Particle p = particles[i];
                if (p.Life <= 0.0f)
                    continue;

                float alphaF = 255.0f * p.Alpha;
                if (alphaF < 0.0f)
                    alphaF = 0.0f;
                if (alphaF > 255.0f)
                    alphaF = 255.0f;

                spriteBatch.Draw(p.Texture, p.Position, null, new Color(p.Color.R, p.Color.G, p.Color.B, (byte)alphaF), p.Rotation, new Vector2(p.Texture.Width / 2, p.Texture.Height / 2), p.Scale, SpriteEffects.None, 0.0f);
            }
        }

        /// <summary>
        /// Creates a particle, preferring to reuse a dead one in the particles list 
        /// before creating a new one.
        /// </summary>
        /// <returns></returns>
        Particle CreateParticle()
        {
            Particle p = null;

            for (int i = 0; i < particles.Count; ++i)
            {
                if (particles[i].Life <= 0.0f)
                {
                    p = particles[i];
                    break;
                }
            }

            if (p == null)
            {
                p = new Particle();
                particles.Add(p);
            }

            if(p.Color == null)
                p.Color = new Color(0.5f,0.5f,0.5f);

            return p;
        }

        /// <summary>
        /// Creates the effect for when the player dies.
        /// </summary>
        /// <param name="position">Where on the screen to create the effect.</param>
        public void CreatePlayerExplosion(Vector2 position)
        {
            Particle p = null;

            for (int i = 0; i < 16; ++i)
            {
                p = CreateParticle();
                p.Position = position;
                p.RotationRate = -6.0f + 12.0f * (float)random.NextDouble();
                p.Scale = 0.05f;
                p.ScaleRate = 0.0005f;// *(float)random.NextDouble();
                p.Alpha = 2.0f;
                p.AlphaRate = -0.25f;
                p.Velocity.X = -0.2f + 0.4f * (float)random.NextDouble();
                p.Velocity.Y = -0.2f + -0.8f * (float)random.NextDouble();
                p.TextureName = "smoke";
                p.Texture = textureDictionary[p.TextureName];
                p.Life = 2000.0f;
                p.Color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
            }
        }

        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reconstructs the particle system from an XML representation.
        /// </summary>
        /// <param name="reader">The XML reader containing a representation of a particle system.</param>
        public void ReadXml(XmlReader reader)
        {
            // Advance into the wrapper element
            reader.Read();

            int particleCount = int.Parse(reader.GetAttribute("Amount"));

            if (particleCount < 1)
            {
                // Read the end elements
                reader.Read();
                reader.Read();
                return;
            }

            // Read into the first particle element
            reader.Read();

            XmlSerializer particleSerializer = new XmlSerializer(typeof(Particle));

            for (int i = 0; i < particleCount; i++)
            {
                Particle particle = particleSerializer.Deserialize(reader) as Particle;                
                particles.Add(particle);
            }

            // Read the end elements
            reader.Read();
            reader.Read();
        }

        /// <summary>
        /// Writes an XML representation of the particle system, by serializing its particle list.
        /// </summary>
        /// <param name="writer">Xml writer into which the XML data is written.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Particles");
            writer.WriteAttributeString("Amount", particles.Count.ToString());

            XmlSerializer particleSerializer = new XmlSerializer(typeof(Particle));

            foreach (Particle particle in particles)
            {
                particleSerializer.Serialize(writer, particle);
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    /// <summary>
    /// A basic particle.  Since this is strictly a data class, I decided to not go
    /// the full property route and used public fields instead.
    /// </summary>
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        [XmlIgnore]
        public Texture2D Texture;
        public string TextureName;
        public float RotationRate;
        public float Rotation;
        public float Life;
        public float AlphaRate;
        public float Alpha;
        public float ScaleRate;
        public float Scale;
        public Color Color = Color.White;
    }    
}