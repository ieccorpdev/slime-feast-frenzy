using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace IEC
{
    public class UserLocationIP : IUserLocation
    {
        const string REQUEST_INFO_URL = "http://extreme-ip-lookup.com/json/";
        const string REQUEST_INFO_URL_2 = "http://apigame.dtac.vn/helper/get_country";

        MonoBehaviour m_Coroutiner;
        IJsonService m_Jsoner;

        CountryInfoIP m_Info;
        bool m_IsHasInfo = false;

        Dictionary<string, Dictionary<string, string>> m_RegionDB;

        public UserLocationIP(MonoBehaviour coroutiner, IJsonService jsoner)
        {
            m_Coroutiner = coroutiner;
            m_Jsoner = jsoner;
            m_RegionDB = m_Jsoner.FromJson<Dictionary<string, Dictionary<string, string>>>(COUNTRY_REGION_DB);
            m_Info = new CountryInfoIP();
        }

        public void RequestInfo(Action<bool> callback)
        {
            m_Coroutiner.StartCoroutine(IERequestInfo(callback));
        }

        IEnumerator IERequestInfo(Action<bool> callback)
        {
            var www = UnityWebRequest.Get(REQUEST_INFO_URL);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error get location info: " + www.error);
                callback(false);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                try
                {
                    var info = m_Jsoner.FromJson<CountryInfoIP>(www.downloadHandler.text);

                    if (info != null)
                    {
                        m_Info = info;
                        m_IsHasInfo = true;
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }
                }
                catch
                {
                    callback(false);
                }
            }
        }

        public string GetAddressInfo()
        {
            return m_Info.ipAddress;
        }

        public string GetCountryCode()
        {
            return m_Info.countryCode;
        }

        public string GetCountryName()
        {
            return m_Info.countryName;
        }

        public string GetRegion()
        {
            var countryCode = GetCountryCode();
            if (string.IsNullOrEmpty(countryCode)) return "";
            countryCode = countryCode.ToLower();

            foreach (var region in m_RegionDB.Keys)
            {
                if (m_RegionDB[region].ContainsKey(countryCode))
                {
                    return region;
                }
            }
            return string.Empty;
        }

        public bool IsRegionGDPR()
        {
            if (Application.isEditor)
            {
                return true;
            }
            else
            {
                return GetRegion() == "gdpr";
            }
        }

        public bool IsHasInfo()
        {
            return m_IsHasInfo;
        }

        class CountryInfoIP
        {
            public string ipAddress = "";
            public string countryCode = "";
            public string countryName = "";
            public string region = "";
        }

        const string COUNTRY_REGION_DB =
        @"{
			'gdpr': {
		        'ch': 'switzerland',
		        'ee': 'estonia',
		        'cz': 'czech republic',
		        'at': 'austria',
		        'ie': 'ireland',
		        'gr': 'greece',
		        'nl': 'netherlands',
		        'pt': 'portugal',
		        'lv': 'latvia',
		        'lt': 'lithuania',
		        'lu': 'luxembourg',
		        'es': 'spain',
		        'it': 'italy',
		        'ro': 'romania',
		        'pl': 'poland',
		        'be': 'belgium',
		        'fr': 'france',
		        'bg': 'bulgaria',
		        'dk': 'denmark',
		        'hr': 'croatia',
		        'de': 'germany',
		        'hu': 'hungary',
		        'fi': 'finland',
		        'cy': 'cyprus',
		        'sk': 'slovakia',
		        'mt': 'malta',
		        'si': 'slovenia',
		        'se': 'sweden',
		        'gb': 'united kingdom'
	        },
	        'europe': {
		        'va': 'vatican city',
		        'ch': 'switzerland',
		        'ad': 'andorra',
		        'ee': 'estonia',
		        'is': 'iceland',
		        'am': 'armenia',
		        'al': 'albania',
		        'cz': 'czech republic',
		        'ge': 'georgia',
		        'at': 'austria',
		        'ie': 'ireland',
		        'gi': 'gibraltar',
		        'gr': 'greece',
		        'nl': 'netherlands',
		        'pt': 'portugal',
		        'no': 'norway',
		        'lv': 'latvia',
		        'lt': 'lithuania',
		        'lu': 'luxembourg',
		        'es': 'spain',
		        'it': 'italy',
		        'ro': 'romania',
		        'pl': 'poland',
		        'be': 'belgium',
		        'fr': 'france',
		        'bg': 'bulgaria',
		        'dk': 'denmark',
		        'hr': 'croatia',
		        'de': 'germany',
		        'hu': 'hungary',
		        'ba': 'bosnia/herzegovina',
		        'fi': 'finland',
		        'by': 'belarus',
		        'fo': 'faeroe islands',
		        'mc': 'monaco',
		        'cy': 'cyprus',
		        'mk': 'macedonia',
		        'sk': 'slovakia',
		        'mt': 'malta',
		        'si': 'slovenia',
		        'sm': 'san marino',
		        'se': 'sweden',
		        'gb': 'united kingdom'
	        },
	        'oceania': {
		        'ck': 'cook islands',
		        'pw': 'palau',
		        'tv': 'tuvalu',
		        'na': 'nauru',
		        'ki': 'kiribati',
		        'mh': 'marshall islands',
		        'nu': 'niue',
		        'to': 'tonga',
		        'nz': 'new zealand',
		        'au': 'australia',
		        'vu': 'vanuatu',
		        'sb': 'solomon islands',
		        'ws': 'samoa',
		        'fj': 'fiji',
		        'fm': 'micronesia'
	        },
	        'africa': {
		        'gw': 'guinea-bissau',
		        'zm': 'zambia',
		        'ci': 'ivory coast',
		        'eh': 'western sahara',
		        'gq': 'equatorial guinea',
		        'eg': 'egypt',
		        'cg': 'congo',
		        'cf': 'central african republic',
		        'ao': 'angola',
		        'ga': 'gabon',
		        'et': 'ethiopia',
		        'gn': 'guinea',
		        'gm': 'gambia',
		        'zw': 'zimbabwe',
		        'cv': 'cape verde',
		        'gh': 'ghana',
		        'rw': 'rwanda',
		        'tz': 'tanzania',
		        'cm': 'cameroon',
		        'na': 'namibia',
		        'ne': 'niger',
		        'ng': 'nigeria',
		        'tn': 'tunisia',
		        'lr': 'liberia',
		        'ls': 'lesotho',
		        'tg': 'togo',
		        'td': 'chad',
		        'er': 'eritrea',
		        'ly': 'libya',
		        'bf': 'burkina faso',
		        'dj': 'djibouti',
		        'sl': 'sierra leone',
		        'bi': 'burundi',
		        'bj': 'benin',
		        'za': 'south africa',
		        'bw': 'botswana',
		        'dz': 'algeria',
		        'sz': 'swaziland',
		        'mg': 'madagascar',
		        'ma': 'morocco',
		        'ke': 'kenya',
		        'ml': 'mali',
		        'km': 'comoros',
		        'st': 'sao tome and principe',
		        'mu': 'mauritius',
		        'mw': 'malawi',
		        'so': 'somalia',
		        'sn': 'senegal',
		        'mr': 'mauritania',
		        'sc': 'seychelles',
		        'ug': 'uganda',
		        'sd': 'sudan',
		        'mz': 'mozambique'
	        },
	        'asia': {
		        'mn': 'mongolia',
		        'cn': 'china',
		        'af': 'afghanistan',
		        'am': 'armenia',
		        'vn': 'vietnam',
		        'ge': 'georgia',
		        'in': 'india',
		        'az': 'azerbaijan',
		        'id': 'indonesia',
		        'ru': 'russia',
		        'la': 'laos',
		        'tw': 'taiwan',
		        'tr': 'turkey',
		        'lk': 'sri lanka',
		        'tm': 'turkmenistan',
		        'tj': 'tajikistan',
		        'pg': 'papua new guinea',
		        'th': 'thailand',
		        'np': 'nepal',
		        'pk': 'pakistan',
		        'ph': 'philippines',
		        'bd': 'bangladesh',
		        'ua': 'ukraine',
		        'bn': 'brunei',
		        'jp': 'japan',
		        'bt': 'bhutan',
		        'hk': 'hong kong',
		        'kg': 'kyrgyzstan',
		        'uz': 'uzbekistan',
		        'mm': 'burma (myanmar)',
		        'sg': 'singapore',
		        'mo': 'macau',
		        'kh': 'cambodia',
		        'kr': 'korea',
		        'mv': 'maldives',
		        'kz': 'kazakhstan',
		        'my': 'malaysia'
	        },
	        'north america': {
		        'gt': 'guatemala',
		        'ag': 'antigua and barbuda',
		        'vg': 'british virgin islands (uk)',
		        'ai': 'anguilla (uk)',
		        'vi': 'virgin island',
		        'ca': 'canada',
		        'gd': 'grenada',
		        'aw': 'aruba (netherlands)',
		        'cr': 'costa rica',
		        'cu': 'cuba',
		        'pr': 'puerto rico (us)',
		        'ni': 'nicaragua',
		        'tt': 'trinidad and tobago',
		        'gp': 'guadeloupe (france)',
		        'pa': 'panama',
		        'do': 'dominican republic',
		        'dm': 'dominica',
		        'bb': 'barbados',
		        'ht': 'haiti',
		        'jm': 'jamaica',
		        'hn': 'honduras',
		        'bs': 'bahamas, the',
		        'bz': 'belize',
		        'sx': 'saint kitts and nevis',
		        'sv': 'el salvador',
		        'us': 'united states',
		        'mq': 'martinique (france)',
		        'ms': 'monsterrat (uk)',
		        'ky': 'cayman islands (uk)',
		        'mx': 'mexico'
	        },
	        'south america': {
		        'gd': 'south georgia',
		        'py': 'paraguay',
		        'co': 'colombia',
		        've': 'venezuela',
		        'cl': 'chile',
		        'sr': 'suriname',
		        'bo': 'bolivia',
		        'ec': 'ecuador',
		        'gf': 'french guiana',
		        'ar': 'argentina',
		        'gy': 'guyana',
		        'br': 'brazil',
		        'pe': 'peru',
		        'uy': 'uruguay',
		        'fk': 'falkland islands'
	        },
	        'middle east': {
		        'om': 'oman',
		        'lb': 'lebanon',
		        'iq': 'iraq',
		        'ye': 'yemen',
		        'ir': 'iran',
		        'bh': 'bahrain',
		        'sy': 'syria',
		        'qa': 'qatar',
		        'jo': 'jordan',
		        'kw': 'kuwait',
		        'il': 'israel',
		        'ae': 'united arab emirates',
		        'sa': 'saudi arabia'
	        }
        }";

    }
}
