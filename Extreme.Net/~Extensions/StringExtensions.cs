using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extreme.Extensions
{
    /// <inheritdoc />
    /// <summary>
    /// Исключение говорящее о том что не удалось найти одну или несколько подстрок между двумя подстроками.
    /// </summary>
    public class SubstringException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Исключение говорящее о том что не удалось найти одну или несколько подстрок между двумя подстроками.
        /// </summary>
        public SubstringException() { }

        /// <inheritdoc />
        /// <inheritdoc cref="SubstringException()"/>        
        public SubstringException(string message) : base(message) { }

        /// <inheritdoc />
        /// <inheritdoc cref="SubstringException()"/>
        public SubstringException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Этот класс является расширением для строк. Не нужно его вызывать напрямую.
    /// </summary>
    public static class StringExtensions
    {
        #region Substrings: Несколько строк

        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, вернет пустой массив.
        /// </summary>
        /// <param name="self">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparison">Метод сравнения строк</param>
        /// <param name="limit">Максимальное число подстрок для поиска</param>
        /// <exception cref="ArgumentNullException">Возникает если один из параметров пустая строка или <keyword>null</keyword>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Возникает если начальный индекс превышает длину строки.</exception>
        /// <returns>Возвращает массив подстрок которые попадают под шаблон или пустой массив если нет совпадений.</returns>
        public static string[] SubstringsOrEmpty(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            #region Проверка параметров
            if (string.IsNullOrEmpty(self))
                return new string[0];

            if (string.IsNullOrEmpty(left))
                throw new ArgumentNullException(nameof(left));

            if (string.IsNullOrEmpty(right))
                throw new ArgumentNullException(nameof(right));

            if (startIndex < 0 || startIndex >= self.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            #endregion

            int currentStartIndex = startIndex;
            int current = limit;
            var strings = new List<string>();

            while (true)
            {
                if (limit > 0)
                {
                    --current;
                    if (current < 0)
                        break;
                }

                // Ищем начало позиции левой подстроки.
                int leftPosBegin = self.IndexOf(left, currentStartIndex, comparison);
                if (leftPosBegin == -1)
                    break;

                // Вычисляем конец позиции левой подстроки.
                int leftPosEnd = leftPosBegin + left.Length;
                // Ищем начало позиции правой строки.
                int rightPos = self.IndexOf(right, leftPosEnd, comparison);
                if (rightPos == -1)
                    break;

                // Вычисляем длину найденной подстроки.
                int length = rightPos - leftPosEnd;
                strings.Add(self.Substring(leftPosEnd, length));
                // Вычисляем конец позиции правой подстроки.
                currentStartIndex = rightPos + right.Length;
            }

            return strings.ToArray();
        }


        /// <inheritdoc cref="SubstringsOrEmpty"/>
        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, вернет <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.        
        /// </remarks>
        /// <example>
        /// str.Substrings("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="SubstringsEx"/> - которая и так бросает исключение <see cref="SubstringException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        /// <param name="fallback">Значение в случае если подстроки не найдены</param>
        /// <returns>Возвращает массив подстрок которые попадают под шаблон или <keyword>null</keyword>.</returns>
        public static string[] Substrings(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0, string[] fallback = null)
        {
            var result = SubstringsOrEmpty(self, left, right, startIndex, comparison, limit);

            return result.Length > 0 ? result : fallback;
        }


        /// <inheritdoc cref="SubstringsOrEmpty"/>
        /// <summary>
        /// Вырезает несколько строк между двумя подстроками. Если совпадений нет, будет брошено исключение <see cref="SubstringException"/>.
        /// </summary>
        /// <exception cref="SubstringException">Будет брошено если совпадений не было найдено</exception>
        /// <returns>Возвращает массив подстрок которые попадают под шаблон или бросает исключение <see cref="SubstringException"/> если совпадений не было найдено.</returns>
        public static string[] SubstringsEx(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            var result = SubstringsOrEmpty(self, left, right, startIndex, comparison, limit);
            if (result.Length == 0)
                throw new SubstringException($"Substrings not found. Left: \"{left}\". Right: \"{right}\".");

            return result;
        }

        #endregion


        #region Substring: Одна подстрока. Прямой порядок (слева направо)

        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, вернет <paramref name="fallback"/> или по-умолчанию <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.</remarks>
        /// <example>
        /// str.Between("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="SubstringEx"/> - которая и так бросает исключение <see cref="SubstringException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        /// <param name="self">Строка где следует искать подстроки</param>
        /// <param name="left">Начальная подстрока</param>
        /// <param name="right">Конечная подстрока</param>
        /// <param name="startIndex">Искать начиная с индекса</param>
        /// <param name="comparison">Метод сравнения строк</param>
        /// <param name="fallback">Значение в случае если подстрока не найдена</param>
        /// <returns>Возвращает строку между двумя подстроками или <paramref name="fallback"/> (по-умолчанию <keyword>null</keyword>).</returns>
        public static string Substring(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string fallback = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) ||
                startIndex < 0 || startIndex >= self.Length)
                return fallback;

            // Ищем начало позиции левой подстроки.
            int leftPosBegin = self.IndexOf(left, startIndex, comparison);
            if (leftPosBegin == -1)
                return fallback;

            // Вычисляем конец позиции левой подстроки.
            int leftPosEnd = leftPosBegin + left.Length;
            // Ищем начало позиции правой строки.
            int rightPos = self.IndexOf(right, leftPosEnd, comparison);

            return rightPos != -1 ? self.Substring(leftPosEnd, rightPos - leftPosEnd) : fallback;
        }


        /// <inheritdoc cref="Substring"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, вернет пустую строку.
        /// </summary>
        /// <returns>Возвращает строку между двумя подстроками. Если совпадений нет, вернет пустую строку.</returns>
        public static string SubstringOrEmpty(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return Substring(self, left, right, startIndex, comparison, string.Empty);
        }

        /// <inheritdoc cref="Substring"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками. Если совпадений нет, будет брошено исключение <see cref="SubstringException"/>.
        /// </summary>
        /// <exception cref="SubstringException">Будет брошено если совпадений не было найдено</exception>
        /// <returns>Возвращает строку между двумя подстроками или бросает исключение <see cref="SubstringException"/> если совпадений не было найдено.</returns>
        public static string SubstringEx(this string self, string left, string right,
            int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return Substring(self, left, right, startIndex, comparison)
                ?? throw new SubstringException($"Substring not found. Left: \"{left}\". Right: \"{right}\".");
        }


        #endregion


        #region Вырезание одной подстроки. Обратный порядок (справа налево)

        /// <inheritdoc cref="Substring"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, вернет <paramref name="notFoundValue"/> или по-умолчанию <keyword>null</keyword>.
        /// <remarks>
        /// Создана для удобства, для написания исключений через ?? тернарный оператор.</remarks>
        /// <example>
        /// str.BetweenLast("<tag>","</tag>") ?? throw new Exception("Не найдена строка");
        /// </example>
        /// 
        /// <remarks>
        /// Не стоит забывать о функции <see cref="SubstringLastEx"/> - которая и так бросает исключение <see cref="SubstringException"/> в случае если совпадения не будет.
        /// </remarks>
        /// </summary>
        public static string SubstringLast(this string self, string right, string left,
            int startIndex = -1, StringComparison comparison = StringComparison.Ordinal,
            string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) ||
                startIndex < -1 || startIndex >= self.Length)
                return notFoundValue;

            if (startIndex == -1)
                startIndex = self.Length - 1;

            // Ищем начало позиции правой подстроки с конца строки
            int rightPosBegin = self.LastIndexOf(right, startIndex, comparison);
            if (rightPosBegin == -1 || rightPosBegin == 0) // в обратном поиске имеет смысл проверять на 0
                return notFoundValue;

            // Вычисляем начало позиции левой подстроки
            int leftPosBegin = self.LastIndexOf(left, rightPosBegin - 1, comparison);
            // Если не найден левый конец или правая и левая подстрока склеены вместе - вернем пустую строку
            if (leftPosBegin == -1 || rightPosBegin - leftPosBegin == 1)
                return notFoundValue;

            int leftPosEnd = leftPosBegin + left.Length;
            return self.Substring(leftPosEnd, rightPosBegin - leftPosEnd);
        }


        /// <inheritdoc cref="SubstringOrEmpty"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, вернет пустую строку.
        /// </summary>
        public static string SubstringLastOrEmpty(this string self, string right, string left,
            int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
        {
            return SubstringLast(self, right, left, startIndex, comparison, string.Empty);
        }

        /// <inheritdoc cref="SubstringEx"/>
        /// <summary>
        /// Вырезает одну строку между двумя подстроками, только начиная поиск с конца. Если совпадений нет, будет брошено исключение <see cref="SubstringException"/>.
        /// </summary>
        public static string SubstringLastEx(this string self, string right, string left,
            int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
        {
            return SubstringLast(self, right, left, startIndex, comparison)
                ?? throw new SubstringException($"StringBetween not found. Right: \"{right}\". Left: \"{left}\".");
        }

        #endregion


        #region Дополнительные функции

        /// <summary>
        /// Проверяет наличие подстроки в строке, без учета реестра, через сравнение: <see cref="StringComparison.OrdinalIgnoreCase" />.
        /// </summary>
        /// <param name="self">Строка</param>
        /// <param name="value">Подстрока которую следует искать в исходной строке</param>
        /// <returns>Вернет <langword>true</langword> </returns>
        public static bool ContainsInsensitive(this string self, string value)
        {
            return self.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        #endregion

        public static string[] BetweensOrEmpty(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            if (string.IsNullOrEmpty(self))
            {
                return new string[0];
            }
            if (string.IsNullOrEmpty(left))
            {
                throw new ArgumentNullException("left");
            }
            if (string.IsNullOrEmpty(right))
            {
                throw new ArgumentNullException("right");
            }
            if (startIndex < 0 || startIndex >= self.Length)
            {
                throw new ArgumentOutOfRangeException("startIndex", "Wrong start index");
            }
            int startIndex2 = startIndex;
            int num = limit;
            List<string> list = new List<string>();
            for (; ; )
            {
                if (limit > 0)
                {
                    num--;
                    if (num < 0)
                    {
                        break;
                    }
                }
                int num2 = self.IndexOf(left, startIndex2, comparison);
                if (num2 == -1)
                {
                    break;
                }
                int num3 = num2 + left.Length;
                int num4 = self.IndexOf(right, num3, comparison);
                if (num4 == -1)
                {
                    break;
                }
                int length = num4 - num3;
                list.Add(self.Substring(num3, length));
                startIndex2 = num4 + right.Length;
            }
            return list.ToArray();
        }

        public static string[] Betweens(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            string[] array = self.BetweensOrEmpty(left, right, startIndex, comparison, limit);
            if (array.Length == 0)
            {
                return null;
            }
            return array;
        }

        public static string[] BetweensEx(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, int limit = 0)
        {
            string[] array = self.BetweensOrEmpty(left, right, startIndex, comparison, limit);
            if (array.Length == 0)
            {
                throw new SubstringException(string.Concat(new string[]
                {
                    "StringBetweens not found. Left: \"",
                    left,
                    "\". Right: \"",
                    right,
                    "\"."
                }));
            }
            return array;
        }

        public static string Between(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) || startIndex < 0 || startIndex >= self.Length)
            {
                return notFoundValue;
            }
            int num = self.IndexOf(left, startIndex, comparison);
            if (num == -1)
            {
                return notFoundValue;
            }
            int num2 = num + left.Length;
            int num3 = self.IndexOf(right, num2, comparison);
            if (num3 == -1)
            {
                return notFoundValue;
            }
            return self.Substring(num2, num3 - num2);
        }

        public static string BetweenOrEmpty(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return self.Between(left, right, startIndex, comparison, string.Empty);
        }

        public static string BetweenEx(this string self, string left, string right, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            string text = self.Between(left, right, startIndex, comparison, null);
            if (text == null)
            {
                throw new SubstringException(string.Concat(new string[]
                {
                    "StringBetween not found. Left: \"",
                    left,
                    "\". Right: \"",
                    right,
                    "\"."
                }));
            }
            return text;
        }

        public static string BetweenLast(this string self, string right, string left, int startIndex = -1, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(right) || string.IsNullOrEmpty(left) || startIndex < -1 || startIndex >= self.Length)
            {
                return notFoundValue;
            }
            if (startIndex == -1)
            {
                startIndex = self.Length - 1;
            }
            int num = self.LastIndexOf(right, startIndex, comparison);
            if (num == -1 || num == 0)
            {
                return notFoundValue;
            }
            int num2 = self.LastIndexOf(left, num - 1, comparison);
            if (num2 == -1 || num - num2 == 1)
            {
                return notFoundValue;
            }
            int num3 = num2 + left.Length;
            return self.Substring(num3, num - num3);
        }

        public static string BetweenLastOrEmpty(this string self, string right, string left, int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
        {
            return self.BetweenLast(right, left, startIndex, comparison, string.Empty);
        }

        public static string BetweenLastEx(this string self, string right, string left, int startIndex = -1, StringComparison comparison = StringComparison.Ordinal)
        {
            string text = self.BetweenLast(right, left, startIndex, comparison, null);
            if (text == null)
            {
                throw new SubstringException(string.Concat(new string[]
                {
                    "StringBetween not found. Right: \"",
                    right,
                    "\". Left: \"",
                    left,
                    "\"."
                }));
            }
            return text;
        }

        public static string EncodeJsonUnicode(this string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in value)
            {
                if (c <= '\u007f')
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("\\u");
                    StringBuilder stringBuilder2 = stringBuilder;
                    int num = (int)c;
                    stringBuilder2.Append(num.ToString("x4"));
                }
            }
            return stringBuilder.ToString();
        }

        public static string DecodeJsonUnicode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return Regex.Replace(value, "\\\\u([\\dA-Fa-f]{4})", (Match v) => ((char)Convert.ToInt32(v.Groups[1].Value, 16)).ToString());
        }

        public static string Win1251ToUTF8(this string source)
        {
            Encoding encoding = Encoding.GetEncoding("Windows-1251");
            byte[] bytes = encoding.GetBytes(source);
            byte[] bytes2 = Encoding.Convert(Encoding.UTF8, encoding, bytes);
            return encoding.GetString(bytes2);
        }

        public static string After(this string self, string input, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(input) || startIndex < 0 || startIndex >= self.Length)
            {
                return notFoundValue;
            }
            int num = self.IndexOf(input, startIndex, comparison);
            if (num == -1)
            {
                return notFoundValue;
            }
            int num2 = num + input.Length;
            return self.Substring(num2, self.Length - num2);
        }

        public static string AfterOrEmpty(this string self, string input, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return self.After(input, startIndex, comparison, string.Empty);
        }

        public static string AfterEx(this string self, string input, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            string text = self.After(input, startIndex, comparison, null);
            if (text == null)
            {
                throw new SubstringException("string.After not found. Input: \"" + input + "\".");
            }
            return text;
        }

        public static string Before(this string self, string input, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
        {
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(input) || startIndex < 0 || startIndex >= self.Length)
            {
                return notFoundValue;
            }
            int num = self.IndexOf(input, startIndex, comparison);
            if (num != -1)
            {
                return self.Substring(0, num);
            }
            return notFoundValue;
        }

        public static string BeforeOrEmpty(this string self, string input, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return self.Before(input, startIndex, comparison, string.Empty);
        }

        public static string BeforeEx(this string self, string input, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            string text = self.Before(input, startIndex, comparison, null);
            if (text == null)
            {
                throw new SubstringException("string.Before not found. Input: \"" + input + "\".");
            }
            return text;
        }

        public static bool IsWebLink(this string self, bool trim = false)
        {
            string text = self;
            if (trim)
            {
                text = text.Trim();
            }
            return text.StartsWith("http://") || text.StartsWith("https://");
        }

        public static string NullOnEmpty(this string self)
        {
            if (!(self == string.Empty))
            {
                return self;
            }
            return null;
        }

        public static bool NullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        public static bool NotNullNotEmpty(this string self)
        {
            return !string.IsNullOrEmpty(self);
        }

        public static bool HasContent(this string self)
        {
            return !string.IsNullOrWhiteSpace(self);
        }

        public static bool ContainsIgnoreCase(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
        }

        public static bool Contains(this IReadOnlyList<string> self, string value, StringComparison comparison = StringComparison.Ordinal)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (self[i].Equals(value, comparison))
                {
                    return true;
                }
            }
            return false;
        }

        public static string ToUpperFirst(this string s, bool useToLower = true)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char c = char.ToUpper(s[0]);
            string text = s.Substring(1);
            if (useToLower)
            {
                text = text.ToLower();
            }
            return c.ToString() + text;
        }

        public static string GetJsonValue(this string json, string key, string endsWith = ",\"")
        {
            if (!(endsWith != "\""))
            {
                return json.Between("\"" + key + "\":\"", "\"", 0, StringComparison.Ordinal, null);
            }
            string text = json.Between("\"" + key + "\":", endsWith, 0, StringComparison.Ordinal, null);
            if (text == null)
            {
                return null;
            }
            return text.Trim(new char[]
            {
                '"',
                '\r',
                '\n',
                '\t'
            });
        }

        public static string GetJsonValueEx(this string json, string key, string ending = ",\"")
        {
            string jsonValue = json.GetJsonValue(key, ending);
            if (jsonValue == null)
            {
                throw new SubstringException("Не найдено значение JSON ключа \"" + key + "\". Ответ: " + json);
            }
            return jsonValue;
        }

        public static byte[] HexStringToBytes(this string hexString)
        {
            int length = hexString.Length;
            byte[] array = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                array[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return array;
        }

        public static string EscapeJsonData(this string jsonData, bool escapeUnicode)
        {
            string text = jsonData.Replace("\\", "\\\\").Replace("\"", "\\\"");
            if (escapeUnicode)
            {
                text = text.EncodeJsonUnicode();
            }
            return text;
        }

        // (get) Token: 0x0600006E RID: 110 RVA: 0x00002EA8 File Offset: 0x000010A8
        public static NumberFormatInfo ThousandNumberFormatInfo
        {
            get
            {
                if (StringExtensions._thousandNumberFormatInfo == null)
                {
                    StringExtensions._thousandNumberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
                    StringExtensions._thousandNumberFormatInfo.NumberGroupSeparator = " ";
                }
                return StringExtensions._thousandNumberFormatInfo;
            }
        }

        public static string InnerHtmlByClass(this string self, string className, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
        {
            string result = notFoundValue;
            StringExtensions.GetInnerHtmlByClass(self, className, ref result, startIndex, comparison);
            return result;
        }

        public static string InnerHtmlByAttribute(this string self, string attribute, string value, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal, string notFoundValue = null)
        {
            string result = notFoundValue;
            StringExtensions.GetInnerHtmlByAttribute(self, attribute, value, ref result, startIndex, comparison);
            return result;
        }

        public static string[] InnerHtmlByClassAll(this string self, string className, int startIndex = 0, bool trim = false, StringComparison comparison = StringComparison.Ordinal)
        {
            int startIndex2 = startIndex;
            string text = string.Empty;
            List<string> list = new List<string>();
            while ((startIndex2 = StringExtensions.GetInnerHtmlByClass(self, className, ref text, startIndex2, comparison)) != -1)
            {
                if (trim)
                {
                    text = text.Trim();
                }
                list.Add(text);
                text = string.Empty;
            }
            return list.ToArray();
        }

        private static bool HasSubstring(this string self, string left, string right, out string substring, out int beginSubstringIndex, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            substring = null;
            beginSubstringIndex = -1;
            if (string.IsNullOrEmpty(self) || string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right) || startIndex < 0 || startIndex >= self.Length)
            {
                return false;
            }
            int num = self.IndexOf(left, startIndex, comparison);
            if (num == -1)
            {
                return false;
            }
            int num2 = num + left.Length;
            int num3 = self.IndexOf(right, num2, comparison);
            if (num3 == -1)
            {
                return false;
            }
            substring = self.Substring(num2, num3 - num2);
            beginSubstringIndex = num;
            return true;
        }

        private static int GetInnerHtmlByAttribute(string self, string attribute, string value, ref string result, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            string text;
            int num;
            while (self.HasSubstring(attribute + "=\"", "\"", out text, out num, startIndex, comparison))
            {
                startIndex += attribute.Length + value.Length + 1;
                text = text.Trim();
                if (!(value == string.Empty) && text.Split(new char[]
                {
                    '\n',
                    '\t',
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries).Contains(value, comparison))
                {
                    int num2 = self.LastIndexOf("<", num - 1, StringComparison.Ordinal);
                    if (num2 != -1)
                    {
                        string text2 = self.Substring(num2 + 1, num - num2 - 2);
                        text2 = text2.Split(new char[]
                        {
                            ' ',
                            '\t',
                            '\n'
                        }, 2)[0];
                        if (!string.IsNullOrEmpty(text2))
                        {
                            int length = text2.Length;
                            int num3 = self.IndexOf(">", num + attribute.Length + value.Length + 1, StringComparison.InvariantCulture);
                            if (num3 != -1)
                            {
                                num3++;
                                int num4 = 0;
                                int num5 = self.Length - 1;
                                for (int i = num3; i < num5; i++)
                                {
                                    if (self[i] == '<')
                                    {
                                        int num6 = i + 1;
                                        int num7;
                                        if (self[num6] == '/')
                                        {
                                            num6++;
                                            num7 = -1;
                                        }
                                        else
                                        {
                                            num7 = 1;
                                        }
                                        if (num6 + length > num5)
                                        {
                                            break;
                                        }
                                        if (self[num6] == text2[0] && !(text2 != self.Substring(num6, length)))
                                        {
                                            num4 += num7;
                                            if (num4 == -1)
                                            {
                                                result = self.Substring(num3, i - num3);
                                                int num8 = num + result.Length;
                                                if (num8 < self.Length)
                                                {
                                                    return num8;
                                                }
                                                return -1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return -1;
        }

        private static int GetInnerHtmlByClass(string self, string className, ref string result, int startIndex = 0, StringComparison comparison = StringComparison.Ordinal)
        {
            return StringExtensions.GetInnerHtmlByAttribute(self, "class", className, ref result, startIndex, comparison);
        }

        public const string HttpProto = "http://";

        public const string HttpsProto = "https://";

        private static NumberFormatInfo _thousandNumberFormatInfo;
    }
}
