//-----------------------------------------------------------------------
// <copyright file="VertexDeclarationFactory.cs" company="Jendrik Illner">
// Jendrik Illner(jendrik.illner@gmail.com)
// Creative Commons Attribution 3.0 United States License. (http://creativecommons.org/licenses/by/3.0/us/)
// </copyright>
//-----------------------------------------------------------------------

namespace b3d
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// This VertexDeclarationFactory has the appilty to create a VertexDeclaration just based on the VertexStructure.
    /// </summary>
    public static class VertexDeclarationFactory
    {
        #region definitions

        /// <summary>
        /// This dictionary contains all the names recognized by this factory. 
        /// Also contains the corresponding VertexElementUsage.
        /// </summary>
        private static Dictionary<string, VertexElementUsage> nameAndElementUsage = new Dictionary
            <string, VertexElementUsage>()
                                                                                        {
                                                                                            {
                                                                                                "Position",
                                                                                                VertexElementUsage.
                                                                                                Position
                                                                                                },
                                                                                            {
                                                                                                "Pos",
                                                                                                VertexElementUsage.
                                                                                                Position
                                                                                                },
                                                                                            {
                                                                                                "TextureCoordinate",
                                                                                                VertexElementUsage.
                                                                                                TextureCoordinate
                                                                                                },
                                                                                            {
                                                                                                "Tex",
                                                                                                VertexElementUsage.
                                                                                                TextureCoordinate
                                                                                                },
                                                                                            {
                                                                                                "TexCoord",
                                                                                                VertexElementUsage.
                                                                                                TextureCoordinate
                                                                                                },
                                                                                            {
                                                                                                "UV",
                                                                                                VertexElementUsage.
                                                                                                TextureCoordinate
                                                                                                },
                                                                                            {
                                                                                                "Tangent",
                                                                                                VertexElementUsage.
                                                                                                Tangent
                                                                                                },
                                                                                            {
                                                                                                "Color",
                                                                                                VertexElementUsage.Color
                                                                                                },
                                                                                            {
                                                                                                "VertexColor",
                                                                                                VertexElementUsage.Color
                                                                                                },
                                                                                            {
                                                                                                "Normal",
                                                                                                VertexElementUsage.
                                                                                                Normal
                                                                                                },
                                                                                            {
                                                                                                "Binormal",
                                                                                                VertexElementUsage.
                                                                                                Binormal
                                                                                                },
                                                                                        };

        /// <summary>
        /// This dictionary contains the types and the corresponding VertexElementFormat supported by this factory.
        /// </summary>
        private static Dictionary<Type, VertexElementFormat> typeAndElementFormat = new Dictionary
            <Type, VertexElementFormat>()
                                                                                        {
                                                                                            {
                                                                                                typeof (float),
                                                                                                VertexElementFormat.
                                                                                                Single
                                                                                                },
                                                                                            {
                                                                                                typeof (Vector2),
                                                                                                VertexElementFormat.
                                                                                                Vector2
                                                                                                },
                                                                                            {
                                                                                                typeof (Vector3),
                                                                                                VertexElementFormat.
                                                                                                Vector3
                                                                                                },
                                                                                            {
                                                                                                typeof (Vector4),
                                                                                                VertexElementFormat.
                                                                                                Vector4
                                                                                                },
                                                                                            {
                                                                                                typeof (Color),
                                                                                                VertexElementFormat.
                                                                                                Color
                                                                                                },
                                                                                        };

        /// <summary>
        /// All supported  VertexElementFormats are listed here, with the corresponding SizeInBytes for the element.
        /// </summary>
        private static Dictionary<VertexElementFormat, short> elementSizes = new Dictionary<VertexElementFormat, short>()
                                                                                 {
                                                                                     {VertexElementFormat.Single, 4},
                                                                                     {VertexElementFormat.Vector2, 8},
                                                                                     {VertexElementFormat.Vector3, 12},
                                                                                     {VertexElementFormat.Vector4, 16},
                                                                                     {VertexElementFormat.Color, 4},
                                                                                 };

        #endregion definitions

        /// <summary>
        /// Generates the vertex declaration based on the provided VertexStruct.
        /// </summary>
        /// <typeparam name="T">The type of the vertex struct to generate the declaration for.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <returns>Returns the generated VertexDeclaration.</returns>
        public static VertexDeclaration GenerateVertexDeclaration<T>(GraphicsDevice device)
        {
            short sizeInBytes = 0;
            return GenerateVertexDeclaration<T>(device, out sizeInBytes);
        }

        /// <summary>
        /// Generates the vertex declaration.
        /// </summary>
        /// <typeparam name="T">The type of the vertex struct to generate the declaration for.</typeparam>
        /// <param name="device">The graphics device.</param>
        /// <param name="sizeInBytes">The size in bytes of the vertex struct.</param>
        /// <returns>Returns the generated VertexDeclaration.</returns>
        public static VertexDeclaration GenerateVertexDeclaration<T>(GraphicsDevice device, out short sizeInBytes)
        {
            List<VertexElement> elements = new List<VertexElement>();

            MemberInfo[] properties = typeof (T).GetFields();

            short offset = 0;

            foreach (var item in properties)
            {
                byte usageIndex = 0;
                short elementSizeInBytes = 0;

                // if the item has a VertexElementAttribue on it that says "ignore" than dont do anything on it
                if (ContainsIgnoreAttribute(item))
                {
                    continue;
                }

                VertexElementUsage elementUsage = GetVertexElementUsage(item, out usageIndex);
                VertexElementFormat elementFormat = GetVertexElementFormat(item);
                elementSizeInBytes = GetSizeInBytes(elementFormat);

                elements.Add(new VertexElement(0, offset, elementFormat, VertexElementMethod.Default, elementUsage,
                                               usageIndex));

                // add the size of the actual vertex element to the offset
                // because the next element will start with this offset from the last element
                offset += elementSizeInBytes;
            }

            sizeInBytes = offset;

            VertexDeclaration declaration = new VertexDeclaration(device, elements.ToArray());

            return declaration;
        }

        /// <summary>
        /// Determines whether [contains ignore attribute] [the specified item].
        /// </summary>
        /// <param name="item">The item to check for the attribute.</param>
        /// <returns>
        ///     <c>true</c> if [contains ignore attribute] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        private static bool ContainsIgnoreAttribute(MemberInfo item)
        {
            if (item.Name == "VertexElements" || item.Name == "SizeInBytes") return true;


            Attribute[] attributes = System.Attribute.GetCustomAttributes(item);

            foreach (Attribute attribute in attributes)
            {
                if (attribute is VertexElementAttribute)
                {
                    // if the element contains a ignore attribute return
                    if (((VertexElementAttribute) attribute).IgnoreElement == true)
                    {
                        return true;
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// Gets the size in bytes.
        /// </summary>
        /// <param name="elementFormat">The element format.</param>
        /// <returns>Returns the size in bytes for the specified elementFormat.</returns>
        private static short GetSizeInBytes(VertexElementFormat elementFormat)
        {
            short sizeInBytes = 0;

            // check if a size for the VertexElementFormat was defined.
            if (elementSizes.ContainsKey(elementFormat))
            {
                // if the size was defined get it
                sizeInBytes = elementSizes[elementFormat];
            }
            else
            {
                throw new NotSupportedException(
                    string.Format("The size of the element format {0} is not provided please add it to the dictionary",
                                  elementFormat));
            }

            return sizeInBytes;
        }

        /// <summary>
        /// Gets the vertex element format.
        /// </summary>
        /// <param name="item">The item to get the format of.</param>
        /// <returns>Returns the element format.</returns>
        private static VertexElementFormat GetVertexElementFormat(MemberInfo item)
        {
            Type type = ((FieldInfo) item).FieldType;
            VertexElementFormat elementFormat = VertexElementFormat.Unused;

            // check if a element format was defined for the given type
            if (typeAndElementFormat.ContainsKey(type))
            {
                // get the format
                elementFormat = typeAndElementFormat[type];
            }
            else
            {
                throw new NotSupportedException(
                    string.Format(
                        "The type {0} is not supported for auto generation please add it to the dictionary containing the supported values",
                        type));
            }

            return elementFormat;
        }

        /// <summary>
        /// Gets the vertex element usage.
        /// </summary>
        /// <param name="item">The member info.</param>
        /// <param name="usageIndex">the usage index for the vertex element</param>
        /// <returns>returns the vertex element usage</returns>
        private static VertexElementUsage GetVertexElementUsage(MemberInfo item, out byte usageIndex)
        {
            string name = item.Name;
            usageIndex = 0;
            VertexElementUsage elementUsage = VertexElementUsage.TessellateFactor;
            bool elementUsageFound = false;

            // get the element Usage

            // check if we can get the meaning of the element from the name
            foreach (string supportedName in nameAndElementUsage.Keys)
            {
                // check if a usageIndex was provided
                string nameWithoutNumber = name;
                string numberCharacter = name.Substring(name.Length - 1);
                byte testUsageIndex = 0;

                // check if whe have a number at the last character
                // when this fails testUsageIndex will be 0, which is also the default
                if (byte.TryParse(numberCharacter, out testUsageIndex))
                {
                    nameWithoutNumber = name.Substring(0, name.Length - 1);
                }

                if (nameWithoutNumber == supportedName)
                {
                    // we got a meaning for the name so use it
                    elementUsage = nameAndElementUsage[nameWithoutNumber];
                    elementUsageFound = true;
                    usageIndex = testUsageIndex;

                    break;
                }
            }

            // if we haven't found the element usage by it's name
            // see if a attribute was provided
            if (elementUsageFound == false)
            {
                Attribute[] attributes = System.Attribute.GetCustomAttributes(item);

                foreach (Attribute attribute in attributes)
                {
                    if (attribute is VertexElementAttribute)
                    {
                        elementUsage = ((VertexElementAttribute) attribute).ElementUsage;
                        usageIndex = ((VertexElementAttribute) attribute).UsageIndex;
                        elementUsageFound = true;
                    }
                }
            }

            if (elementUsageFound == false)
            {
                throw new NotSupportedException(
                    string.Format(
                        "The provided vertex structure could not be transfered into a VertexDeclaration Automaticcly. Please add a VertexAttribute to the {0} member",
                        name));
            }

            return elementUsage;
        }
    }
}