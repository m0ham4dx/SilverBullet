using System;
using System.Collections.Generic;

namespace Extreme.Net
{
    /// <summary>
    /// Представляет коллекцию строк, представляющих параметры запроса.
    /// </summary>
    public class RequestParams : List<KeyValuePair<string,string>>
    {
        public readonly bool ValuesUnescaped;

        public readonly bool KeysUnescaped;

        public RequestParams(bool valuesUnescaped = false, bool keysUnescaped = false)
        {
            ValuesUnescaped = valuesUnescaped;
            KeysUnescaped = keysUnescaped;
        }

        public RequestParams(Dictionary<string, string> values, bool valuesUnescaped = false, bool keysUnescaped = false) : this(valuesUnescaped, keysUnescaped)
        {
            foreach (KeyValuePair<string, string> keyValuePair in values)
            {
                this[keyValuePair.Key] = keyValuePair.Value;
            }
        }


        public string Query
        {
            get
            {
                return Http.ToQueryString(this, this.ValuesUnescaped, this.KeysUnescaped);
            }
        }

        /// <summary>
        /// Задаёт новый параметр запроса.
        /// </summary>
        /// <param name="paramName">Название параметра запроса.</param>
        /// <exception cref="System.ArgumentNullException">Значение параметра <paramref name="paramName"/> равно <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Значение параметра <paramref name="paramName"/> является пустой строкой.</exception>
        public object this[string paramName]
        {
            set
            {
                #region Проверка параметра

                if (paramName == null)
                {
                    throw new ArgumentNullException("paramName");
                }

                if (paramName.Length == 0)
                {
                    throw ExceptionHelper.EmptyString("paramName");
                }

                #endregion

                string str = (value == null ? string.Empty : value.ToString());

                Add(new KeyValuePair<string, string>(paramName, str));
            }
        }
    }
}
