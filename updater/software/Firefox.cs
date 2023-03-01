﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/110.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "b6bf30764a94952c6a2a6f215f9e32f53b57d4afd175ba273e119a16a8e217d1a4fd6ab1b41491c7b4fbd50886a035b95ae4f18647838cda46d60db8234c3196" },
                { "af", "87326e606da89f2824f485dbd128c9a41f7d85c9232d442d77e051452179c17114e7b4726525f351270a047161366f2c0c6b315befb2e96c19c7d1e4073ee15c" },
                { "an", "46e2a7de35b989edf1d1ef413be91895fce8b271c9f8059f5662fc264a25ece3c865017a8d250d6d879967b464ed12b1cd016f978575636d63d8737bd08a25c7" },
                { "ar", "d798f26149101ece5f7ccb6efba2d766323d88e70542ba2b94f26a0386543fc1d8e5e713d53a800de082eb5ab458bd4338c6f3f058094c0d5c47f47912439be5" },
                { "ast", "c5eb61ec69673a4c40a319600a5e36548fd4e657c1ed6f555c93eae2c338e2d44dc9968a93808bc7a4283000e56a9d9a0f9afe89b7200e099dc52ada58d1e5a6" },
                { "az", "b73d4d376ab66b8e901bbfced7d27a74b04302c629c9d4eef4ae104c677aa2ad752880aa29c0d51bc8fccf7a4f4c6fdcbead76c35c505364282a0a37c356992a" },
                { "be", "86a8bd019d0e3c766633ea53ece4febaa0af8fd19a8a7874d947009753a47eb1b8a7baa925de0adb84984505cb5068232eeb1b6cc4f9f63d18971946e3f4c223" },
                { "bg", "2f9c5f035df20e53083c7356751d5d1978b78d90c2e91c1c0712afaa00353d06af7069da219322ab42168f851fa1f749272aba148dbe127f58d5088dd5108087" },
                { "bn", "717367c754ff3d6a3174cdcdf5eb29e72c9bc3653a3a95474fe5b1d067b376083ba092ee9731e33cc3a240ad4965c14b5977273f270b5d0edd4ddc8b1bfbbd08" },
                { "br", "572521fe5202bb4a781f4a8e01f43d28c30c7aa23cf8ab2bf242cbbc4a0bc2874da76ad508b3328c5c5764e9c0001436023e2346b6a37db3a5697a8ce25e571b" },
                { "bs", "f45f922cb6ed3d6aa85923e80bcf79c526567d8abfd6a3d3e711f611abf5a473f6390b86718cb3e95d3b6540579a9f977e3c3e562f24e263bde116a18581ab9b" },
                { "ca", "da9ee355b27efbaa86a577ef329d647c0cd66b4c5209056c640369b2080c7cffb4a29661d6f272cfda990f097c56eeef7e51cf942d13953f7c24e0aacde09d10" },
                { "cak", "2586f42642dd5e407bae0ec0121066df7b7dcdb97c5f525c7d7684d5f135ed6f838dcc8b37fa9c9ca40692bc73ef5076ec4ba2795d03b95cacad47bfe9fa4c7a" },
                { "cs", "e36012743ca92b20b31c4eeb991d32d93b98c8079f0620460bd7f9f25baa7e30868735bb85d713333e055611d2a22443257559fe67255e818a0ec968e5e0c579" },
                { "cy", "2f47f8104155457b4697940804c10a5b550047ad51739885dff91480bf3c68caceaa280824064f7824a498d350420a91c9cae1acd9aa24c46c7fdd1e0530c138" },
                { "da", "4e8e82fcc3337e7fafa89c945a067b2a9056806e6f35d14afa55522ad9a2c27a1ed63f090103bf3cb9223132e44eed800d63c6e2681af8a2d4271edccf9c817f" },
                { "de", "4067494ee162a2cc848ba4d0b334f461d8bc7d0e0ed6e15a4d9f6a20eeacdd6e167c54abf8f17d0a34790488b6d0e5ef3b6816a3a155815389ee9cf5b28d999e" },
                { "dsb", "078517fe3f39a1832d1d53a998be4b4691bd2c2459538fda2e77e9688eb05f511d6b86b4663f30a0174d5047a5dd87b2bba60d7209baa5fa325bb3c9686d9e9f" },
                { "el", "98fd8c2ada7c7957567370390f69b76bf35d12c0cbdc537530c27f334824bb3f17b8977127f7371125e8ff72339a23822306af499e1df69d690e914d57485016" },
                { "en-CA", "99eac1aaf7dfc051f35efff871c552c93e138b155a55439d49bccd33dab837a90d3609db80bfc1784ee232b4bc93768798c362b475a3d0fdd9adf1502d5f43d6" },
                { "en-GB", "b08be1a4abd2fa7f7d2890186df77f791efa240fe5882ba2c87f973961f91e9a4f9a0bd982ba80b1ef158bbeb892c2ad5290bc77657f1ee73e639ba905784f2e" },
                { "en-US", "e3b4f9930cadb8262a6644574eed70ddd8ca4785d0842105db7bd09cb9f075737e12be1e1458e3b6931999bf3cfc11a48ca99ae689dd52711e8d85f90ea3d157" },
                { "eo", "505e30d7389e32be48bae39ab35967a208680afb7bf4b95a7cde92952580b7942104b43314ce596262e9eea4a66694642be65e7a065afcac510527edc0df90b1" },
                { "es-AR", "4aa64c26a893d90983c527d14c01bc8399b60b0b3f25988e3acffcb4711c7b1a9b752c44307dc69078ddc7f2b38db0844eeccfdf6d4a4e7ed47be406e7df2c42" },
                { "es-CL", "b011eb0a49d03ffafb7004e482854030cab7db38c8daed3959980a79c5606d801c4ce8d7aa2e90cf3b2549a7910c80dd4ee482e7646d5f2dc2ad2f1d376b7b46" },
                { "es-ES", "06be76f71e9a66f68bc0ef48b23174fe5d805c5ceb1a9a863996f3dd20c7ee1ec33c92312afd92271674bc31e0856b9dbdff2420ec230e7bbd309f4547fccde3" },
                { "es-MX", "e7214dc81ac03e2f34f56e86b3e6f0fcf48383a72936fd38a111eece2ab02d7df95143c2203c10d9723cd5eb202c772d9999423363f720ab4e58dc24d44f14a4" },
                { "et", "2d5360ce89ed1ef31ccbe02474251c2967dc89fb509d4eb8ae6e4f5205e8ce0a341364380a0cd7dc7a0aef592fa3479658fe59bde5827246a722e9a6b91eee88" },
                { "eu", "6ceea97bc44cfeb040b3bb4f724f6a9622ac959fc4b327ebbc9dab2b9d59602e5af125ee9fbd4ee4d989829225d090b28360342aa05a9f68bd8a5167162a81bb" },
                { "fa", "a285426b25c603f284a0143517d7de3dffea0349a91d197bbcaea3f9da2d3f21d6e04d1f19a1cf054372874657043f87c4bfca56bb3944c0627b7419d4accbe8" },
                { "ff", "170dceef1e8684f104825f539e0b45bf522ab32b066d3e61c42b97a5abe42b2ae48a31c17663473cf8619367bec19fc16b3f6e631bb9acd07777ebc2dee36d42" },
                { "fi", "ab328fa96a313225c39b8846f9f601ffefa8ae109bd3e075c84c7ae774048d1f909831e6d2f9e0162dbd7ec4ee1487e24a37379abe4ece2617af97bf9a445107" },
                { "fr", "352b8d10741610b71a88480e03eaa242ed4c9afbd6d4712feb29cba292a155574abac12ad1644b5b84f9411f756df3f5bf945f0f5a263ba1b0a5ca5e92f2cd68" },
                { "fy-NL", "5052abdbc0cc24f89c8995e83c6d6c7ee62415c00be93c40b2dd9ac41de26dda1287531c9acc81351bb44a5b2805c327f9750308f954c9fbb8b07daa2e57d92d" },
                { "ga-IE", "a592d995391915260238151546056987c9a9dd2e75dbb526f9766f7d18be0454b4440f9691cee8e1d1901c28c038dee691db17e431f9a1196b76517c2519caef" },
                { "gd", "6ba07fed3fa035d0a1a4848258db3293f25c421bf7725faa5436a69adb97ca9365163e10bd4c673d26f4f0ce64512ce45ff9e4a37d8c4001e3a0e315d0435b7d" },
                { "gl", "49bfc1efa72114d60c075d2f1c97cc35a99d0ac2249311a2dd4f2337e50cea413ba2795f3ac7531169b3d5db9d722d0a9f08e5278bdf165966a23240fa3fbe9c" },
                { "gn", "05f55383bc7b3588fdfd1b0490534a5cb28822202d4715fd547865a84ef992a1d152a2459f5ffd0dc39e9c5f7654ec394a9933f6fbc548c8d828eda841675668" },
                { "gu-IN", "bbd3357c2d9b98c398b617b28aacb72657efc4f6ab0888d05dfd8b19c6927987cf4ffbf7a76f94a1bc4f90ff610bb67a32f63fc528ed6e24bf8a906c17e29046" },
                { "he", "4218d744c9cb691734c8f4fba6c30d03486c29af0c4ae26837a026ce3c1a098ef91bbd139bdd979c2f533243dfb7675b134beed744cb0dcc279b9857d4be8652" },
                { "hi-IN", "4a72b5503cad6e9f531cd13bf9d11a06dd0be095b626fb15fe2abd406a1f02917a00b8aaa244c3bde2386edde780795bbb90e4391d7390345b89728933f89c1f" },
                { "hr", "c53c30317a1ab3e5c4d5cb80f462dff237f0e1faaa349c6a09d6594abc537f44ad5352b3953e696a7e47925aacc191f991be32ae687d6dba1168d881040a1879" },
                { "hsb", "25484e81343ab19b4be7145e45cf3528132e5335c775e1000d8d613e7d8ccf721484ebb72f253ee2bdb6a7d01df4df0c3aaf439a86d60c2c538242615bfb2267" },
                { "hu", "62154db992bb132989ac8877244fc637b7b6fc5bb6a612d0372593ac12e6ce73f61d8bcb1f615417d5cee3e1b937f780ebfbf89b7da8b6eef86f86288ddc628d" },
                { "hy-AM", "bd16a4450f2ba164fb9b70fd4d276dddfd44b733924b616e620e0785c726090b4abbb6d17b9f768019c28c7b4291059ce37f2ae108e79fb8910c6d4145b9aa94" },
                { "ia", "38c51bedf8706937dbabccd518dde1094ac44b16deb4453ad3d0f8e9c4cf4ee9e1f9d3d99f9588c95e87c793eb562a574654ef75453a53fcec120d750e8940dd" },
                { "id", "a890c5bd88c6e6fec770509d2c7ba319b2af4ee96965cc04d2d63e4dd5a51203b17b6e0a557872ee36da19351ec7f2f05ad1b168b8e8f9874d5cfda727268cca" },
                { "is", "f50b5e8cd87aa83fa7af184dbeea8990ad7a487fb8f99f447ea8ca3a6acbee0d454020e5ff74c5341c9c4219320d14b320245b135a51de918cee4cdb5e5f56d9" },
                { "it", "f99e62e37802e71b93a7bfde55a4274b73d52e81f00de0187c827900f1dff6311c70e3c19d9449016b6d1f2e84290c00ccdb99c7074f5899105615734af66da1" },
                { "ja", "75e8d44f9fe4e497c29a05e5adea2539524d8217cf1c381fb7865bf4312a120b63d1ae3d5841ad7c177062bddba103659982ebfa7abb76986358cd8d603de15f" },
                { "ka", "feb629b41d3a0a82ed43a21566c79d202650940f413b35db88ed041c343f0af8221d2e084319bdc2485f460085382cb1c2bd28c1e695e86c9b222f8f176052a5" },
                { "kab", "505798969ca0ca0b223ccf3e6200f21b79ff511a951b16d787996d34f5c7fb729ef162c4a07425df49ed8fd4f40ba37020e5754bf094686adcddccdddc208dbe" },
                { "kk", "5e16283bd5517be1641d1b1519efff302adce6ba1f6db694308e2f58202073c1db2adc5de5b3c407488b2f39c734a04c727c2baef760a549ec349927ef4ce37b" },
                { "km", "392e1cfdb2f7e136d02b3fd45875498e408fdbc3d1cf98c637f49fb68ef1ee836dae70dac17317e8eae970821b09b4a9ef8ef81554d6f11eace431d73373fdf4" },
                { "kn", "5f9c788b7bc84ab26a8a106a89336705e6d6389de11f132a79155cded9904a7f8b756967e39b68277e930e26471c4d3a22d104c5781222f197acb167e63af1a0" },
                { "ko", "eb49227cefc8de68ba4e11481695627c4ed4fb9ca53fcf69fb645f355633dfed7861d897ac258629293e11b0ffa3ef7a414de0bfc84b3f1f09c8173e1ab11049" },
                { "lij", "f736c6ee29a3a2417b64431d09759008ea5ff636406ed0d97b8378546c3f2e03b2dd4f591ab539a0cce04930979bd2a4d617eb2401b84b66cea9d24bc8afc453" },
                { "lt", "0af89121a4c84702209dae32dfcd580bd9833feadf509c7799f0342c851fcadc31d9441dcf9639065b22e2ed644af16af957aadf7cf5e1a99b260b2ffe618d89" },
                { "lv", "14664b9d34a2bcda8c5ed576121e8a84ac0b4f80457f9bb5a116f5fd5b383f72af6976232c32b4f7e9b2042cc330228423c601ea26e511871c72c368e65c164c" },
                { "mk", "db25cb6d9d24dd4119b31df73d6765b35c74fbd55b8a8e5a64bbcd50c8f90cd78a1ac87c43c4d7c06c81ff4bb8a68ff593a79ae4415f057ad36876121df7e6cf" },
                { "mr", "22895e86d8ad2d79736355ea778fc3c1ce74ba6fd44afc3b30f0480667ec967b7c542d65f59250143de8903331ca8c8b75bb96e866820b49ce3db3d50657ea1b" },
                { "ms", "bbdeeb860a2f2cc2f8f78423858d566d2ae31ae5476673e2b3f6e5be0d4065214b7ca7140ae25b8ccdce33d9c740e05592bd215a43e67773b7fc6348ff37ec2f" },
                { "my", "16fc58dbf27efd3ceb7dc09128b1a2156456175afd67738aefc8643afb3d12c13d1ebbb5cba2f82074bfd8bb7864e80b624def7842c34dc0574e1140758cc2af" },
                { "nb-NO", "57d2a21e19d6c084e51ead528a0bf524c37178ed9555bb8f79bbda2736e0d62e8da65c6d9a14cf6e277a895c548da9fd945eb0549227cc67cbc72967406c8429" },
                { "ne-NP", "5ba14fb874036b44f187254956117045b7c175eef47ed06887f0ece91d66428dfb9f6af4a1a7d43b10d6010e95cce3719131ed30dad234041a01110dbb3c546a" },
                { "nl", "0c9a89d652ad350de4d088abf3bbb9960311f5612489d3fbc1ba632669225c496ec701591023820638f0afbf7dc87b527cd10a435ea0e5015750e949f2756576" },
                { "nn-NO", "8a20659409af1643e1d8bf20fc61a84038e19c7d52509627e3adbc908466987c2e756a5754aabbe7cf612b8abba70478658bcfea2322a1622d0939182a3e84ab" },
                { "oc", "c3535761c50f332cc7eb437a9febf3aa1c1b2b4ac32f807c846b4cc98a314bc0a0a57097b03f9311f4c7f7ac773c37984b7b96dbd4eff26dcb2fbe509a2b7975" },
                { "pa-IN", "31bb36a4c35c1765bdb73c883032d563131ee3bb384b84047482e8940b873cc7e33c03000f9e895a62344cc8bd01f0b4939b26aec8353c2d33e871b659563b0e" },
                { "pl", "3b0fd625c62c3621d52c3427ce6efd091589906143285c6d90cbc3e282c49d54e1d3662222049923639769d9d752b59f5f713b2c8685ae0fffdafe4f55a16b67" },
                { "pt-BR", "958d2a2013f495ae88ea8fe3561b3dbee9e9ebba1e26b89f12e9c54a9aca0f9847591981c61bac70a32d27eb9de40e06043590ed7627cfe0f4ec50a0c73f0ec6" },
                { "pt-PT", "d637bb0fc35c72cd5a1eeeb5c4e385c6047db580014afae9a12bb5205a274670747bc2912b4a85607adb171acd4903094670a375458d2f99159e1d897ccfdd24" },
                { "rm", "524bad6cc96c3b6a194975abe17f5b056001d86ce38e179f55b676eec7c303e66ee2fbdcc3c87efdab10f59d0127fe04ec4a7f3d311e8b7dc4934a295bd5c2c9" },
                { "ro", "b0f150eaa694dfa468b2ca3d1d17f54341d1923f7bd3a05e6ed9e82d887328cdc58a28cb2799c5e4341d71c5960e7ad9c319786937d48fb4c67acad70a6783ef" },
                { "ru", "475d519cc845670f10f5496e6ef6353b879ea838056d48b666f9f9a0b876649c7cf44b566cdf5b353dd94d0e1e36074b3a6436f6edcb04b5e886fa802dc2cc33" },
                { "sco", "d50fba169f3cf48ade5d848bd76a7ba65e2e52896eb6fd330b8657620d32cb19659b4bfd6a069d54f9b733ece6cd40894636e69f0aaad15e30b5337c2d4f86e2" },
                { "si", "6a490a15207048f478c9763cbaf9f3146bfbf1370568e8c7152a7b2ada1888cbb1c7d49c8f941d740194a1472844d33c8494aea88aceffce593d937ecc7994d9" },
                { "sk", "01859e2f56858d431f35bcba8fbef873c2d3b274a018a632645c62a609a5db1602784edc8cc40774a1e831d338338032a40843ecb994fa38a11fa3fbf5eef0a4" },
                { "sl", "b14ff3f1b8ae5467ad974efeb079d09418c4821c00d8fa7e9da08a28d8cae0b2d1fd84ba185d40bd0b76e92e0f53ecf7cc7aae0b8f6686d4e04d6cfff91f9f34" },
                { "son", "f352472df96f59ac050463029a52b3d43021354b2be369c75a282f5c97d27f2226cd66534eb0413a7f829fb82f35c25d955563fa55a0bb45fa29ead9461b9029" },
                { "sq", "6eceeaf366d5757e1dc144f88ba0ebf81c482bfbc68445698f949f9a5283c26352767dbc0a41bc123ca63483473871a3e934df9e9d518b08bd0257969e78b05a" },
                { "sr", "e7fcb3e9fa829a11b5d3d793c3e176492c47615a9ad04708ee788a7f1a989f573cf03bbdcb4a6dcc9879710145f42665ce5ed91e9a829ea809d630cd6fa1af76" },
                { "sv-SE", "834e097f4ab500dc8e00ce52c7fda0391837db960a13d0eb098959e9fccba9f42b142534dbdf5c68f47b5bb558882a124ccc44830cc03009b4e832a753570410" },
                { "szl", "7c4cfa55024ed021a0cf7980847b64986ebef0e537e03ee3fa4a72fce0d382c5ef2e66ad84c1acd155e6c33f044de0710105a1cdfd7e85b10ef595e0eb756c22" },
                { "ta", "2aef697ce1f1c1e939c2743aacb38e18c83fb1870d213bbf775a349829d9e0108066eddd953671d8623d321ca8c2976989d65f45c39b7070499fa196a80f8423" },
                { "te", "3cfc247db2d0eeac9cff8706e1e9f70dcdc8a4b9f4ee4d3e27750ea7d8437b4566981fe0ab1df384c33452893769cd47f36212d5aeb315d12ba5b10dd342ad85" },
                { "th", "d9282512d15e573c12a9b5eb95d6a20a268cc896e930456a8708a61b5d68eab95ba1d059b3b49a60d26d88043d5c7520ea53ffebc75cc9b75c4973f3b066a122" },
                { "tl", "d479c16c422183bc540bdc069e2077296ab733094fed38952791b321bd6bee777bcf11cdc650c4a202d4716236a9b7922a554ce5069497e2f83f6124b6c8466b" },
                { "tr", "038e7eb5ec178d4f0fba3797fc22aa7b5ee47a127b218d0d20d797247eefe5574f861721f0af7f44cba548e65880ddec5f582f8024959b6c54e6d639efa601f3" },
                { "trs", "b66430464e60b65721ee821366b59446082e0ff46887697b46664a3ced14f761c69d3978f0af8fb4b062ba0aa5991151fa56b721410d3b71b2fd63c209ced173" },
                { "uk", "42a0c21b456d3e706873402c4b44ad3e1f10ea90ca095cd525961567530f64c909c9f392997090596c347a16533812a6542dcd297537af25c61b4790c96c28bc" },
                { "ur", "c090881d6c84b79b7217d153fd0cc4621be9ca19315ad47222e4dc71777e6d46c72a2c633ef14c7d6c4ac40ec86060bd901b577f71ead532cd6c38a859a77933" },
                { "uz", "370567c8eddaaf4e3701896c4a861997f75d6db2d6bcdc2c6f0bf218b9c575a9f0887ed660edc44b68835a353163350321bc7ec8fe05e75734810b9ec166e840" },
                { "vi", "1d74073764fa91f1c72ae0d5360458db99e3a17211b35149396846fbb1d7bba646e4f1eec4c1808f258aaa7474fd69414c72d9da16dbb47ee7d807eb3cecabe0" },
                { "xh", "1a0cdb23c753a1a8a6443e59c4644b39f9cc2321713628204368d1b3c9041e444732ca69171303fbc8232fc532e256709cf28fffd95ebcfaa1c10f3d9ddd2355" },
                { "zh-CN", "8b817bfe37a4c801f3e28e412434ebdf9d5ef6f49e4f90ced8b587489f413769fccd8e3cbcec93e0c614e0f35ea0af7a2ad47247735a283c39446db85fbd4c74" },
                { "zh-TW", "46cb987e26ceebdd22e1b40fb0b8939f074e8e5b1bb5a91205679df4cac700970f75118780bcb3a840838c7abae79836c84659debd4b7a9641d6e27e4af3a9a0" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/110.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d729ff10c3520db2e63cc7f2ade40e4b0a564d594e63524d5832b98c4a2c3a8089eb8dde80f42c505e3e178d4ed612e89f9139a1eeeefdd5e974b43b2d330512" },
                { "af", "c66fe8baf08714b11c63552ee1038f3a7e9fb495b81297e4fc90d9e8465d88413999e897dc56b5528d9a369f74239e8afb1d003e36876ac9890d8f6d744d3ba6" },
                { "an", "02d66d615341d18114c0f939d6700f03ac1220a53a3c7042e9c2e2bf48a96166ceeec7d00d6157e77de2b505c991ad458fb8ede3553c2e0902c146089d680dd1" },
                { "ar", "af1f913182cf2054639938f6c752d1ed3765dd07f65eb0cb06fc354bb0ad259bad63c1bef4ff23d480f03aec4ca156e8b95bb1836cf80d6a38084bad9f25fa5f" },
                { "ast", "63cc163eead3f0cf340eaf528a328cc6cbef1897d9178228df5bda05a5ef2a5b0ec73e9c21c2b449d3b10e1f2b65f0656732b09589c574d736221d3ba56d3d03" },
                { "az", "94810cbbf8bcf59dcee5b4fd0d6ec8b597fe326e36f14da01fb0409f6fab3e9a4c8263c11d8319b8d9a18e734aac179b574b3309e8cacff2cb61b42b7ef6ec39" },
                { "be", "57804d6f7f911f79dc077af18f3cec1950b99c0b16ae48374ed364f698494c7d832aff42742b0ef49642e930def4f1286f6e9a87963e8388158b32b5e4685f8d" },
                { "bg", "e54177cf5ef87df5f653a3a74442e46aec0e6b868e66723a656aa13d23dfa6598a6dbb95a23c1bfa81f61df75855f5fef48af43861aa17897cc23470501753ae" },
                { "bn", "37a32c70875fbc702790fcaca6185508b50da7565ef6eb1d745eacca34cbbab11cfed7bdc77882a365eaf7a5e1ac6950205843ee28c8999943e7f76837b673db" },
                { "br", "d7deaf8f5cc37bb0a7e6672e85eb509ee5036fcb5d94c8059e2f309d561a645f2557c8ed5ab576ed28cba814158b62b70951b556017b2c8cec06cd0524a3eacf" },
                { "bs", "b225fca89950622fd628be7ade9bcc49eb9f1dc3be9fb43524befa74953fe1ff23369fe6acbf20140a5087fcf94570c652408ca349a19765e8791b23cd5e9c2d" },
                { "ca", "58b6c85a47a314b3cd8be77a39858efa774802dcbb97b2a8ff28573ad4892ea709e77c9bdeb6a282fe4ca1d5f58178469f32ae9ce0da2a03f77f2916a21daea0" },
                { "cak", "e93e73ee2c5c67d7d9831391554d73fe7431099edb3124ddc548a4b8df4358ed1d98603c8ec3c0208926c6202822b242cbaa6038e7e32b81bd6e2b785fa04c04" },
                { "cs", "eb03465dff669d25b77418964b7c63551181d7123a7a77f1db1ff76fd75b91c8faaa1cfb2a14c8ec778e52f9ea84bd5962d5f4bb3291f255f0ec9d090ea46916" },
                { "cy", "6c02bdfcbab526ea1bcaab7f4963d68b1b987379c2c68fcdf728ff93fa56215e4ba1bc153986bbd38814e93c88d6380713e5770ec9f1860873d29044504e323d" },
                { "da", "0999056f210e3e22806b780698f4ced89ba970cf04b8336f0562e6505d89c8ff9f7a97eee778268e100f35f7d03d9ee9991ec6f54a7820ea12878ca92895f4f6" },
                { "de", "393e3362f828429d3dcff6a2a7f19e28b425d7226c190d3ae22232a974eeda4ea06a62f275f4191190bc8e512f1e5832d2af57ba28b9fcd90f1193dece0a30a2" },
                { "dsb", "d98b932fa008c0f809d58d186f11ab48768c83d05b478b5a790adc6f620758f350ccdcc6a4fc4db5f510a3189025087447facc90b1bee8e4158c17f585ab0bcc" },
                { "el", "cbf3feeef32a28e2b97e4a6dbc7852468e97230bd7305e18b809862e10440371c045c3fa64fe9b7a02eaf0c3d005c173c1f18978227fa6116a91a92e4c182bdf" },
                { "en-CA", "ff3d2006d7ebd2c3c883626a804629175d0046a60e1e66df4c18112ae1229ffb25f66cd33fb7e2155a2c04f7e69f7068f702b322eb2c484f3770ce7a67435daa" },
                { "en-GB", "fc9b59b833c47acbb4d0b110cd19ba73baf901bf5cb0bca7726d81a1bb9fc3024d7e5ff179f630e5d27aac5b13acd121be168e0bdbbf8cc78f0ec3bbc4315905" },
                { "en-US", "444cfdd5b7ba2e29ff634666c8af0fe1b2d3ca70a0dab547712b7b0f9686e29ef054b3aee2fc8b18d03d265623bb4bd3e42d819cedab3b3d1b2d71f9506e58d0" },
                { "eo", "7034776af37d37c039bc241d286850d8399734566e9569da82b083a89f8787d105ad90d7ec4a97c998fce81e4a7503b62d7dff724810b7afb224e83c93294487" },
                { "es-AR", "3bde462fa01a1e0835e64bd065d6e2c6c1c968ee06c1803adb64e9f708516be548cc7874f010f2766dbb2eab0c8690e54b0b06e56db5b841f0933a880aa72663" },
                { "es-CL", "f58740b1292ba02676763e2fea0c57fdf951c2351c6cc6c881896f4c46036af5db9ce6e8b3607caa1efff335ff50d63557fb300978cb37363f5bdf8a520e6c0f" },
                { "es-ES", "70a81e098957160c0d2f616ee59211adb639ca1882f909445863e8f93a61673022b3a1c53cec11a40ea76f6bec863d1ad7622196cb25912f6b81298e8f3efad4" },
                { "es-MX", "5391d2c5229d260f72d3f4c41c61c7c1bf6cadbda5b2c7b817f070cb10a133c69b3d70a95d9e7ba98e5b0c49b4fcc24e1ce500a84f58deac601da3bbd28b02a6" },
                { "et", "ac39ff3f35760371db5a2ecebb75ba2b72c3985a9073e431b2a92d5deffe8ed1e3dd639749aa64e9b00932108f3603b446c224772ce67ada71ebfc98d3f0e89b" },
                { "eu", "e4c0356cbee24d7af2d03fe737d564309ad8a07136a2c18ed24efb4aec2a6b1f713007c3c5dc681014c36ce6b541f57298514588f6627ff553390e92573df6a0" },
                { "fa", "60e8ae79173bc233a3744aca58295e135aab054000bc628bd448017fdfd359512f315c6fabfdec70cbabe7314ad33ac8e10ad53e44cc7151a2035544b543d622" },
                { "ff", "018a220297b7ae60737e400418a16531bd567e94274524ff5a4d7785336a9859743bd8006e08e96d826d7ecf4431960a92d89636a52a188cc37bab9017d946c8" },
                { "fi", "5d691935476d31e5332d40bdcf4d22dd0ad2f00f82dbfcfc22d3a85f5a07839e13a0bfa1169dbf7e2f1ead497558d67f79ed1db23b7d3728f4963b27de1da2af" },
                { "fr", "a400cb61cf9a0a499e2d92c1dca965a00547b0fa8f3b5323b4f3f01e348bbf416180d82c6829db4779a53c51866a65872a6913c41a822536bd210868abacc4f6" },
                { "fy-NL", "87df907f53a2f8c361f28f94c7a322f1bb96a6d00c1ddcf841f8aa2f0f87c54b83353cb218d4375db7dc0784e196fcf7a8642678d4fab75f0a5bc5af08954c3c" },
                { "ga-IE", "3b026716cc7dde38f81cb1aee86826c34f0fc68e4a57874a659a83bf1a2a98efe510560a5d0ce41f5a6f9acbc71fdf672cffe392b41eb7d76a5a8e56fbbdbffb" },
                { "gd", "3dc04c3cc55a46c407553bd4fc29f9481b20e7920a95a93fa3f8dd74c7b8140baa52d7f8fa30dbf1badc2b84208a5a5f54e28dd0f850046f89c3811a983b99ad" },
                { "gl", "48571b9eb1925d7cd4f61462320f493521e5f8d9487a869fc8602c70c1a0582d3fce2151b010ee2e5f128bc3999e274da2048712a9c449fcd925b65b6262a8be" },
                { "gn", "a8ed524d91b2c372a353933e656fd0377e4429e6f2868148c14539c77d081c6729434b42de51896dc275d8c7f0b7b4c420bbd6d18496de8aca03ee8254bc99e1" },
                { "gu-IN", "f673a902d3a5e0efc566a6357732f875b9ea471eb7a535238f12be20ae0454391fa7ce5ffff35e7c519bea76c34932c7a0011be1642e652b71c169c0cd2bb5ff" },
                { "he", "8cef153f06890935701becbb7a60af0c10d31342967ad68f079d81dd7048555be9725abbdbc2037f0a571cf316be5e9d69124f79245d8ba72f175fa828a0446c" },
                { "hi-IN", "894f47df10e13ef98947f6967405dfb70f6c254a5c507a733dac2e394af4b393bfafe783d9ef7f8837a7f54ea2a6b419dcd1941fccbda10bc86dd5b3d06d2e28" },
                { "hr", "18d1630a7358a0050c3c4d628269189f9976690b88a8509e0f02e4cf8ee4c26889d88d54b00bf456c1ccd04ffdf40d645807c72978d7f2318a29e171202b41d8" },
                { "hsb", "c4cb36cdcb05932cf3e8b0e7269dc782c56aa0fa76eb22444054df02c05570833e340ee869ce64e79ee41196cd9b83005a5bb30ab42b217b946663bad47f61f5" },
                { "hu", "5c6bb9f822de7c17a8c4e7285a0145b7cd12e7bc1b51dd12af79b40b13510502dddd5a969f26dfac1c3fe604733f1baf3a9a604911bcd7a24836b73a0ffb73f8" },
                { "hy-AM", "d2f5ea42e585ce30c9bfa5c2d22a7e5098548ab5cd9fcc93fb3f2b30ee5fdc4da832e1dae91f737f8598f7ca92e1b0769e596d35259a4385a1c0b11c1515421a" },
                { "ia", "7757f50bd1e1eb45cc40408051dbce9c7288df89f72ae6b86fab43dff769ea47673c45e060361af56ac850b993705592e50d4894d96820e968143093be732b5a" },
                { "id", "afb9f1ac84c780993c62d3162abb2107e2934f93bcc18ada1ac0201fd0fb8675066f9c7117bedb264b0b294b141b187720e99838ded0631ec13ecbb01fea9d79" },
                { "is", "074102a23ee9ea09b63b9a8d4d929e43f7fed5b3b1be0ae9f849f870e3d93d4e2af178df1335ce7ab0d2f8df4b4ffb3bab71fc373d3aa0c9b60fc30cd9595791" },
                { "it", "7bbd4c2ae76b0a29af1a97a4c40c6c639bdd5a081b4a9cb0ff64abeb7ccbcb6ef564de00372c55128b3eba64b348d47512d080b930f1982d68e319ecf4bcfb3a" },
                { "ja", "1c4d3f9447a91e4ddbfa2ab165577ac0707bc693c3024f8d3dd54bf11b70e468a721e13ec079260601c178d71461376b2a3d68f7793320a5438a087fe81ff4de" },
                { "ka", "2ae9611a50e9ac902cda907bcaf7f99e515f7470ff11b651c63e0e71c4a378ae2ed7fc1bcf87c76ec11a18d279bb00d1ff2e89be29a177dead982cf76ad462be" },
                { "kab", "ccfbb46b5c53718581cb32f5938da3ed03097749a73ed36268704bb795ac18f03ec0efcbea44efb81517148a72220d208ce7dcd19e9a45d2967293ff38866b53" },
                { "kk", "416c41ca629357804a157524e920d07799fb33d4b0ab299c97c87057ba036646d22b0c8752f6724a81cb6f60defbe919c14f62f72a4b600fb346de3720420bc9" },
                { "km", "63f06d1b1783fcda2a1447b30115dea6f470057f08c99d3f0b489b41061527b146ec681a564eab72342c2c71a4c1232658c3244682b19c8190d16b308f02462c" },
                { "kn", "4a4a25d9c5f1ccb0e0e2d1f5c12ac057824ce8309d517e58a76a82e1c8ca0a756e2524084a50a6ef29363a5ce7d531b0b562c051cc99ec405d5e5832ca12ca7e" },
                { "ko", "2e9962234565152ba3a914824fe00c062a408e99d0ac9691681699d15d209ac6f0e61fcb9b01a0d9ff255853ae238dcdede3fe67f900225c12521a6366416f98" },
                { "lij", "704f3371b84b4ba3b2fbfe78d56f00b9c86f29d320f4fff316fcaaa7451e3ffcf86808a3c592fd51ff2f3a73805d592870eacd4c6851a1cb9327abd02b7bd40e" },
                { "lt", "279674b697421105488cdc5b7cc85156a9b8d686165b27e6fea60b456f2307aa5a2e3a57e3557da7d87a56ada2e1350ef65359e7c7863112b2334372573d2275" },
                { "lv", "c1e81605b8be9676bda180dcecac5ed7ae2beed762915984abe604d9bb50996e214cae32459e9091c218a09742948f6a8da9f2593815382b51ed967f980aa0ba" },
                { "mk", "6117cb2f78d745c697d535dd4c30c97a2bf596ceeb365d5e39899ef871580ca48b4e4aa727a03be99b346b0ddec5039cfecc553768ec78e0943db46e684dc0a4" },
                { "mr", "9895d36855be1111b3e30ab7e25d439249bb67eb3dc78766abccc1ce68dedd0ad3dfac69fae16e34a1c991b58006ade54ea3e32c8ecee8c18f7e7e270e97605d" },
                { "ms", "53c0c16f58f50328deb81bc04d8572af5820f68df030f94f0bd9915406a083f017b7211ef30910b09c17a59035303ce41795f7463b8655998aa959773e4786f7" },
                { "my", "768324a51db1bdff04ff89331929dd763186064d3a332100947c67ad9ccd2168647c9983f2504b9017d3d538b0fbc0c0f76b45ab56968b6112c233f674e140b7" },
                { "nb-NO", "8c427e5f2ccdc6f1714dbb22d675d279b1298e5db6a7b4ed13bfd9bfe96bfc6ae3be9b4a858354585abb1dfdba1966c09b35b9fcd3119e757d7e90def07c6fae" },
                { "ne-NP", "54aa100c55b683fa92ccb77b67bae72bb7b35a05d7a3753e05d5f44d0fcaf4e9ee510f58b980efa7282bb728a5db73a51b77a88cd863790e47ca8bcfa5fd26a3" },
                { "nl", "066f75aced3388933cd2e09c6b7e970526157e599c02e2cf4ee6fd42100864d60fabb0bf4a794f54ebc2ba1c5dcac2588df827871ad85551bf72af19a54da5f5" },
                { "nn-NO", "90dfec17c26c01910c3c955dc052ba982e0bc4c1ad99d303b37ce0d4857fd3280794972db445f7803b059192865ffc4d5a0d2e9ee1f68071887185258f89b4b7" },
                { "oc", "18be2374ec00e67710aea1e1fd55a4da3206022c14e94e8d0a0fee881bcde83b66ebf9c1fa1c1b58749f3fb7446cf77f41f7f53dc2b50891b5a4269c95d65a86" },
                { "pa-IN", "b5c7e283e067488959ba3e71c86db232d04f7a37c73dca991fbc66fc228680de5752f88d340181be3d230d28e44d2d19f27f27a6eb1c6fbbd1b2a53ec3101cc3" },
                { "pl", "c4bfdd6c76deea6d140eafaa3d8df9fbd0709f3143ad4d8f37d75142ff85c3daf3637b6ad709b07eb4ca7a177a239d4ef24fbb6c0444f45c3ab3031f821fd4ad" },
                { "pt-BR", "2c551faf1c696b0bcdb3729a774c6bc80d9d4a073ed4e4db2809ebb1645f48adf20c88f398854741fa9f806430b5e6f5bf5969cb008a1241eaeb062c321bb292" },
                { "pt-PT", "ddb2d706dd8341b7173cda62b43cb0df09bb28bc29de452afca5d8235da5aef794a8d71b9a54d6491f524f90316e29ab67b1c95c8157d0307aee3ae3f547faff" },
                { "rm", "9750935417cc45a286e87086c1d2085025b716db9389b96ceb6960e24fc4da3c96b2d43a4431b83a96b314139dfe714a9cf681702064719e3bc5f8af966c204d" },
                { "ro", "df773dfc1bdd265ab6233e5c37c15095ff9e25a3ef5230e194a7155890a031136e6cb0e61b75ce58e4385016162b1e1706c51cdecddfde0c42cfa5d3d036bf76" },
                { "ru", "8228caf275b429b8025e54e9f1913bb173389f98ea007393c1b6d6fe9288961c8ba64ae30b98eff5a28ac5979e67fd39543b71ddaef29e1543e5b675e67dbe39" },
                { "sco", "ec2b72e358be0d4c59bec78ed3e58972e5654a1aad5774dafffafd5a33ddcc147ab8addbd018ab4a350d556be54fae52298caf6e5b0785252b95e790ee768d7d" },
                { "si", "2f8e85c8f0d63d1a809042caa65b7406d3d90d9997b04d86423a3d1bbf11f0db62544ca3c0af6b8659958af6d32159a704261d41e6810d78704f3e6757e8f6e6" },
                { "sk", "1d5b297f2d1cddc8bc8d92c9e33a4d7e50892411cd11725adc54ad0006d8c3c58f19fda945b270776976191f7aa30c8bbb55dcc06bfb332d79973127d250b613" },
                { "sl", "8544cfdda893e6fabbb850d5eb95c955b65a433650edc85d5ad9aee841149d317f970b7cf2c083b7a22b21e2c6eb57c093e9a56dc002df9826b1e8088125d409" },
                { "son", "8088afefbf6ee15c7bbe6af307957deacc74d71158aa13505864c5fa527559f055fdd8f8314edc188eaa2dae055a47dc09571eb55e20d8a73152388f1517d19b" },
                { "sq", "7b6f7a76ff8380fd7820f3c6b9be237ae092fda084b79950f9694079734ca35129273d8308e8c43c963d630f1a61b9d702ddf2a766e85ae385f86136df6fdc70" },
                { "sr", "5459e49d9202fbdafb9f449e850be1ae9fade09dbd992f875f30ad198163332f136d58462aa83d853a84ff479a21f8cc0222f456ae2abe3158cc1413f792f9a9" },
                { "sv-SE", "7aa6c119539ed5c564577ae9b456cdce366693907e802bca2360d2b482bc4c808e2fca3682c570eb1c6674d1fae7466e14324a8a52a8e90057739ee29c02c558" },
                { "szl", "716b5995ac5abbc5ee4af4b0d92b9f388e0e87cbdadb589173bd76710a6884c191ad5e1045497bbfbdb5e90e3fbf4f2f50de9336f49b4aef916ef642bd5de291" },
                { "ta", "9b68251800e7eb049c6ea1b0f83e423ed75d6674d0970377eefb36646140971f2d98ba39d3a89cac76531aca146d4e0193a16a96445da83cfaadfc9429fc01c5" },
                { "te", "c23f47cea822a83e551681fd289dc9d69bda5c6344804df7cbcf07be64de9eac69818344dfca9fb1536515bd37a29139a921f2bee246a6795392473aefe457fd" },
                { "th", "5e4b03eca6dfc823ee0d560151a790c2f5875985662e12ae9311eda4612cceba487c06e61c598a0e7913cad0fc000c868db29a293b4cb9f7f5005df3a912c06d" },
                { "tl", "8237d037dc338b383f2a27b4f2419e3db371cfe6e8d523355872c5d23e735589bb5ac606c2973c2f77ac7b88eae357a1a73ca74ae73900e870ec79f5bae8c442" },
                { "tr", "d871f867c850a4da49353cbf27eedba1aecc26e6c99c7aa0088e0b0ce159b800732551b5c0313e816170ca381237df8e9db5393b2fc65bedd91ca0e1441ab51c" },
                { "trs", "9863ed304196abc9138a66c809fe97305a1782e3593ece588faf6021b9b74ecd9783a81945c02d8a8d346b5f64acb113caff3723fbafe7d3c917f0775195f530" },
                { "uk", "981a0031b42ebc7c3ff4c86a8329b92525f259f72f1c923dbd4648863ee0327949acd295d3f1df0f80b7fe8c5ca9c880ae40d8800191db300e75f57f66aff671" },
                { "ur", "79eeeabcbee34bea03e22b6482f65b7b05baf497940143f5dfea7d26c75a4a88419ff20d1151ec8455df32b0d167c53051b0267e478dc25d6767b7abedfcfc9f" },
                { "uz", "0e50369fcea3c204365761617dad9bf08892c33ee109dd65ba757b662fbcf149a0fe1a0d1777627229bf5bae1e4c6459ee046f2382a905f0e14ec9ac08349111" },
                { "vi", "50e85f6cdd399c17805bf3382f898c693324ee0e78c48bc3788900e08162cf5c0876e1d6bad60b8fb2d8b09ac5c9289f8396ab67244b7c2dce51892a90565682" },
                { "xh", "65d61dc8b9cf31b7835cf0f60fd8eeba93f8a2108d771310c2418337e37a699b851c2ac7e376977fa931699c65c0e45eb40fc02d82a711a95b62b06e3155de90" },
                { "zh-CN", "c4884965e7cea19c67af49d7b5c31c9a62f630a005336fdcb9aeff0f2a59fc59b9d32ee86535bcda91405a98b9c8925f6768b750be99414d0ae1f9e200c9655e" },
                { "zh-TW", "95279c85ac1d30eee8b3cf14a7752ec500819a60f035bec767a515d68d0a6c3082327f5683f2073fd903e5ffd16b9e79fdb9d71ec329efb0eecc236d6c6c8ca3" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "110.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
