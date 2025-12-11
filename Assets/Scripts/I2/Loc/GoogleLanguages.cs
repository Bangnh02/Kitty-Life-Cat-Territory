using System;
using System.Collections.Generic;
using UnityEngine;

namespace I2.Loc
{
	public static class GoogleLanguages
	{
		public struct LanguageCodeDef
		{
			public string Code;

			public string GoogleCode;

			public bool HasJoinedWords;

			public int PluralRule;
		}

		public static Dictionary<string, LanguageCodeDef> mLanguageDef;

		public static string GetLanguageCode(string Filter, bool ShowWarnings = false)
		{
			if (string.IsNullOrEmpty(Filter))
			{
				return string.Empty;
			}
			string[] filters = Filter.ToLowerInvariant().Split(" /(),".ToCharArray());
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (LanguageMatchesFilter(item.Key, filters))
				{
					return item.Value.Code;
				}
			}
			if (ShowWarnings)
			{
				UnityEngine.Debug.Log($"Language '{Filter}' not recognized. Please, add the language code to GoogleTranslation.cs");
			}
			return string.Empty;
		}

		public static List<string> GetLanguagesForDropdown(string Filter, string CodesToExclude)
		{
			string[] filters = Filter.ToLowerInvariant().Split(" /(),".ToCharArray());
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (string.IsNullOrEmpty(Filter) || LanguageMatchesFilter(item.Key, filters))
				{
					string text = string.Concat("[" + item.Value.Code + "]");
					if (!CodesToExclude.Contains(text))
					{
						list.Add(item.Key + " " + text);
					}
				}
			}
			for (int num = list.Count - 2; num >= 0; num--)
			{
				string text2 = list[num].Substring(0, list[num].IndexOf(" ["));
				if (list[num + 1].StartsWith(text2))
				{
					list[num] = text2 + "/" + list[num];
					list.Insert(num + 1, text2 + "/");
				}
			}
			return list;
		}

		private static bool LanguageMatchesFilter(string Language, string[] Filters)
		{
			Language = Language.ToLowerInvariant();
			int i = 0;
			for (int num = Filters.Length; i < num; i++)
			{
				if (Filters[i] != "")
				{
					if (!Language.Contains(Filters[i].ToLower()))
					{
						return false;
					}
					Language = Language.Remove(Language.IndexOf(Filters[i]), Filters[i].Length);
				}
			}
			return true;
		}

		public static string GetFormatedLanguageName(string Language)
		{
			string empty = string.Empty;
			int num = Language.IndexOf(" [");
			if (num > 0)
			{
				Language = Language.Substring(0, num);
			}
			num = Language.IndexOf('/');
			if (num > 0)
			{
				empty = Language.Substring(0, num);
				if (Language == empty + "/" + empty)
				{
					return empty;
				}
				Language = Language.Replace("/", " (") + ")";
			}
			return Language;
		}

		public static string GetCodedLanguage(string Language, string code)
		{
			string languageCode = GetLanguageCode(Language);
			if (string.Compare(code, languageCode, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Language;
			}
			return Language + " [" + code + "]";
		}

		public static void UnPackCodeFromLanguageName(string CodedLanguage, out string Language, out string code)
		{
			if (string.IsNullOrEmpty(CodedLanguage))
			{
				Language = string.Empty;
				code = string.Empty;
				return;
			}
			int num = CodedLanguage.IndexOf("[");
			if (num < 0)
			{
				Language = CodedLanguage;
				code = GetLanguageCode(Language);
			}
			else
			{
				Language = CodedLanguage.Substring(0, num).Trim();
				code = CodedLanguage.Substring(num + 1, CodedLanguage.IndexOf("]", num) - num - 1);
			}
		}

		public static string GetGoogleLanguageCode(string InternationalCode)
		{
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (InternationalCode == item.Value.Code)
				{
					if (item.Value.GoogleCode == "-")
					{
						return null;
					}
					return (!string.IsNullOrEmpty(item.Value.GoogleCode)) ? item.Value.GoogleCode : InternationalCode;
				}
			}
			return InternationalCode;
		}

		public static string GetLanguageName(string code, bool useParenthesesForRegion = false, bool allowDiscardRegion = true)
		{
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (code == item.Value.Code)
				{
					string text = item.Key;
					if (useParenthesesForRegion)
					{
						int num = text.IndexOf('/');
						if (num > 0)
						{
							text = text.Substring(0, num) + " (" + text.Substring(num + 1) + ")";
						}
					}
					return text;
				}
			}
			if (allowDiscardRegion)
			{
				int num2 = code.IndexOf("-");
				if (num2 > 0)
				{
					return GetLanguageName(code.Substring(0, num2), useParenthesesForRegion, allowDiscardRegion: false);
				}
			}
			return null;
		}

		public static List<string> GetAllInternationalCodes()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				hashSet.Add(item.Value.Code);
			}
			return new List<string>(hashSet);
		}

		public static bool LanguageCode_HasJoinedWord(string languageCode)
		{
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (languageCode == item.Value.GoogleCode || languageCode == item.Value.Code)
				{
					return item.Value.HasJoinedWords;
				}
			}
			return false;
		}

		private static int GetPluralRule(string langCode)
		{
			if (langCode.Length > 2)
			{
				langCode = langCode.Substring(0, 2);
			}
			langCode = langCode.ToLower();
			foreach (KeyValuePair<string, LanguageCodeDef> item in mLanguageDef)
			{
				if (item.Value.Code == langCode)
				{
					return item.Value.PluralRule;
				}
			}
			return 0;
		}

		public static bool LanguageHasPluralType(string langCode, string pluralType)
		{
			if (pluralType == "Plural" || pluralType == "Zero" || pluralType == "One")
			{
				return true;
			}
			switch (GetPluralRule(langCode))
			{
			case 3:
				if (!(pluralType == "Two"))
				{
					return pluralType == "Few";
				}
				return true;
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
				return pluralType == "Few";
			case 9:
				if (!(pluralType == "Two"))
				{
					return pluralType == "Few";
				}
				return true;
			case 10:
			case 11:
			case 15:
			case 16:
				if (!(pluralType == "Two") && !(pluralType == "Few"))
				{
					return pluralType == "Many";
				}
				return true;
			case 12:
				if (!(pluralType == "Few"))
				{
					return pluralType == "Many";
				}
				return true;
			case 13:
				return pluralType == "Two";
			default:
				return false;
			}
		}

		public static ePluralType GetPluralType(string langCode, int n)
		{
			switch (n)
			{
			case 0:
				return ePluralType.Zero;
			case 1:
				return ePluralType.One;
			default:
				switch (GetPluralRule(langCode))
				{
				case 0:
					return ePluralType.Plural;
				case 1:
					if (n != 1)
					{
						return ePluralType.Plural;
					}
					return ePluralType.One;
				case 2:
					if (n > 1)
					{
						return ePluralType.Plural;
					}
					return ePluralType.One;
				case 3:
					switch (n)
					{
					default:
						if (!inRange(n, 3, 10) && !inRange(n, 13, 19))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					case 2:
					case 12:
						return ePluralType.Two;
					case 1:
					case 11:
						return ePluralType.One;
					}
				case 4:
					if (n != 1)
					{
						if (!inRange(n % 100, 1, 19))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 5:
					if (n % 10 != 1 || n % 100 == 11)
					{
						if (n % 10 < 2 || (n % 100 >= 10 && n % 100 < 20))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 6:
					if (n % 10 != 1 || n % 100 == 11)
					{
						if (!inRange(n % 10, 2, 4) || inRange(n % 100, 12, 14))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 7:
					if (n != 1)
					{
						if (!inRange(n, 2, 4))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 8:
					if (n != 1)
					{
						if (!inRange(n % 10, 2, 4) || inRange(n % 100, 12, 14))
						{
							return ePluralType.Plural;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 9:
					if (n % 100 != 1)
					{
						if (n % 100 != 2)
						{
							if (!inRange(n % 100, 3, 4))
							{
								return ePluralType.Plural;
							}
							return ePluralType.Few;
						}
						return ePluralType.Two;
					}
					return ePluralType.One;
				case 10:
					switch (n)
					{
					default:
						if (!inRange(n, 3, 6))
						{
							if (!inRange(n, 7, 10))
							{
								return ePluralType.Plural;
							}
							return ePluralType.Many;
						}
						return ePluralType.Few;
					case 2:
						return ePluralType.Two;
					case 1:
						return ePluralType.One;
					}
				case 11:
					switch (n)
					{
					default:
						if (!inRange(n % 100, 3, 10))
						{
							if (n % 100 < 11)
							{
								return ePluralType.Plural;
							}
							return ePluralType.Many;
						}
						return ePluralType.Few;
					case 2:
						return ePluralType.Two;
					case 1:
						return ePluralType.One;
					case 0:
						return ePluralType.Zero;
					}
				case 12:
					if (n != 1)
					{
						if (!inRange(n % 100, 1, 10))
						{
							if (!inRange(n % 100, 11, 19))
							{
								return ePluralType.Plural;
							}
							return ePluralType.Many;
						}
						return ePluralType.Few;
					}
					return ePluralType.One;
				case 13:
					if (n % 10 != 1)
					{
						if (n % 10 != 2)
						{
							return ePluralType.Plural;
						}
						return ePluralType.Two;
					}
					return ePluralType.One;
				case 14:
					if (n % 10 != 1 || n % 100 == 11)
					{
						return ePluralType.Plural;
					}
					return ePluralType.One;
				case 15:
					if (n % 10 != 1 || n % 100 == 11 || n % 100 == 71 || n % 100 == 91)
					{
						if (n % 10 != 2 || n % 100 == 12 || n % 100 == 72 || n % 100 == 92)
						{
							if ((n % 10 != 3 && n % 10 != 4 && n % 10 != 9) || n % 100 == 13 || n % 100 == 14 || n % 100 == 19 || n % 100 == 73 || n % 100 == 74 || n % 100 == 79 || n % 100 == 93 || n % 100 == 94 || n % 100 == 99)
							{
								if (n % 1000000 != 0)
								{
									return ePluralType.Plural;
								}
								return ePluralType.Many;
							}
							return ePluralType.Few;
						}
						return ePluralType.Two;
					}
					return ePluralType.One;
				case 16:
					switch (n)
					{
					default:
						return ePluralType.Plural;
					case 6:
						return ePluralType.Many;
					case 3:
						return ePluralType.Few;
					case 2:
						return ePluralType.Two;
					case 1:
						return ePluralType.One;
					case 0:
						return ePluralType.Zero;
					}
				default:
					return ePluralType.Plural;
				}
			}
		}

		public static int GetPluralTestNumber(string langCode, ePluralType pluralType)
		{
			switch (pluralType)
			{
			case ePluralType.Zero:
				return 0;
			case ePluralType.One:
				return 1;
			case ePluralType.Few:
				return 3;
			case ePluralType.Many:
				switch (GetPluralRule(langCode))
				{
				case 10:
					return 8;
				case 11:
				case 12:
					return 13;
				case 15:
					return 1000000;
				default:
					return 6;
				}
			default:
				return 936;
			}
		}

		private static bool inRange(int amount, int min, int max)
		{
			if (amount >= min)
			{
				return amount <= max;
			}
			return false;
		}

		static GoogleLanguages()
		{
			Dictionary<string, LanguageCodeDef> dictionary = new Dictionary<string, LanguageCodeDef>(StringComparer.Ordinal);
			LanguageCodeDef value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ab",
				GoogleCode = "-"
			};
			dictionary.Add("Abkhazian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "aa",
				GoogleCode = "-"
			};
			dictionary.Add("Afar", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "af"
			};
			dictionary.Add("Afrikaans", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ak",
				GoogleCode = "-"
			};
			dictionary.Add("Akan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sq"
			};
			dictionary.Add("Albanian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "am"
			};
			dictionary.Add("Amharic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar"
			};
			dictionary.Add("Arabic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-DZ",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Algeria", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-BH",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Bahrain", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-EG",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Egypt", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-IQ",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Iraq", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-JO",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Jordan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-KW",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Kuwait", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-LB",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Lebanon", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-LY",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Libya", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-MA",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Morocco", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-OM",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Oman", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-QA",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Qatar", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-SA",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Saudi Arabia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-SY",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Syria", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-TN",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Tunisia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-AE",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/U.A.E.", value);
			value = new LanguageCodeDef
			{
				PluralRule = 11,
				Code = "ar-YE",
				GoogleCode = "ar"
			};
			dictionary.Add("Arabic/Yemen", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "an",
				GoogleCode = "-"
			};
			dictionary.Add("Aragonese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "hy"
			};
			dictionary.Add("Armenian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "as",
				GoogleCode = "-"
			};
			dictionary.Add("Assamese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "av",
				GoogleCode = "-"
			};
			dictionary.Add("Avaric", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ae",
				GoogleCode = "-"
			};
			dictionary.Add("Avestan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ay",
				GoogleCode = "-"
			};
			dictionary.Add("Aymara", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "az"
			};
			dictionary.Add("Azerbaijani", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "bm",
				GoogleCode = "-"
			};
			dictionary.Add("Bambara", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ba",
				GoogleCode = "-"
			};
			dictionary.Add("Bashkir", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "eu"
			};
			dictionary.Add("Basque", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "eu-ES",
				GoogleCode = "eu"
			};
			dictionary.Add("Basque/Spain", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "be"
			};
			dictionary.Add("Belarusian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "bn"
			};
			dictionary.Add("Bengali", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "bh",
				GoogleCode = "-"
			};
			dictionary.Add("Bihari", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "bi",
				GoogleCode = "-"
			};
			dictionary.Add("Bislama", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "bs"
			};
			dictionary.Add("Bosnian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "br",
				GoogleCode = "-"
			};
			dictionary.Add("Breton", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "bg"
			};
			dictionary.Add("Bulgariaa", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "my"
			};
			dictionary.Add("Burmese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ca"
			};
			dictionary.Add("Catalan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ch",
				GoogleCode = "-"
			};
			dictionary.Add("Chamorro", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ce",
				GoogleCode = "-"
			};
			dictionary.Add("Chechen", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ny"
			};
			dictionary.Add("Chichewa", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh",
				GoogleCode = "zh-CN",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-HK",
				GoogleCode = "zh-TW",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/Hong Kong", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-MO",
				GoogleCode = "zh-CN",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/Macau", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-CN",
				GoogleCode = "zh-CN",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/PRC", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-CN",
				GoogleCode = "zh-CN",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/Simplified", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-SG",
				GoogleCode = "zh-CN",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/Singapore", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-TW",
				GoogleCode = "zh-TW",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/Taiwan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "zh-TW",
				GoogleCode = "zh-TW",
				HasJoinedWords = true
			};
			dictionary.Add("Chinese/Traditional", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "cv",
				GoogleCode = "-"
			};
			dictionary.Add("Chuvash", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kw",
				GoogleCode = "-"
			};
			dictionary.Add("Cornish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "co"
			};
			dictionary.Add("Corsican", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "cr",
				GoogleCode = "-"
			};
			dictionary.Add("Cree", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "hr"
			};
			dictionary.Add("Croatian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 5,
				Code = "hr-BA",
				GoogleCode = "hr"
			};
			dictionary.Add("Croatian/Bosnia and Herzegovina", value);
			value = new LanguageCodeDef
			{
				PluralRule = 7,
				Code = "cs"
			};
			dictionary.Add("Czech", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "da"
			};
			dictionary.Add("Danish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "dv",
				GoogleCode = "-"
			};
			dictionary.Add("Divehi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nl"
			};
			dictionary.Add("Dutch", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nl-BE",
				GoogleCode = "nl"
			};
			dictionary.Add("Dutch/Belgium", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nl-NL",
				GoogleCode = "nl"
			};
			dictionary.Add("Dutch/Netherlands", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "dz",
				GoogleCode = "-"
			};
			dictionary.Add("Dzongkha", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en"
			};
			dictionary.Add("English", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-AU",
				GoogleCode = "en"
			};
			dictionary.Add("English/Australia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-BZ",
				GoogleCode = "en"
			};
			dictionary.Add("English/Belize", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-CA",
				GoogleCode = "en"
			};
			dictionary.Add("English/Canada", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-CB",
				GoogleCode = "en"
			};
			dictionary.Add("English/Caribbean", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-IE",
				GoogleCode = "en"
			};
			dictionary.Add("English/Ireland", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-JM",
				GoogleCode = "en"
			};
			dictionary.Add("English/Jamaica", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-NZ",
				GoogleCode = "en"
			};
			dictionary.Add("English/New Zealand", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-PH",
				GoogleCode = "en"
			};
			dictionary.Add("English/Republic of the Philippines", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-ZA",
				GoogleCode = "en"
			};
			dictionary.Add("English/South Africa", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-TT",
				GoogleCode = "en"
			};
			dictionary.Add("English/Trinidad", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-GB",
				GoogleCode = "en"
			};
			dictionary.Add("English/United Kingdom", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-US",
				GoogleCode = "en"
			};
			dictionary.Add("English/United States", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "en-ZW",
				GoogleCode = "en"
			};
			dictionary.Add("English/Zimbabwe", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "eo"
			};
			dictionary.Add("Esperanto", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "et"
			};
			dictionary.Add("Estonian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ee",
				GoogleCode = "-"
			};
			dictionary.Add("Ewe", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "fo",
				GoogleCode = "-"
			};
			dictionary.Add("Faeroese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "fj",
				GoogleCode = "-"
			};
			dictionary.Add("Fijian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "fi"
			};
			dictionary.Add("Finnish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr"
			};
			dictionary.Add("French", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr-BE",
				GoogleCode = "fr"
			};
			dictionary.Add("French/Belgium", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr-CA",
				GoogleCode = "fr"
			};
			dictionary.Add("French/Canada", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr-FR",
				GoogleCode = "fr"
			};
			dictionary.Add("French/France", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr-LU",
				GoogleCode = "fr"
			};
			dictionary.Add("French/Luxembourg", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr-MC",
				GoogleCode = "fr"
			};
			dictionary.Add("French/Principality of Monaco", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "fr-CH",
				GoogleCode = "fr"
			};
			dictionary.Add("French/Switzerland", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ff",
				GoogleCode = "-"
			};
			dictionary.Add("Fulah", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "gl"
			};
			dictionary.Add("Galician", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "gl-ES",
				GoogleCode = "gl"
			};
			dictionary.Add("Galician/Spain", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "ka"
			};
			dictionary.Add("Georgian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "de"
			};
			dictionary.Add("German", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "de-AT",
				GoogleCode = "de"
			};
			dictionary.Add("German/Austria", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "de-DE",
				GoogleCode = "de"
			};
			dictionary.Add("German/Germany", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "de-LI",
				GoogleCode = "de"
			};
			dictionary.Add("German/Liechtenstein", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "de-LU",
				GoogleCode = "de"
			};
			dictionary.Add("German/Luxembourg", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "de-CH",
				GoogleCode = "de"
			};
			dictionary.Add("German/Switzerland", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "el"
			};
			dictionary.Add("Greek", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "gn",
				GoogleCode = "-"
			};
			dictionary.Add("Guaraní", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "gu"
			};
			dictionary.Add("Gujarati", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ht"
			};
			dictionary.Add("Haitian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ha"
			};
			dictionary.Add("Hausa", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "he",
				GoogleCode = "iw"
			};
			dictionary.Add("Hebrew", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "hz",
				GoogleCode = "-"
			};
			dictionary.Add("Herero", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "hi"
			};
			dictionary.Add("Hindi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ho",
				GoogleCode = "-"
			};
			dictionary.Add("Hiri Motu", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "hu"
			};
			dictionary.Add("Hungarian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ia",
				GoogleCode = "-"
			};
			dictionary.Add("Interlingua", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "id"
			};
			dictionary.Add("Indonesian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ie",
				GoogleCode = "-"
			};
			dictionary.Add("Interlingue", value);
			value = new LanguageCodeDef
			{
				PluralRule = 10,
				Code = "ga"
			};
			dictionary.Add("Irish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ig"
			};
			dictionary.Add("Igbo", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ik",
				GoogleCode = "-"
			};
			dictionary.Add("Inupiaq", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "io",
				GoogleCode = "-"
			};
			dictionary.Add("Ido", value);
			value = new LanguageCodeDef
			{
				PluralRule = 14,
				Code = "is"
			};
			dictionary.Add("Icelandic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "it"
			};
			dictionary.Add("Italian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "it-IT",
				GoogleCode = "it"
			};
			dictionary.Add("Italian/Italy", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "it-CH",
				GoogleCode = "it"
			};
			dictionary.Add("Italian/Switzerland", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "iu",
				GoogleCode = "-"
			};
			dictionary.Add("Inuktitut", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "ja",
				HasJoinedWords = true
			};
			dictionary.Add("Japanese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "jv"
			};
			dictionary.Add("Javanese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kl",
				GoogleCode = "-"
			};
			dictionary.Add("Kalaallisut", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kn"
			};
			dictionary.Add("Kannada", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kr",
				GoogleCode = "-"
			};
			dictionary.Add("Kanuri", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ks",
				GoogleCode = "-"
			};
			dictionary.Add("Kashmiri", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kk"
			};
			dictionary.Add("Kazakh", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "km"
			};
			dictionary.Add("Central Khmer", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ki",
				GoogleCode = "-"
			};
			dictionary.Add("Kikuyu", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "rw",
				GoogleCode = "-"
			};
			dictionary.Add("Kinyarwanda", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ky"
			};
			dictionary.Add("Kirghiz", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kv",
				GoogleCode = "-"
			};
			dictionary.Add("Komi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kg",
				GoogleCode = "-"
			};
			dictionary.Add("Kongo", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "ko"
			};
			dictionary.Add("Korean", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ku"
			};
			dictionary.Add("Kurdish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "kj",
				GoogleCode = "-"
			};
			dictionary.Add("Kuanyama", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "la"
			};
			dictionary.Add("Latin", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "lb"
			};
			dictionary.Add("Luxembourgish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "lg",
				GoogleCode = "-"
			};
			dictionary.Add("Ganda", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "li",
				GoogleCode = "-"
			};
			dictionary.Add("Limburgan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ln",
				GoogleCode = "-"
			};
			dictionary.Add("Lingala", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "lo"
			};
			dictionary.Add("Lao", value);
			value = new LanguageCodeDef
			{
				PluralRule = 5,
				Code = "lv"
			};
			dictionary.Add("Latvian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "lu",
				GoogleCode = "-"
			};
			dictionary.Add("Luba-Katanga", value);
			value = new LanguageCodeDef
			{
				PluralRule = 5,
				Code = "lt"
			};
			dictionary.Add("Lithuanian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "gv",
				GoogleCode = "-"
			};
			dictionary.Add("Manx", value);
			value = new LanguageCodeDef
			{
				PluralRule = 13,
				Code = "mk"
			};
			dictionary.Add("Macedonian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "mg"
			};
			dictionary.Add("Malagasy", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "ms"
			};
			dictionary.Add("Malay", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "ms-BN",
				GoogleCode = "ms"
			};
			dictionary.Add("Malay/Brunei Darussalam", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "ms-MY",
				GoogleCode = "ms"
			};
			dictionary.Add("Malay/Malaysia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ml"
			};
			dictionary.Add("Malayalam", value);
			value = new LanguageCodeDef
			{
				PluralRule = 12,
				Code = "mt"
			};
			dictionary.Add("Maltese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "mi"
			};
			dictionary.Add("Maori", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "mr"
			};
			dictionary.Add("Marathi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "mh",
				GoogleCode = "-"
			};
			dictionary.Add("Marshallese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "mn"
			};
			dictionary.Add("Mongolian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "na",
				GoogleCode = "-"
			};
			dictionary.Add("Nauru", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nv",
				GoogleCode = "-"
			};
			dictionary.Add("Navajo", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nd",
				GoogleCode = "-"
			};
			dictionary.Add("North Ndebele", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ne"
			};
			dictionary.Add("Nepali", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ng",
				GoogleCode = "-"
			};
			dictionary.Add("Ndonga", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ns",
				GoogleCode = "st"
			};
			dictionary.Add("Northern Sotho", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nb",
				GoogleCode = "no"
			};
			dictionary.Add("Norwegian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nn",
				GoogleCode = "no"
			};
			dictionary.Add("Norwegian/Nynorsk", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ii",
				GoogleCode = "-"
			};
			dictionary.Add("Sichuan Yi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "nr",
				GoogleCode = "-"
			};
			dictionary.Add("South Ndebele", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "oc",
				GoogleCode = "-"
			};
			dictionary.Add("Occitan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "oj",
				GoogleCode = "-"
			};
			dictionary.Add("Ojibwa", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "cu",
				GoogleCode = "-"
			};
			dictionary.Add("Church\u00a0Slavic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "om",
				GoogleCode = "-"
			};
			dictionary.Add("Oromo", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "or",
				GoogleCode = "-"
			};
			dictionary.Add("Oriya", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "os",
				GoogleCode = "-"
			};
			dictionary.Add("Ossetian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "pi",
				GoogleCode = "-"
			};
			dictionary.Add("Pali", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ps"
			};
			dictionary.Add("Pashto", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "fa"
			};
			dictionary.Add("Persian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 8,
				Code = "pl"
			};
			dictionary.Add("Polish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "pt"
			};
			dictionary.Add("Portuguese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "pt-BR",
				GoogleCode = "pt"
			};
			dictionary.Add("Portuguese/Brazil", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "pt-PT",
				GoogleCode = "pt"
			};
			dictionary.Add("Portuguese/Portugal", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "pa"
			};
			dictionary.Add("Punjabi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "qu",
				GoogleCode = "-"
			};
			dictionary.Add("Quechua", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "qu-BO",
				GoogleCode = "-"
			};
			dictionary.Add("Quechua/Bolivia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "qu-EC",
				GoogleCode = "-"
			};
			dictionary.Add("Quechua/Ecuador", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "qu-PE",
				GoogleCode = "-"
			};
			dictionary.Add("Quechua/Peru", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "rm",
				GoogleCode = "ro"
			};
			dictionary.Add("Rhaeto-Romanic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 4,
				Code = "ro"
			};
			dictionary.Add("Romanian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "rn",
				GoogleCode = "-"
			};
			dictionary.Add("Rundi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "ru"
			};
			dictionary.Add("Russian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "ru-MO",
				GoogleCode = "ru"
			};
			dictionary.Add("Russian/Republic of Moldova", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sa",
				GoogleCode = "-"
			};
			dictionary.Add("Sanskrit", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sc",
				GoogleCode = "-"
			};
			dictionary.Add("Sardinian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sd"
			};
			dictionary.Add("Sindhi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "se",
				GoogleCode = "-"
			};
			dictionary.Add("Northern Sami", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sm"
			};
			dictionary.Add("Samoan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sg",
				GoogleCode = "-"
			};
			dictionary.Add("Sango", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "sr"
			};
			dictionary.Add("Serbian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 5,
				Code = "sr-BA",
				GoogleCode = "sr"
			};
			dictionary.Add("Serbian/Bosnia and Herzegovina", value);
			value = new LanguageCodeDef
			{
				PluralRule = 5,
				Code = "sr-SP",
				GoogleCode = "sr"
			};
			dictionary.Add("Serbian/Serbia and Montenegro", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "gd"
			};
			dictionary.Add("Scottish Gaelic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sn"
			};
			dictionary.Add("Shona", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "si"
			};
			dictionary.Add("Sinhala", value);
			value = new LanguageCodeDef
			{
				PluralRule = 7,
				Code = "sk"
			};
			dictionary.Add("Slovak", value);
			value = new LanguageCodeDef
			{
				PluralRule = 9,
				Code = "sl"
			};
			dictionary.Add("Slovenian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "so"
			};
			dictionary.Add("Somali", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "st"
			};
			dictionary.Add("Southern Sotho", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es"
			};
			dictionary.Add("Spanish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-AR",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Argentina", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-BO",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Bolivia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-ES",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Castilian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-CL",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Chile", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-CO",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Colombia", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-CR",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Costa Rica", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-DO",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Dominican Republic", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-EC",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Ecuador", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-SV",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/El Salvador", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-GT",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Guatemala", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-HN",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Honduras", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-MX",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Mexico", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-NI",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Nicaragua", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-PA",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Panama", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-PY",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Paraguay", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-PE",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Peru", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-PR",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Puerto Rico", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-ES",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Spain", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-UY",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Uruguay", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-VE",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Venezuela", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "es-US",
				GoogleCode = "es"
			};
			dictionary.Add("Spanish/Latin Americas", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "su"
			};
			dictionary.Add("Sundanese", value);
			value = new LanguageCodeDef
			{
				Code = "sw"
			};
			dictionary.Add("Swahili", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ss",
				GoogleCode = "-"
			};
			dictionary.Add("Swati", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sv"
			};
			dictionary.Add("Swedish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sv-FI",
				GoogleCode = "sv"
			};
			dictionary.Add("Swedish/Finland", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "sv-SE",
				GoogleCode = "sv"
			};
			dictionary.Add("Swedish/Sweden", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ta"
			};
			dictionary.Add("Tamil", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "tt",
				GoogleCode = "-"
			};
			dictionary.Add("Tatar", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "te"
			};
			dictionary.Add("Telugu", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "tg"
			};
			dictionary.Add("Tajik", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "th",
				HasJoinedWords = true
			};
			dictionary.Add("Thai", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ti",
				GoogleCode = "-"
			};
			dictionary.Add("Tigrinya", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "bo",
				GoogleCode = "-"
			};
			dictionary.Add("Tibetan", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "tk",
				GoogleCode = "-"
			};
			dictionary.Add("Turkmen", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "tl"
			};
			dictionary.Add("Tagalog", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "tn",
				GoogleCode = "-"
			};
			dictionary.Add("Tswana", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "to",
				GoogleCode = "-"
			};
			dictionary.Add("Tonga", value);
			value = new LanguageCodeDef
			{
				PluralRule = 0,
				Code = "tr"
			};
			dictionary.Add("Turkish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ts",
				GoogleCode = "-"
			};
			dictionary.Add("Tsonga", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "tw",
				GoogleCode = "-"
			};
			dictionary.Add("Twi", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ty",
				GoogleCode = "-"
			};
			dictionary.Add("Tahitian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ug",
				GoogleCode = "-"
			};
			dictionary.Add("Uighur", value);
			value = new LanguageCodeDef
			{
				PluralRule = 6,
				Code = "uk"
			};
			dictionary.Add("Ukrainian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ur"
			};
			dictionary.Add("Urdu", value);
			value = new LanguageCodeDef
			{
				PluralRule = 2,
				Code = "uz"
			};
			dictionary.Add("Uzbek", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "ve",
				GoogleCode = "-"
			};
			dictionary.Add("Venda", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "vi"
			};
			dictionary.Add("Vietnamese", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "vo",
				GoogleCode = "-"
			};
			dictionary.Add("Volapük", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "wa",
				GoogleCode = "-"
			};
			dictionary.Add("Walloon", value);
			value = new LanguageCodeDef
			{
				PluralRule = 16,
				Code = "cy"
			};
			dictionary.Add("Welsh", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "wo",
				GoogleCode = "-"
			};
			dictionary.Add("Wolof", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "fy"
			};
			dictionary.Add("Frisian", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "xh"
			};
			dictionary.Add("Xhosa", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "yi"
			};
			dictionary.Add("Yiddish", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "yo"
			};
			dictionary.Add("Yoruba", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "za",
				GoogleCode = "-"
			};
			dictionary.Add("Zhuang", value);
			value = new LanguageCodeDef
			{
				PluralRule = 1,
				Code = "zu"
			};
			dictionary.Add("Zulu", value);
			mLanguageDef = dictionary;
		}
	}
}
