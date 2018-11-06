using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jikji
{
    public class Printer
    {
        private Dictionary<string, int>[] _book = new Func<Dictionary<string, int>[]>(() =>
        {
            var rst = new Dictionary<string, int>[4];

            for (int i = 0; i < 4; i++)
            {
                rst[i] = new Dictionary<string, int>();
            }

            return rst;
        })(); //new Dictionary<string, int>[4];
        private Mold _mold;
        public Mold Mold { get { return _mold; } private set { } }

        public Printer(Mold mold)
        {
            _mold = mold;
        }

        public Dict Print()
        {
            return new Dict(_mold, _book);
        }

        public Printer Add(int what)
        {
            Style style = (Mold.IsHalf(what) ? Style.Half : Style.Full) | Style.Bold;
            var key = _mold.Press(what, Style.Bold).ToValidMap().ToHexString();
            if (_book[(int)style].ContainsKey(key)) return this;

            _book[(int)style].Add(key, what);

            key = _mold.Press(what, Style.Regular).ToValidMap().ToHexString();

            _book[(int)(style & ~Style.Bold | Style.Regular)].Add(key, what);

            return this;
        }
        public Printer Add(int from, int to)
        {
            to++;
            for(int i = from; i < to; i++)
            {
                Add(i);
            }

            return this;
        }
    }
}
