using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilEncConverters40.EcTranslators.BingTranslator
{
	public class TranslationLanguage
	{
		public string IsoCode { get; set; }         // e.g. 'ar'
		public string Name { get; set; }            // e.g.	'Arabic'
		public string NativeName { get; set; }      // e.g. "العربية"
		public string Direction { get; set; }       // e.g. 'rtl'

		public override string ToString()           // for now, we'll display this as /Arabic|العربية (ar)/
		{
			return (Name == NativeName)
					? $"{Name} ({IsoCode})"
					: $"{Name} {NativeName} ({IsoCode})";
		}

		/// <summary>
		/// For when the IsoCode is a the key of a sub-translation (e.g. "en": {...})
		/// (i.e. for /translation)
		/// </summary>
		/// <param name="translationTokens"></param>
		/// <returns></returns>
		public static List<TranslationLanguage> LoadFromJTokens(JEnumerable<JToken>? translationTokens)
		{
			return translationTokens?.Select(t =>
			{
				var jToken = t.FirstOrDefault();
				return new TranslationLanguage
				{
					IsoCode = (t as JProperty).Name,
					Name = jToken["name"]?.ToString(),
					NativeName = jToken["nativeName"]?.ToString(),
					Direction = jToken["dir"]?.ToString(),
				};
			}).ToList();
		}

		/// <summary>
		/// for when the 'translation' is just a child w/out the iso code as the key
		/// (e.g. /dictionary/en/translations or /transliteration/en/scripts/toScripts)
		/// </summary>
		/// <param name="translationTokens"></param>
		/// <returns></returns>
		public static List<TranslationLanguage> LoadFromSubJTokens(JEnumerable<JToken>? translationTokens)
		{
			return translationTokens?.Select(t =>
			{
				return new TranslationLanguage
				{
					IsoCode = t["code"]?.ToString(),    // different from above
					Name = t["name"]?.ToString(),
					NativeName = t["nativeName"]?.ToString(),
					Direction = t["dir"]?.ToString(),
				};
			}).ToList();
		}
	}

	public class TransliterationLanguage
	{
		public string IsoCode { get; set; }         // e.g. 'ar'
		public string Name { get; set; }            // e.g.	'Arabic'
		public string NativeName { get; set; }      // e.g. "العربية"
		public List<Script> ScriptsSupported { get; set; }

		public override string ToString()           // for now, we'll display this as /Arabic|العربية (ar)/
		{
			return (Name == NativeName)
					? $"{Name} ({IsoCode})"
					: $"{Name} {NativeName} ({IsoCode})" +
					$" => ({String.Join(") OR (", ScriptsSupported.SelectMany(s => s.ToScripts))})";
		}

		public static List<TransliterationLanguage> LoadFromJTokens(JEnumerable<JToken>? transliterationsTokens)
		{
			return transliterationsTokens?.Select(t =>
			{
				var jToken = t.FirstOrDefault();
				return new TransliterationLanguage
				{
					IsoCode = (t as JProperty)?.Name,
					Name = jToken["name"]?.ToString(),
					NativeName = jToken["nativeName"]?.ToString(),
					ScriptsSupported = Script.LoadFromJTokens(jToken["scripts"]?.Children()),
				};
			}).ToList();
		}
	}

	public class Script : TranslationLanguage
	{
		public List<TranslationLanguage> ToScripts { get; set; }

		public new static List<Script> LoadFromJTokens(JEnumerable<JToken>? scriptTokens)
		{
			return scriptTokens?.Select(s =>
			{
				return new Script
				{
					IsoCode = s["code"]?.ToString(),
					Name = s["name"]?.ToString(),
					NativeName = s["nativeName"]?.ToString(),
					Direction = s["dir"]?.ToString(),
					ToScripts = LoadFromSubJTokens(s["toScripts"]?.Children()),
				};
			}).ToList();
		}
	}

	public class DictionaryLanguage : TranslationLanguage
	{
		public List<TranslationLanguage> Translations { get; set; }

		public override string ToString()           // for now, we'll display this as /Arabic|العربية (ar)/
		{
			return (Name == NativeName)
					? $"{Name} ({IsoCode})"
					: $"{Name} {NativeName} ({IsoCode})" +
					$" => {String.Join(" | ", Translations)}";
		}

		public static List<DictionaryLanguage> LoadFromJTokens(JEnumerable<JToken> dictionaryTokens)
		{
			return dictionaryTokens.Select(d =>
			{
				var jToken = d.FirstOrDefault();
				return new DictionaryLanguage
				{
					IsoCode = (d as JProperty).Name,
					Name = jToken["name"]?.ToString(),
					NativeName = jToken["nativeName"]?.ToString(),
					Direction = jToken["dir"]?.ToString(),
					Translations = LoadFromSubJTokens(jToken["translations"].Children()),
				};
			}).ToList();
		}
	}

	/*
		public class Rootobject
		{
			public Translation translation { get; set; }
			public Transliteration transliteration { get; set; }
			public Dictionary dictionary { get; set; }
		}

		public class Translation
		{
			public Af af { get; set; }
			public Am am { get; set; }
			public Ar ar { get; set; }
			public As _as { get; set; }
			public Az az { get; set; }
			public Ba ba { get; set; }
			public Bg bg { get; set; }
			public Bn bn { get; set; }
			public Bo bo { get; set; }
			public Bs bs { get; set; }
			public Ca ca { get; set; }
			public Cs cs { get; set; }
			public Cy cy { get; set; }
			public Da da { get; set; }
			public De de { get; set; }
			public Dv dv { get; set; }
			public El el { get; set; }
			public En en { get; set; }
			public Es es { get; set; }
			public Et et { get; set; }
			public Fa fa { get; set; }
			public Fi fi { get; set; }
			public Fil fil { get; set; }
			public Fj fj { get; set; }
			public Fr fr { get; set; }
			public FrCA frCA { get; set; }
			public Ga ga { get; set; }
			public Gu gu { get; set; }
			public He he { get; set; }
			public Hi hi { get; set; }
			public Hr hr { get; set; }
			public Ht ht { get; set; }
			public Hu hu { get; set; }
			public Hy hy { get; set; }
			public Id id { get; set; }
			public Is _is { get; set; }
			public It it { get; set; }
			public Iu iu { get; set; }
			public Ja ja { get; set; }
			public Ka ka { get; set; }
			public Kk kk { get; set; }
			public Km km { get; set; }
			public Kmr kmr { get; set; }
			public Kn kn { get; set; }
			public Ko ko { get; set; }
			public Ku ku { get; set; }
			public Ky ky { get; set; }
			public Lo lo { get; set; }
			public Lt lt { get; set; }
			public Lv lv { get; set; }
			public Lzh lzh { get; set; }
			public Mg mg { get; set; }
			public Mi mi { get; set; }
			public Mk mk { get; set; }
			public Ml ml { get; set; }
			public MnCyrl mnCyrl { get; set; }
			public MnMong mnMong { get; set; }
			public Mr mr { get; set; }
			public Ms ms { get; set; }
			public Mt mt { get; set; }
			public Mww mww { get; set; }
			public My my { get; set; }
			public Nb nb { get; set; }
			public Ne ne { get; set; }
			public Nl nl { get; set; }
			public Or or { get; set; }
			public Otq otq { get; set; }
			public Pa pa { get; set; }
			public Pl pl { get; set; }
			public Prs prs { get; set; }
			public Ps ps { get; set; }
			public Pt pt { get; set; }
			public PtPT ptPT { get; set; }
			public Ro ro { get; set; }
			public Ru ru { get; set; }
			public Sk sk { get; set; }
			public Sl sl { get; set; }
			public Sm sm { get; set; }
			public Sq sq { get; set; }
			public SrCyrl srCyrl { get; set; }
			public SrLatn srLatn { get; set; }
			public Sv sv { get; set; }
			public Sw sw { get; set; }
			public Ta ta { get; set; }
			public Te te { get; set; }
			public Th th { get; set; }
			public Ti ti { get; set; }
			public Tk tk { get; set; }
			public TlhLatn tlhLatn { get; set; }
			public TlhPiqd tlhPiqd { get; set; }
			public To to { get; set; }
			public Tr tr { get; set; }
			public Tt tt { get; set; }
			public Ty ty { get; set; }
			public Ug ug { get; set; }
			public Uk uk { get; set; }
			public Ur ur { get; set; }
			public Uz uz { get; set; }
			public Vi vi { get; set; }
			public Yua yua { get; set; }
			public Yue yue { get; set; }
			public ZhHans zhHans { get; set; }
			public ZhHant zhHant { get; set; }
		}

		public class Af
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Am
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ar
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class As
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Az
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ba
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Bg
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Bn
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Bo
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Bs
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ca
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Cs
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Cy
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Da
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class De
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Dv
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class El
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class En
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Es
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Et
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Fa
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Fi
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Fil
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Fj
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Fr
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class FrCA
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ga
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Gu
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class He
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Hi
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Hr
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ht
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Hu
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Hy
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Id
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Is
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class It
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Iu
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ja
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ka
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Kk
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Km
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Kmr
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Kn
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ko
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ku
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ky
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Lo
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Lt
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Lv
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Lzh
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mg
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mi
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mk
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ml
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class MnCyrl
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class MnMong
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mr
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ms
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mt
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mww
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class My
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Nb
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ne
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Nl
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Or
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Otq
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Pa
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Pl
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Prs
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ps
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Pt
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class PtPT
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ro
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ru
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sk
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sl
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sm
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sq
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class SrCyrl
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class SrLatn
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sv
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sw
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ta
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Te
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Th
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ti
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Tk
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class TlhLatn
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class TlhPiqd
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class To
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Tr
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Tt
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ty
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ug
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Uk
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ur
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Uz
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Vi
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Yua
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Yue
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class ZhHans
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class ZhHant
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Transliteration
		{
			public Ar1 ar { get; set; }
			public As1 _as { get; set; }
			public Be be { get; set; }
			public Bg1 bg { get; set; }
			public Bn1 bn { get; set; }
			public El1 el { get; set; }
			public Fa1 fa { get; set; }
			public Gu1 gu { get; set; }
			public He1 he { get; set; }
			public Hi1 hi { get; set; }
			public Ja1 ja { get; set; }
			public Kk1 kk { get; set; }
			public Kn1 kn { get; set; }
			public Ko1 ko { get; set; }
			public Ky1 ky { get; set; }
			public Mk1 mk { get; set; }
			public Ml1 ml { get; set; }
			public MnCyrl1 mnCyrl { get; set; }
			public Mr1 mr { get; set; }
			public Or1 or { get; set; }
			public Pa1 pa { get; set; }
			public Ru1 ru { get; set; }
			public Sd sd { get; set; }
			public Si si { get; set; }
			public SrCyrl1 srCyrl { get; set; }
			public SrLatn1 srLatn { get; set; }
			public Ta1 ta { get; set; }
			public Te1 te { get; set; }
			public Tg tg { get; set; }
			public Th1 th { get; set; }
			public Tt1 tt { get; set; }
			public Uk1 uk { get; set; }
			public Ur1 ur { get; set; }
			public ZhHans1 zhHans { get; set; }
			public ZhHant1 zhHant { get; set; }
		}

		public class Ar1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script[] scripts { get; set; }
		}

		public class Script
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript[] toScripts { get; set; }
		}

		public class Toscript
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class As1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script1[] scripts { get; set; }
		}

		public class Script1
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript1[] toScripts { get; set; }
		}

		public class Toscript1
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Be
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script2[] scripts { get; set; }
		}

		public class Script2
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript2[] toScripts { get; set; }
		}

		public class Toscript2
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Bg1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script3[] scripts { get; set; }
		}

		public class Script3
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript3[] toScripts { get; set; }
		}

		public class Toscript3
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Bn1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script4[] scripts { get; set; }
		}

		public class Script4
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript4[] toScripts { get; set; }
		}

		public class Toscript4
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class El1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script5[] scripts { get; set; }
		}

		public class Script5
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript5[] toScripts { get; set; }
		}

		public class Toscript5
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Fa1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script6[] scripts { get; set; }
		}

		public class Script6
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript6[] toScripts { get; set; }
		}

		public class Toscript6
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Gu1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script7[] scripts { get; set; }
		}

		public class Script7
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript7[] toScripts { get; set; }
		}

		public class Toscript7
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class He1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script8[] scripts { get; set; }
		}

		public class Script8
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript8[] toScripts { get; set; }
		}

		public class Toscript8
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Hi1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script9[] scripts { get; set; }
		}

		public class Script9
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript9[] toScripts { get; set; }
		}

		public class Toscript9
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ja1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script10[] scripts { get; set; }
		}

		public class Script10
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript10[] toScripts { get; set; }
		}

		public class Toscript10
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Kk1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script11[] scripts { get; set; }
		}

		public class Script11
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript11[] toScripts { get; set; }
		}

		public class Toscript11
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Kn1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script12[] scripts { get; set; }
		}

		public class Script12
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript12[] toScripts { get; set; }
		}

		public class Toscript12
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ko1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script13[] scripts { get; set; }
		}

		public class Script13
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript13[] toScripts { get; set; }
		}

		public class Toscript13
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ky1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script14[] scripts { get; set; }
		}

		public class Script14
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript14[] toScripts { get; set; }
		}

		public class Toscript14
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mk1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script15[] scripts { get; set; }
		}

		public class Script15
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript15[] toScripts { get; set; }
		}

		public class Toscript15
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ml1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script16[] scripts { get; set; }
		}

		public class Script16
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript16[] toScripts { get; set; }
		}

		public class Toscript16
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class MnCyrl1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script17[] scripts { get; set; }
		}

		public class Script17
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript17[] toScripts { get; set; }
		}

		public class Toscript17
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Mr1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script18[] scripts { get; set; }
		}

		public class Script18
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript18[] toScripts { get; set; }
		}

		public class Toscript18
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Or1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script19[] scripts { get; set; }
		}

		public class Script19
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript19[] toScripts { get; set; }
		}

		public class Toscript19
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Pa1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script20[] scripts { get; set; }
		}

		public class Script20
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript20[] toScripts { get; set; }
		}

		public class Toscript20
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ru1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script21[] scripts { get; set; }
		}

		public class Script21
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript21[] toScripts { get; set; }
		}

		public class Toscript21
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Sd
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script22[] scripts { get; set; }
		}

		public class Script22
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript22[] toScripts { get; set; }
		}

		public class Toscript22
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Si
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script23[] scripts { get; set; }
		}

		public class Script23
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript23[] toScripts { get; set; }
		}

		public class Toscript23
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class SrCyrl1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script24[] scripts { get; set; }
		}

		public class Script24
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript24[] toScripts { get; set; }
		}

		public class Toscript24
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class SrLatn1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script25[] scripts { get; set; }
		}

		public class Script25
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript25[] toScripts { get; set; }
		}

		public class Toscript25
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ta1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script26[] scripts { get; set; }
		}

		public class Script26
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript26[] toScripts { get; set; }
		}

		public class Toscript26
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Te1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script27[] scripts { get; set; }
		}

		public class Script27
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript27[] toScripts { get; set; }
		}

		public class Toscript27
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Tg
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script28[] scripts { get; set; }
		}

		public class Script28
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript28[] toScripts { get; set; }
		}

		public class Toscript28
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Th1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script29[] scripts { get; set; }
		}

		public class Script29
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript29[] toScripts { get; set; }
		}

		public class Toscript29
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Tt1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script30[] scripts { get; set; }
		}

		public class Script30
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript30[] toScripts { get; set; }
		}

		public class Toscript30
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Uk1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script31[] scripts { get; set; }
		}

		public class Script31
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript31[] toScripts { get; set; }
		}

		public class Toscript31
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Ur1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script32[] scripts { get; set; }
		}

		public class Script32
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript32[] toScripts { get; set; }
		}

		public class Toscript32
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class ZhHans1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script33[] scripts { get; set; }
		}

		public class Script33
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript33[] toScripts { get; set; }
		}

		public class Toscript33
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class ZhHant1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public Script34[] scripts { get; set; }
		}

		public class Script34
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Toscript34[] toScripts { get; set; }
		}

		public class Toscript34
		{
			public string code { get; set; }
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
		}

		public class Dictionary
		{
			public Af1 af { get; set; }
			public Ar2 ar { get; set; }
			public Bg2 bg { get; set; }
			public Bn2 bn { get; set; }
			public Bs1 bs { get; set; }
			public Ca1 ca { get; set; }
			public Cs1 cs { get; set; }
			public Cy1 cy { get; set; }
			public Da1 da { get; set; }
			public De1 de { get; set; }
			public El2 el { get; set; }
			public En1 en { get; set; }
			public Es1 es { get; set; }
			public Et1 et { get; set; }
			public Fa2 fa { get; set; }
			public Fi1 fi { get; set; }
			public Fr1 fr { get; set; }
			public He2 he { get; set; }
			public Hi2 hi { get; set; }
			public Hr1 hr { get; set; }
			public Ht1 ht { get; set; }
			public Hu1 hu { get; set; }
			public Id1 id { get; set; }
			public Is1 _is { get; set; }
			public It1 it { get; set; }
			public Ja2 ja { get; set; }
			public Ko2 ko { get; set; }
			public Lt1 lt { get; set; }
			public Lv1 lv { get; set; }
			public Ms1 ms { get; set; }
			public Mt1 mt { get; set; }
			public Mww1 mww { get; set; }
			public Nb1 nb { get; set; }
			public Nl1 nl { get; set; }
			public Pl1 pl { get; set; }
			public Pt1 pt { get; set; }
			public Ro1 ro { get; set; }
			public Ru2 ru { get; set; }
			public Sk1 sk { get; set; }
			public Sl1 sl { get; set; }
			public SrLatn2 srLatn { get; set; }
			public Sv1 sv { get; set; }
			public Sw1 sw { get; set; }
			public Ta2 ta { get; set; }
			public Th2 th { get; set; }
			public TlhLatn1 tlhLatn { get; set; }
			public Tr1 tr { get; set; }
			public Uk2 uk { get; set; }
			public Ur2 ur { get; set; }
			public Vi1 vi { get; set; }
			public ZhHans2 zhHans { get; set; }
		}

		public class Af1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation1[] translations { get; set; }
		}

		public class Translation1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ar2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation2[] translations { get; set; }
		}

		public class Translation2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Bg2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation3[] translations { get; set; }
		}

		public class Translation3
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Bn2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation4[] translations { get; set; }
		}

		public class Translation4
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Bs1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation5[] translations { get; set; }
		}

		public class Translation5
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ca1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation6[] translations { get; set; }
		}

		public class Translation6
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Cs1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation7[] translations { get; set; }
		}

		public class Translation7
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Cy1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation8[] translations { get; set; }
		}

		public class Translation8
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Da1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation9[] translations { get; set; }
		}

		public class Translation9
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class De1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation10[] translations { get; set; }
		}

		public class Translation10
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class El2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation11[] translations { get; set; }
		}

		public class Translation11
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class En1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation12[] translations { get; set; }
		}

		public class Translation12
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Es1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation13[] translations { get; set; }
		}

		public class Translation13
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Et1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation14[] translations { get; set; }
		}

		public class Translation14
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Fa2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation15[] translations { get; set; }
		}

		public class Translation15
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Fi1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation16[] translations { get; set; }
		}

		public class Translation16
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Fr1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation17[] translations { get; set; }
		}

		public class Translation17
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class He2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation18[] translations { get; set; }
		}

		public class Translation18
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Hi2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation19[] translations { get; set; }
		}

		public class Translation19
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Hr1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation20[] translations { get; set; }
		}

		public class Translation20
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ht1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation21[] translations { get; set; }
		}

		public class Translation21
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Hu1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation22[] translations { get; set; }
		}

		public class Translation22
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Id1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation23[] translations { get; set; }
		}

		public class Translation23
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Is1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation24[] translations { get; set; }
		}

		public class Translation24
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class It1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation25[] translations { get; set; }
		}

		public class Translation25
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ja2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation26[] translations { get; set; }
		}

		public class Translation26
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ko2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation27[] translations { get; set; }
		}

		public class Translation27
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Lt1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation28[] translations { get; set; }
		}

		public class Translation28
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Lv1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation29[] translations { get; set; }
		}

		public class Translation29
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ms1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation30[] translations { get; set; }
		}

		public class Translation30
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Mt1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation31[] translations { get; set; }
		}

		public class Translation31
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Mww1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation32[] translations { get; set; }
		}

		public class Translation32
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Nb1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation33[] translations { get; set; }
		}

		public class Translation33
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Nl1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation34[] translations { get; set; }
		}

		public class Translation34
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Pl1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation35[] translations { get; set; }
		}

		public class Translation35
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Pt1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation36[] translations { get; set; }
		}

		public class Translation36
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ro1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation37[] translations { get; set; }
		}

		public class Translation37
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ru2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation38[] translations { get; set; }
		}

		public class Translation38
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Sk1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation39[] translations { get; set; }
		}

		public class Translation39
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Sl1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation40[] translations { get; set; }
		}

		public class Translation40
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class SrLatn2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation41[] translations { get; set; }
		}

		public class Translation41
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Sv1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation42[] translations { get; set; }
		}

		public class Translation42
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Sw1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation43[] translations { get; set; }
		}

		public class Translation43
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ta2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation44[] translations { get; set; }
		}

		public class Translation44
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Th2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation45[] translations { get; set; }
		}

		public class Translation45
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class TlhLatn1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation46[] translations { get; set; }
		}

		public class Translation46
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Tr1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation47[] translations { get; set; }
		}

		public class Translation47
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Uk2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation48[] translations { get; set; }
		}

		public class Translation48
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Ur2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation49[] translations { get; set; }
		}

		public class Translation49
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class Vi1
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation50[] translations { get; set; }
		}

		public class Translation50
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}

		public class ZhHans2
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public Translation51[] translations { get; set; }
		}

		public class Translation51
		{
			public string name { get; set; }
			public string nativeName { get; set; }
			public string dir { get; set; }
			public string code { get; set; }
		}
	*/
}
