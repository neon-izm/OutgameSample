using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniRx;
using NewDemo.Core.Scripts.Domain.Character.Model;

namespace NewDemo.Tests.Domain
{
    /// <summary>
    /// Characterクラスのユニットテスト
    /// </summary>
    public class CharacterTests
    {
        private Character _character;

        [SetUp]
        public void SetUp()
        {
            _character = new Character();
        }

        [TearDown]
        public void TearDown()
        {
            _character = null;
        }

        #region 色設定のテスト

        [Test]
        public void SetRightHandColor_正常な色_正しく設定される()
        {
            // Arrange
            var testColor = Color.red;

            // Act
            _character.SetRightHandColor(testColor);

            // Assert
            Assert.AreEqual(testColor, _character.RightHandColor.Value);
        }

        [Test]
        public void SetLeftHandColor_正常な色_正しく設定される()
        {
            // Arrange
            var testColor = Color.green;

            // Act
            _character.SetLeftHandColor(testColor);

            // Assert
            Assert.AreEqual(testColor, _character.LeftHandColor.Value);
        }

        [Test]
        public void SetRightFootColor_正常な色_正しく設定される()
        {
            // Arrange
            var testColor = Color.blue;

            // Act
            _character.SetRightFootColor(testColor);

            // Assert
            Assert.AreEqual(testColor, _character.RightFootColor.Value);
        }

        [Test]
        public void SetLeftFootColor_正常な色_正しく設定される()
        {
            // Arrange
            var testColor = Color.yellow;

            // Act
            _character.SetLeftFootColor(testColor);

            // Assert
            Assert.AreEqual(testColor, _character.LeftFootColor.Value);
        }

        #endregion

        #region スケール設定のテスト

        [Test]
        [TestCase(0.5f, 0.5f)]  // 最小値
        [TestCase(1.0f, 1.0f)]  // デフォルト値
        [TestCase(1.5f, 1.5f)]  // 最大値
        [TestCase(0.851f, 0.9f)] // 四捨五入テスト（0.85f -> 0.9f）
        [TestCase(0.84f, 0.8f)] // 四捨五入テスト（0.84f -> 0.8f）
        public void SetHeadScale_有効な値_正しく設定される(float input, float expected)
        {
            // Act
            _character.SetHeadScale(input);

            // Assert
            Assert.AreEqual(expected, _character.HeadScale.Value, 0.001f);
        }

        [Test]
        [TestCase(0.3f, 0.5f)]  // 下限を下回る値 -> 最小値にクランプ
        [TestCase(1.8f, 1.5f)]  // 上限を上回る値 -> 最大値にクランプ
        public void SetHeadScale_範囲外の値_クランプされる(float input, float expected)
        {
            // Act
            _character.SetHeadScale(input);

            // Assert
            Assert.AreEqual(expected, _character.HeadScale.Value, 0.001f);
        }

        [Test]
        [TestCase(0.5f, 0.5f)]  // 最小値
        [TestCase(1.0f, 1.0f)]  // デフォルト値
        [TestCase(1.5f, 1.5f)]  // 最大値
        public void SetFootScale_有効な値_正しく設定される(float input, float expected)
        {
            // Act
            _character.SetFootScale(input);

            // Assert
            Assert.AreEqual(expected, _character.FootScale.Value, 0.001f);
        }

        [Test]
        public void IncreaseHeadScale_通常操作_10パーセント増加()
        {
            // Arrange
            var initialScale = 1.0f;
            _character.SetHeadScale(initialScale);

            // Act
            _character.IncreaseHeadScale();

            // Assert
            Assert.AreEqual(1.1f, _character.HeadScale.Value, 0.001f);
        }

        [Test]
        public void IncreaseHeadScale_最大値で実行_最大値のまま()
        {
            // Arrange
            _character.SetHeadScale(1.5f);

            // Act
            _character.IncreaseHeadScale();

            // Assert
            Assert.AreEqual(1.5f, _character.HeadScale.Value, 0.001f);
        }

        [Test]
        public void DecreaseHeadScale_通常操作_10パーセント減少()
        {
            // Arrange
            var initialScale = 1.0f;
            _character.SetHeadScale(initialScale);

            // Act
            _character.DecreaseHeadScale();

            // Assert
            Assert.AreEqual(0.9f, _character.HeadScale.Value, 0.001f);
        }

        [Test]
        public void DecreaseHeadScale_最小値で実行_最小値のまま()
        {
            // Arrange
            _character.SetHeadScale(0.5f);

            // Act
            _character.DecreaseHeadScale();

            // Assert
            Assert.AreEqual(0.5f, _character.HeadScale.Value, 0.001f);
        }

        [Test]
        public void IncreaseFootScale_通常操作_10パーセント増加()
        {
            // Arrange
            var initialScale = 1.0f;
            _character.SetFootScale(initialScale);

            // Act
            _character.IncreaseFootScale();

            // Assert
            Assert.AreEqual(1.1f, _character.FootScale.Value, 0.001f);
        }

        [Test]
        public void DecreaseFootScale_通常操作_10パーセント減少()
        {
            // Arrange
            var initialScale = 1.0f;
            _character.SetFootScale(initialScale);

            // Act
            _character.DecreaseFootScale();

            // Assert
            Assert.AreEqual(0.9f, _character.FootScale.Value, 0.001f);
        }

        #endregion

        #region データ変換のテスト

        [Test]
        public void ToData_設定済みデータ_正しくデータオブジェクトに変換される()
        {
            // Arrange
            _character.SetRightHandColor(Color.red);
            _character.SetLeftHandColor(Color.green);
            _character.SetRightFootColor(Color.blue);
            _character.SetLeftFootColor(Color.yellow);
            _character.SetHeadScale(1.2f);
            _character.SetFootScale(0.8f);

            // Act
            var data = _character.ToData();

            // Assert
            Assert.AreEqual(Color.red, data.RightHandColor);
            Assert.AreEqual(Color.green, data.LeftHandColor);
            Assert.AreEqual(Color.blue, data.RightFootColor);
            Assert.AreEqual(Color.yellow, data.LeftFootColor);
            Assert.AreEqual(1.2f, data.HeadScale, 0.001f);
            Assert.AreEqual(0.8f, data.FootScale, 0.001f);
        }

        [Test]
        public void FromData_有効なデータ_正しく設定される()
        {
            // Arrange
            var data = new CharacterData
            {
                RightHandColor = Color.cyan,
                LeftHandColor = Color.magenta,
                RightFootColor = Color.black,
                LeftFootColor = Color.gray,
                HeadScale = 1.3f,
                FootScale = 0.7f
            };

            // Act
            _character.FromData(data);

            // Assert
            Assert.AreEqual(Color.cyan, _character.RightHandColor.Value);
            Assert.AreEqual(Color.magenta, _character.LeftHandColor.Value);
            Assert.AreEqual(Color.black, _character.RightFootColor.Value);
            Assert.AreEqual(Color.gray, _character.LeftFootColor.Value);
            Assert.AreEqual(1.3f, _character.HeadScale.Value, 0.001f);
            Assert.AreEqual(0.7f, _character.FootScale.Value, 0.001f);
        }

        #endregion

        #region リセット機能のテスト

        [Test]
        public void ResetToDefault_カスタマイズ済み_デフォルト値に戻る()
        {
            // Arrange
            _character.SetRightHandColor(Color.red);
            _character.SetLeftHandColor(Color.green);
            _character.SetRightFootColor(Color.blue);
            _character.SetLeftFootColor(Color.yellow);
            _character.SetHeadScale(1.5f);
            _character.SetFootScale(0.5f);

            // Act
            _character.ResetToDefault();

            // Assert
            Assert.AreEqual(Color.white, _character.RightHandColor.Value);
            Assert.AreEqual(Color.white, _character.LeftHandColor.Value);
            Assert.AreEqual(Color.white, _character.RightFootColor.Value);
            Assert.AreEqual(Color.white, _character.LeftFootColor.Value);
            Assert.AreEqual(1.0f, _character.HeadScale.Value, 0.001f);
            Assert.AreEqual(1.0f, _character.FootScale.Value, 0.001f);
        }

        #endregion

        #region ReactivePropertyの動作テスト

        [UnityTest]
        public IEnumerator OnRightHandColorChanged_色変更_イベント発火される()
        {
            // Arrange
            var eventFired = false;
            var receivedColor = Color.clear;
            _character.OnRightHandColorChanged.Subscribe(color =>
            {
                eventFired = true;
                receivedColor = color;
            });

            // Act
            _character.SetRightHandColor(Color.red);

            // Wait for one frame
            yield return null;

            // Assert
            Assert.IsTrue(eventFired);
            Assert.AreEqual(Color.red, receivedColor);
        }

        [UnityTest]
        public IEnumerator OnHeadScaleChanged_スケール変更_イベント発火される()
        {
            // Arrange
            var eventFired = false;
            var receivedScale = 0f;
            _character.OnHeadScaleChanged.Subscribe(scale =>
            {
                eventFired = true;
                receivedScale = scale;
            });

            // Act
            _character.SetHeadScale(1.2f);

            // Wait for one frame
            yield return null;

            // Assert
            Assert.IsTrue(eventFired);
            Assert.AreEqual(1.2f, receivedScale, 0.001f);
        }

        [UnityTest]
        public IEnumerator OnFootScaleChanged_IncreaseFootScale実行_イベント発火される()
        {
            // Arrange
            var eventCount = 0;
            // 最初にsubscribeした時点で1度呼ばれる、その後、変更しても呼ばれる
            // よってeventCountは2になることに注意
            _character.OnFootScaleChanged.Subscribe(_ =>
            {
                
                eventCount++;
                Debug.Log("called: count:"+eventCount);
            });

            // Act
            _character.IncreaseFootScale();

            // Wait for one frame
            yield return null;

            // Assert
            Assert.AreEqual(2, eventCount);
        }

        #endregion
    }
} 