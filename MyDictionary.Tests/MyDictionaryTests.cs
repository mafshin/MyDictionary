using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyDictionary.Tests
{
    [TestClass]
    public class MyDictionaryTests
    {
        [TestMethod]
        public void TestAddMany()
        {
            var symbols = GenerateData();
            var myDic = new MyDictionary<string, SymbolInfo>();

            foreach (var item in symbols)
            {
                myDic.Add(item.Symbol, item);
            }

            Assert.AreEqual(symbols.Count(), myDic.Count);
        }

        [TestMethod]
        public void TestAddExistingKeyThrowsException()
        {
            var symbols = GenerateData();
            var myDic = new MyDictionary<string, SymbolInfo>();

            var symbol = symbols.First();
            myDic.Add(symbol.Symbol, symbol);
            Assert.ThrowsException<ArgumentException>(() => myDic.Add(symbol.Symbol, symbol));
        }

            [TestMethod]
        public void TestAddGet()
        {
            var symbols = GenerateData();
            var myDic = new MyDictionary<string, SymbolInfo>();

            var symbol = symbols.First();
            myDic.Add(symbol.Symbol, symbol);

            Assert.AreEqual(1, myDic.Count);

            var symbolInfoFromDic = myDic.Get(symbol.Symbol);

            Assert.IsNotNull(symbolInfoFromDic);
            Assert.AreSame(symbol, symbolInfoFromDic);

            var symbol2 = symbols.Skip(1).First();
            myDic.Add(symbol2.Symbol, symbol2);

            Assert.AreEqual(2, myDic.Count);

            symbolInfoFromDic = myDic.Get(symbol.Symbol);

            Assert.IsNotNull(symbolInfoFromDic);
            Assert.AreSame(symbol, symbolInfoFromDic);

            var symbolInfoFromDic2 = myDic.Get(symbol2.Symbol);

            Assert.IsNotNull(symbolInfoFromDic2);
            Assert.AreSame(symbol2, symbolInfoFromDic2);

            myDic.Remove(symbol.Symbol);

            Assert.AreEqual(1, myDic.Count);

            // Original passed object must keep untouched.
            Assert.IsNotNull(symbol);
            Assert.IsNotNull(symbolInfoFromDic);

            Assert.ThrowsException<KeyNotFoundException>(() => myDic.Get(symbol.Symbol));

            myDic.Remove(symbol2.Symbol);

            Assert.AreEqual(0, myDic.Count);
        }

        private IEnumerable<SymbolInfo> GenerateData()
        {
            var list = new List<SymbolInfo>();
            var random = new Random();
            for (int i = 1; i < 1000; i++)
            {
                list.Add(new SymbolInfo()
                {
                    Symbol = $"IRO1{i.ToString().PadLeft(6, '0')}",
                    Price = random.Next(1000, 2000)
                });
            }

            return list;
        }

        public class SymbolInfo
        {
            public string Symbol { get; set; }
            public decimal Price { get; set; }
        }
    }
}
