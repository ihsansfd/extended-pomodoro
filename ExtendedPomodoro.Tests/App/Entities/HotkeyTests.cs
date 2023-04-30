using ExtendedPomodoro.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExtendedPomodoro.Tests.App.Entities
{
    public class HotkeyTests
    {
        [Theory]
        [ClassData(typeof(HotkeyTestData))]
        public void ToString_ReturnCorrectString(Hotkey inputHotkey, string expectedString)
        {
            Assert.Contains(expectedString, inputHotkey.ToString());
        } 
    }

    public class HotkeyTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {new Hotkey(Key.A, ModifierKeys.Windows), "Win + A"};
            yield return new object[]
            { new Hotkey(Key.F10, ModifierKeys.Shift), "Shift + F10" };
            yield return new object[] { new Hotkey(Key.A, ModifierKeys.None), "A" };
            yield return new object[] { new Hotkey(Key.None, ModifierKeys.None), "None" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
