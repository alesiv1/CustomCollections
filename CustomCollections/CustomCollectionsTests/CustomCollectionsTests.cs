using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CustomCollections;

namespace CustomCollectionsTests
{
    public class CustomHashTableTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Clear_HashTableIsEmpty_ReturnTrue()
        {
            CustomHashTable<int, string> hashTable = new CustomHashTable<int, string>
            {
                { 1, "1"},
                { 2, "2"},
                { 3, "3"}
            };

            hashTable.Clear();

            Assert.IsEmpty(hashTable);
        }

        [Test]
        public void Contains_SomeElement_ReturnTrue()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>();
            for (var index = 1; index <= 100; index++)
                hashTable.Add(index.ToString(), index);
            for (var index = 1; index <= 100; index++)
            {
                var containsItem = hashTable.Contains(new KeyValuePair<string, int>(index.ToString(), index));
                Assert.IsTrue(containsItem);
            }
        }

        [Test]
        public void Contains_SomeElement_ReturnFalse()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>();
            for (var index = 1; index <= 100; index++)
                hashTable.Add(index.ToString(), index);
            for (var index = 100; index <= 200; index++)
            {
                var containsItem = hashTable.Contains(new KeyValuePair<string, int>(index.ToString(), 35));
                Assert.IsFalse(containsItem);
            }
        }

        [Test]
        public void TryGetValue_SomeElement_ReturnTrue()
        {
            CustomHashTable<string, string> hashTable = new CustomHashTable<string, string>
            {
                { "1", "One" },
                { "2", "Two" },
                { "4", "Four" },
                { "5", "Five" },
                { "6", "Six" },
                { "13", "Three, No" }
            };

            var containsItem = hashTable.TryGetValue("13", out var value);

            Assert.IsTrue(containsItem);
            Assert.AreEqual(value, "Three, No");
        }

        [Test]
        public void TryGetValue_SomeElement_ReturnFalse()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>
            {
                { "1", 1 },
                { "2", 2 },
                { "4", 3 },
                { "5", 4 },
                { "6", 5 },
                { "13", 6 }
            };

            var containsItem = hashTable.TryGetValue("3", out var value);

            Assert.IsFalse(containsItem);
            Assert.AreEqual(value, default(int));

        }

        [Test]
        public void CopyTo_ArrayContainItemsWithHashTable_ReturnTrue()
        {
            CustomHashTable<int, char> hashTable = new CustomHashTable<int, char>();
            KeyValuePair<int, char>[] array = new KeyValuePair<int, char>[100];
            for (int i = 0; i < 100; i++)
            {
                hashTable.Add(i, i.ToString()[0]);
            }
            hashTable.CopyTo(array, 0);

            foreach (var item in array)
            {
                Assert.AreEqual(item.Value, hashTable[item.Key]);
            }
        }

        [Test]
        public void CopyTo_ArrayContainItemsWithHashTable_ReturnFalse()
        {
            CustomHashTable<int, char> hashTable = new CustomHashTable<int, char>
            {
                { 1, 't' },
                { 2, 'y' },
                { 4, 'i' },
                { 5, 'c' },
                { 6, 'r' },
                { 13, 'o' }
            };
            KeyValuePair<int, char>[] array = new KeyValuePair<int, char>[5];
            array[0] = new KeyValuePair<int, char>(0, 'v');

            try
            {
                hashTable.CopyTo(array, 1);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(false);
            }
        }

        [Test]
        public void Add_SameElement_ReturnFalse()
        {
            CustomHashTable<int, string> hashTable = new CustomHashTable<int, string>()
            {
                {13, "1"}
            };

            try
            {
                hashTable.Add(new KeyValuePair<int, string>(13, "2"));
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(false);
            }
        }

        [Test]
        public void Remote_Element_ReturnTrue()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>();
            for (int i = 0; i < 100; i++)
            {
                hashTable.Add(i.ToString(), i);
            }

            for (int i = 0; i < 100; i++)
            {
                var isRemote = hashTable.Remove(new KeyValuePair<string, int>(i.ToString(), i));
                Assert.IsTrue(isRemote);
            }
        }

        [Test]
        public void Remote_Element_ReturnFalse()
        {
            CustomHashTable<string, int> hashTable = new CustomHashTable<string, int>();
            for (int i = 0; i < 100; i++)
            {
                hashTable.Add(i.ToString(), i);
            }

            for (int i = 100; i < 200; i++)
            {
                var isRemote = hashTable.Remove(new KeyValuePair<string, int>(i.ToString(), i));
                Assert.IsFalse(isRemote);
            }
        }

        [Test]
        public void Remote_ElementNotFound_ReturnTrue()
        {
            var hashTable = new CustomHashTable<int, int>();

            for (var index = 1; index <= 100; index++)
                hashTable.Add(index, index);

            for (var index = 1; index <= 100; index++)
            {
                var isRemote = hashTable.Remove(index);
                var isElement = hashTable.ContainsKey(index);

                Assert.AreEqual(isRemote, !isElement);
            }
        }

        [Test]
        public void GetEnumerator_GetAllElements_GetTrue()
        {
            CustomHashTable<int, string> hashTable = new CustomHashTable<int, string>
            {
                { 1, "1"},
                { 2, "2"},
                { 3, "3"},
                { 4, "4"},
                { 5, "5"},
                { 6, "6"}
            };
            KeyValuePair<int, string>[] sameElements = new KeyValuePair<int, string>[]{
                new KeyValuePair<int, string>( 1, "1"),
                new KeyValuePair<int, string>( 2, "2"),
                new KeyValuePair<int, string>( 3, "3"),
                new KeyValuePair<int, string>( 4, "4"),
                new KeyValuePair<int, string>( 5, "5"),
                new KeyValuePair<int, string>( 6, "6")
            };

            foreach (var element in hashTable)
            {
                Assert.IsTrue(sameElements.Contains(element));
            }
        }

        [Test]
        public void AddALotsOfIntegerItems_ItemsAreInTheDictionary()
        {
            var hashTable = new CustomHashTable<int, int>();

            for (var index = 1; index <= 1000; index++)
                hashTable.Add(index, index);
            for (var index = 1; index <= 1000; index++)
                Assert.AreEqual(index, hashTable[index]);
        }

        [Test]
        public void AddALotsOfStringItems_ItemsAreInTheDictionary()
        {
            var hashTable = new CustomHashTable<string, string>();

            for (var index = 1; index <= 1000; index++)
                hashTable.Add($"Key_{index}", $"Value_{index}");

            for (var index = 1; index <= 1000; index++)
                Assert.AreEqual($"Value_{index}", hashTable[$"Key_{index}"]);
        }

        [Test]
        public void ChangeItems_SetAnotherValue_GetFalse()
        {
            var hashTable = new CustomHashTable<int, int>();

            for (var index = 1; index <= 100; index++)
                hashTable.Add(index, index);
            for (var index = 1; index <= 100; index++)
                hashTable[index] = 101;
            for (var index = 1; index <= 100; index++)
                Assert.IsFalse(index == hashTable[index]);
        }
    }
}