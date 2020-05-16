using NUnit.Framework;
using static HannaHandCipher.DigitSeriesOperations;

namespace HannaHandCipher.Test
{
    public class UnitTests
    {
        [Test]
        public void RelativeAlphabeticalIndexEncodeTest()
        {
            // Arrange
            string splitComponentAOne = "IHRNAHTEUC";
            string splitComponentATwo = "HWIEDER";
            string expectedSplitComponentAOneDigitSeries = "6487159302";
            string expectedSplitComponentATwoDigitSeries = "4752136";
            
            // Act
            string actualOne = RelativeAlphabeticalIndexEncode(splitComponentAOne);
            string actualTwo = RelativeAlphabeticalIndexEncode(splitComponentATwo);
            
            // Assert
            Assert.AreEqual(expectedSplitComponentAOneDigitSeries, actualOne);
            Assert.AreEqual(expectedSplitComponentATwoDigitSeries, actualTwo);
        }

        [Test]
        public void ResizeUsingLaggedFibonacciGeneratorTest()
        {
            // Arrange
            string componentC = "36951";
            int keyMatrixEncodedComponentBLength = 31;
            string expectedComponentCDigitSeries = "3695195460490643960725679713668";
            
            // Act
            string actual = ResizeUsingLaggedFibonacciGenerator(componentC, keyMatrixEncodedComponentBLength);
            
            // Assert
            Assert.AreEqual(expectedComponentCDigitSeries, actual);
        }

        [Test]
        public void IndividualDigitsMod10CalculationAdditionTest()
        {
            // Arrange
            string componentBDigitSeries = "4544414134662384677444147696860";
            string componentCDigitSeries = "3695195460490643960725679713668";
            string expectedComponentDDigitSeries = "7139509594052927537169716309428";

            // Act
            string actual = IndividualDigitsModulus10Calculation(componentBDigitSeries, componentCDigitSeries, IndividualDigitsModulus10Operation.Add);
            
            // Assert
            Assert.AreEqual(expectedComponentDDigitSeries, actual);
        }
        
        [Test]
        public void IndividualDigitsMod10CalculationSubtractionTest()
        {
            // Arrange
            string componentBDigitSeriesExpected = "4544414134662384677444147696860";
            string componentCDigitSeries = "3695195460490643960725679713668";
            string componentDDigitSeries = "7139509594052927537169716309428";

            // Act
            string actual = IndividualDigitsModulus10Calculation(componentDDigitSeries, componentCDigitSeries, IndividualDigitsModulus10Operation.Subtract);
            
            // Assert
            Assert.AreEqual(componentBDigitSeriesExpected, actual);
        }

        [Test]
        public void ColumnarTranspositionTest()
        {
            // Arrange
            string digitSeriesToTranspose =
                "3054012499334469997458746031473866033485815257003057786633036791330482323368962152940";
            
            string firstTranspositionKey = "1394636";
            string secondTranspositionKey = "7353";
            
            string firstTranspositionExpected = 
                "3468435031360099773253322144368067889439688773035037065086462245101369294599434570331";
            string secondTranspositionExpected = 
                "4310731674830662095308069224089737065399536539334883803544129433430752369675082164471";

            
            // Act
            string firstTranspostionActual = ColumnarTransposition(digitSeriesToTranspose, firstTranspositionKey);
            string secondTranspostionActual = ColumnarTransposition(firstTranspostionActual, secondTranspositionKey);

            // Assert
            Assert.AreEqual(firstTranspositionExpected, firstTranspostionActual);
            Assert.AreEqual(secondTranspositionExpected, secondTranspostionActual);
        }
        [Test]
        public void ReverseColumnarTranspositionTest()
        {
            // Arrange
            string digitSeriesToTranspose =
                "4310731674830662095308069224089737065399536539334883803544129433430752369675082164471";
            
            string firstTranspositionKey = "7353";
            string secondTranspositionKey = "1394636";
            
            string firstTranspositionExpected = 
                "3468435031360099773253322144368067889439688773035037065086462245101369294599434570331";
            string secondTranspositionExpected = 
                "3054012499334469997458746031473866033485815257003057786633036791330482323368962152940";

            
            // Act
            string firstTranspostionActual = ReverseColumnarTransposition(digitSeriesToTranspose, firstTranspositionKey);
            string secondTranspostionActual = ReverseColumnarTransposition(firstTranspostionActual, secondTranspositionKey);

            // Assert
            Assert.AreEqual(firstTranspositionExpected, firstTranspostionActual);
            Assert.AreEqual(secondTranspositionExpected, secondTranspostionActual);
        }

        [Test]
        public void GetSerialInsertPositionFromComponentATest()
        {
            // Arrange
            string componentA = "IHR NAHT EUCH WIEDER";
            
            // Act
            int insertPositionActual = GetSerialInsertPositionFromComponentA(componentA);
            
            // Assert

            Assert.AreEqual(11, insertPositionActual);
        }

        [Test]
        public void KeyMatrixDigitKeysToCharValuesTest()
        {
            // Arrange
            string tenColumnDigitKeys = "6487159302";
            string threeRowDigitKeys = "475";
            var keyMatrix = new StraddlingCheckerboardKeyMatrix(tenColumnDigitKeys, threeRowDigitKeys);
            string encodedPlainText = "4664540678816996064506846774441844419776886984581679178640816714278845444114764642160";
            string expectedPlainText = "BEIMERSTELLENEINESBUCHSCHLUESSELSISTEXTREMSTEVORSICHTGEBOTEN";
            
            // Act
            string actualPlainText = keyMatrix.DigitKeysToCharValues(encodedPlainText);
            
            // Assert
            Assert.AreEqual(expectedPlainText, actualPlainText);

        }
        
        [Test]
        public void KeyMatrixCharValuesToDigitKeysTest()
        {
            // Arrange
            string tenColumnDigitKeys = "6487159302";
            string threeRowDigitKeys = "475";
            var keyMatrix = new StraddlingCheckerboardKeyMatrix(tenColumnDigitKeys, threeRowDigitKeys);
            string expectedEncodedPlainText = "4664540678816996064506846774441844419776886984581679178640816714278845444114764642160";
            string plainText = "BEIMERSTELLENEINESBUCHSCHLUESSELSISTEXTREMSTEVORSICHTGEBOTEN";
            
            // Act
            string actualEncodedPlainText = StraddlingCheckerboardKeyMatrix.CharValuesToDigitKeys(plainText);
            
            // Assert
            Assert.AreEqual(expectedEncodedPlainText, actualEncodedPlainText);

        }
    }
}