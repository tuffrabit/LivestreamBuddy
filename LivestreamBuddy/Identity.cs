using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivestreamBuddy
{
    public class Identity
    {
        public Identity()
        {
            this.Name = string.Empty;
            this.Exists = false;
            this.Password = string.Empty;
            this.offset = -1;
        }

        public Identity(string name)
            : this()
        {
            this.Name = name;

            find();
        }

        # region Properties

        public string Name { get; private set; }

        public string Password { get; private set; }

        public bool Exists { get; private set; }

        private int offset;

        # endregion

        # region Public Methods

        public void Add(string password)
        {
            if (!this.Exists)
            {
                if (string.IsNullOrEmpty(this.Name))
                {
                    throw new ArgumentNullException("Must provide a name.");
                }

                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("Must provide a password.");
                }

                this.Password = password;
                this.writeField();
            }
        }

        public void ChangePassword(string oldPassword, string oldPassword2, string newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(oldPassword2) || string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("No arguments can be null.");
            }

            if (string.Compare(oldPassword, oldPassword2) != 0)
            {
                throw new Exception("Passwords do not match.");
            }

            if (!this.Exists)
            {
                this.Add(newPassword);
            }
            else
            {
                this.Password = newPassword;
                this.writeField();
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        # endregion

        # region Private Methods

        private void writeField()
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Name)));

            if (nameBytes.Length > 64)
            {
                throw new Exception("Name is to long.");
            }

            byte[] passwordBytes = Encoding.ASCII.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Password)));

            if (passwordBytes.Length > 64)
            {
                throw new Exception("Password is to long.");
            }

            byte[] bytes = new byte[128];

            Buffer.BlockCopy(nameBytes, 0, bytes, 0, nameBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, bytes, 64, passwordBytes.Length);

            using (BinaryWriter writer = new BinaryWriter(File.Open("identities.bin", FileMode.OpenOrCreate)))
            {
                if (this.offset == -1)
                {
                    writer.Seek(0, SeekOrigin.End);
                }
                else
                {
                    writer.Seek(this.offset, SeekOrigin.Begin);
                }

                writer.Write(bytes);

                this.offset = Convert.ToInt32(writer.BaseStream.Position - 128);
            }
        }

        private void find()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                throw new ArgumentNullException("Must provide a name.");
            }

            using (BinaryReader reader = new BinaryReader(File.Open("identities.bin", FileMode.OpenOrCreate)))
            {
                byte[] chunk = reader.ReadBytes(128);
                while (chunk.Length > 0)
                {
                    string name = Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(chunk, 0, countTillEndOfString(chunk, 0, 64))));

                    if (string.Compare(this.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        this.Password = Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(chunk, 64, countTillEndOfString(chunk, 64, 128))));
                        this.offset = Convert.ToInt32(reader.BaseStream.Position - 128);
                        this.Exists = true;

                        break;
                    }

                    chunk = reader.ReadBytes(128);
                }
            }
        }

        # endregion

        # region Static Methods

        private static int countTillEndOfString(byte[] src, int start, int end)
        {
            int count = -1;

            for (; start < end; start++)
            {
                byte b = src[start];
                count++;

                if (b == 0)
                {
                    break;
                }
            }

            return count;
        }

        public static Identity[] GetAllByName()
        {
            List<Identity> identities = new List<Identity>();

            using (BinaryReader reader = new BinaryReader(File.Open("identities.bin", FileMode.OpenOrCreate)))
            {
                byte[] chunk = reader.ReadBytes(128);
                while (chunk.Length > 0)
                {
                    Identity identity = new Identity();

                    identity.Name = Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(chunk, 0, countTillEndOfString(chunk, 0, 64))));
                    identity.Password = Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.ASCII.GetString(chunk, 64, countTillEndOfString(chunk, 64, 128))));
                    identity.offset = Convert.ToInt32(reader.BaseStream.Position - 128);
                    identity.Exists = true;
                    identities.Add(identity);

                    chunk = reader.ReadBytes(128);
                }
            }

            return identities.ToArray();
        }

        # endregion
    }
}
