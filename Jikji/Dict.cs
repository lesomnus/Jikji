using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Jikji
{
    [Serializable]
    public class Dict
    {
        private Dictionary<string, int>[] _book;
        private Mold _mold;
        public Mold Mold { get { return _mold; } private set { } }

        internal Dict(Mold mold, Dictionary<string, int>[] book)
        {
            _mold = mold;
            _book = book;
        }

        public bool TryFind(Bitmap raster, Style weight, out int value)
        {
            int index = (int)((weight & Style.Weight)
                            | (raster.Width == _mold.Size.Width
                            ? Style.Full : Style.Half));

            return _book[index].TryGetValue(raster.ToValidMap().ToHexString(), out value);
        }

        public bool TryFind(Bitmap raster, out int value)
        {
            return TryFind(raster, Style.Regular, out value)
                || TryFind(raster, Style.Bold, out value);
        }

        static public bool TryImportFrom(string path, out Dict dict)
        {
            dict = null;

            if (!File.Exists(path)) return false;

            var file = new FileStream(path, FileMode.Open);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            try
            {
                dict = (Dict)formatter.Deserialize(file);
            }
            catch
            {
                return false;
            }
            finally
            {
                file.Close();
            }

            return true;
        }

        public bool TryExportTo(string path)
        {
            if (_book == null
             || _mold == null) return false;

            var file = new FileStream(path, FileMode.Create);
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            try
            {
                formatter.Serialize(file, this);
            }
            catch
            {
                return false;
            }
            finally
            {
                file.Close();
            }
            
            return true;
        }
    }
}
