﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.3.2/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "344c5162090da00a3d0aff96ea6475639e54898faa689214ff7890002fe27c9349663174afe6f72b4881b1e99685353a044070e23720e8d6e8b5b2f1ce110a44" },
                { "ar", "740edb341202f6fa85037ad7bdd7b587fa03186ae575933d6b2f5b589602f20e6c709cd8b273c0e23032675ad3249b4da27c1e463b4c6a47c23b318cac04aaa4" },
                { "ast", "343cdae3094e225524122830b97cc944722328c4ec928aa7b7229755f6d45a477b2466ca889b6986d88f774fdae6cdd0ee65cf2437e52f05704a27b1217a2407" },
                { "be", "b38b0703df5117b581bd914f3fa8391bbddb3a8bb2b9b73272967b59968704b059a2ed2bd9f6f9407744262bb478200be0a8505466781808a44e95ccfc8b7bed" },
                { "bg", "dffe1c6a2bc0ce5810d2c62fbd34f2e9ee1123c9c309eae9f37706188d54b781838a912abc003252f8178e030f91ece31e53e925eb18dadbef9ee6bb5fbcf938" },
                { "br", "2b080ad70832f866265ab40dd0d29f12f7913ccb109d15eb0be7c1b2e570a129328368bc84f60825a94f7bc4642bf6dd767825a801fa6c067d600536740f4244" },
                { "ca", "070158b5b1515c5f503618d33dbe96353e1a02e6876c8b08e5e1ced4e3cf153aea402ef426b896ae0618487c4daa61d401bbc7d0671f8cfa07f4674660a10ea7" },
                { "cak", "e6ff34c01438b930429f657f1cf0e79ed488f91588541f705b4b5e596a15380b21ecda4d1487ccfe4e1a4a21c54ccf26503a7de6aeaedf5b2400e19c92fbdf4a" },
                { "cs", "36dc320b683572ae3ec28653b476cd2e55de524c79810abba8718f8a242d1deb116bd6c35ebc0e743d0040beff7c97c67c67149eb338fd38977ed83b00c06b20" },
                { "cy", "8b93fd3c377780e17db3324ecf77ee6f0f6a78b201e94cb0e96450e2666aeaa89a41c6a35336491bef5967d772011bd4478b2d0a3b6b7720de682b74db9da42e" },
                { "da", "7d9a4c9fdca9811ec9cdcc9de4a065e4ae7f85175acb16285678b00075863441235ba3332ab487baa5ac7a468598abfec1e36d6b58d43851dc2942ee7cab0a99" },
                { "de", "84b212d22f0504be76dd4ffe1989d9e0bbcea5adbe884940a870221f9b9ba465973f93948b3b6176eb415df5ebaf26aa4eb77c2c6fe83964ee2936a634c8b3ec" },
                { "dsb", "0a50f6b10c99461fc06618a8c7b40391e6da7174218ad688c40f28cbef7dd5cb651dca40777c0f754b97e50d10ba39a1f002a26f64892454ce0721725f58c441" },
                { "el", "d8e7150aeed00d93c4a1de8fd30eabf062ef4c70897d54b86bc5fba7fa12a2a1b38194f3038fec6fd745f48ae44c23739138457c5f29e5a71a94144092680fbe" },
                { "en-CA", "393ff988559ec177447860af5b61560de0f76949f3148f18fadae0e40171db98480adad241a9d955800f5524caabd0d8b32a33f9a86d39a4276b88e2d76912a6" },
                { "en-GB", "2722261cf3aa9cada8ab63886d2d370e4949b8c1a66a75b0ea61218fc9881fd937086778d5e71167b806e2176b5b4dc9bdcdd2a8d24914239880e6a8efe899a5" },
                { "en-US", "265f7be32cf0c205dd41beed1d3c7a33dd27dbc72710f64b0af0bae9799a83a3aaed8c004f98a52e25baeedd4cb6053e8771c2432305df5822a45a369e32c123" },
                { "es-AR", "4b2a04ada4511207616133469aa793bce40396e67be31a1c0c0867c6c7af40aa206b47234efc921598a06ed990323d5ce6040181ec8f5c5a74dc4aa5cd6e0231" },
                { "es-ES", "ab273cdcfef462904434c51a6be40c23315a729f5169bcbe0330c534a19f2b153095508902cfd32cb41b8bfce8e8d950a78a2c3291c905e0e4fbef33eef04fe3" },
                { "et", "9755f5cc90641b3651adc49b6b85b4c2417fead0998d2d28bb87760ebd5729424b247bd11cff1e65d13d7ab945d6414559d2f48935a956e02f195d57c14361fd" },
                { "eu", "33df49a1aa7c6d75378a9355bdf6cf5dca4e6f9dc0c9dbf30f99b49e3170ee1f389d1c8aa0764325734252ff14de25c43cbb57d1d9090815831a4c32cddc49b3" },
                { "fi", "c9a5cd4e304296412526753018e87790901c3f07c813326dd36e6f2acc8eb7901b76ed8c837c2d11047b99ff31883984fe01cc2d7fb4328ebcb55e79fee0cbb0" },
                { "fr", "b05a785f196e0739fab73f1be2c1c3c08a9e6de799b4e7db9d437b846253bd590ca364a6579d4d488e8c42d2553deb7784cdba1555cae36a607013200e5edbd1" },
                { "fy-NL", "0acadc3f6d9920d5cf0d9148e0d1c119d80c0e212e3c92051115b1dea0db500ff1acf86220d0981929aa14cd684eaa16396066d2a20f56578090f5f212135cc4" },
                { "ga-IE", "bed5de3dd1b5ead2d6edb3363ad45de2a8c37c5efd1c276641559502498079c6b39b4a2292386cac40f68360364622a3a6d9dce7f497dee40fad80f6da93c455" },
                { "gd", "362a09ec06b5a11a0c0073d2cd4623891ecec3a21d267adec4f66ee85631e1332d2f26ea8ba59470abe6696e347069634ae82aad4cd6212336e203f8fbe70f34" },
                { "gl", "a9d39d8937fc69ecfd32ab4212dc7489cde3ed5945e3499dd7c0543715997d07727b2ef358a24952e4f4f4202b554b344348c590aba40e543145c69a900b9a6e" },
                { "he", "27a9f315d26bc8a926fd051f1e528643498cae388c4af435fb4f6d2f0ee62e01c3237cb5db5af632622595c627b2ed0e29e30a21627203806b4df1bb4105f525" },
                { "hr", "d04a7e419c34ebf6fa3773ec7a87b073628b6b97d86e8fde206faf0185dc23a866243bbaf31d4943e5a7a9bf1bea638bc6a89ec62b8038bd0c7eab2a5c746b15" },
                { "hsb", "8739db56748c9579def017851a6cb13d868e81328c486d88a3d7a14627e3bdaef3a907beb7ec8dfe0b82c4ad80c5f1944e2a18e9b6cad08ddbd604959c05a979" },
                { "hu", "331c40082ba2fb2c2748eecfbf9f3a8bc3f6b575aa75b66679d43fc63b8387b38425670620b7bb1d904be3d9e9ce92b3ffb144633264f968ca73b7709f7d6a85" },
                { "hy-AM", "86109101b4ec8975270b00f57e6f77944b1ac81c9d524ca512998f568dfb427ead9ef1d2dc5adda8845555a579a030d2a433541c35be65a921787d312945aa38" },
                { "id", "05b558db7fc6d6f85bad4976f622c1725697f42b160f3f8548bc86650b7238a413d21e77255d8555600e1373bbb862f3080b6a111b167b109adf27a9552e2f59" },
                { "is", "c469c44e33e7c0e7dfc01e70bd2b9e818a8c429728a90624909b67d53e3b9250115808a38cf52694d039ae8c83224417380b0b54981565b9ce93890fee2bda8c" },
                { "it", "374ebcf44cd35a97d2c11eac8bdcffb9ca5741454b17b411f7aad0cf8a23da8f4b933f0b9d6a9073e6d114aae8c395f205d44d1bab39b12b1d853f858b00b310" },
                { "ja", "1b230afccf05ff7d7e0febdcbe32353aabd4d43a658564b13730323b2db3628b0ef8aabf4c8f039c66587eecc80f7b3a081af62de96e457b2cd98bc23d23c67f" },
                { "ka", "cfabbf85dd07535ca7921233742b2239238bf4cdc4cd859176253657a105e41bda8effe5151c3987f3328ff7a20e2400fcf912ab6045ed52caa60aefc4fde19f" },
                { "kab", "78e57fa53b02a178b3a1bf4e5f894122019bb710fba4bce5e370b39ad17643bf9380db057353263e1ee653793ae1d03e2f76d12027d6ae2a52f0ac68486ac913" },
                { "kk", "d54b13282a7134b28066337d80aea5fd256d9ae57c6ddd20d59f3d3466ddc0087f4590bcb57718a3cd7ed745915c3e7fec08419082c3bdbd1b50695c7afe259e" },
                { "ko", "5c7dbb84a6d0f5c7e8a20e7399cec9da00b64a8b466333ebf0a90677cdf6bc97f8d2382233fa48f6026b32a34ce4a20eb0ee0cd02a8bc11576406a0838e5318e" },
                { "lt", "0e413ed50bd53dc03ee6195f9422bb7adab805325f2162c4a44e17c50fa891593e427a9e4cc293879c32009c5a16ac11c05f28fe03708006fcc0570d42cd2897" },
                { "lv", "bcbde35929bba33851884a3e9d5386e215b744f2bdc08a85be9506232ccbd32cc09847acfe30c1c283e0addcfee923d5a5e8bebf6c7746a6ac30aae8bdb750b2" },
                { "ms", "9de5bd6eab4469c2c12a6ac86b2cdb33831b62cf3df72c12f1cfa306397fd615688bbe780147d9d03df301dc7be55aa0480ea0f134cf72666097f69ce77f27b4" },
                { "nb-NO", "38c112036c986d54649b063e810fcdc4ee79af7964a7458e7cb7b9e90e6f408b645c448652418b87f7b80ebdddeb7b6cf5097e6b17bd32ff7b94a3524f41eee6" },
                { "nl", "62b6cd1a7ee1770977623abd3445e32745b610f2309d501b3b875d2bae836a5ff9205a1e4143fd3e4090db201080f0b1fe24623f1d6151afc1cce6e439cc4779" },
                { "nn-NO", "d126d4d3aebd7e46b647f84ad46bc65ff715b8273b7c17ad83f35c29a596da1b068079e540ff3d2eee057b19e41568d36a70693199ede8834fff2febbccd886b" },
                { "pa-IN", "47dc9c13c0ee8ff9eb35a99969b67c8b041e4349fc772f5daf415e00c8a50824563ff2823eb2f7af7ce1dc1c24743bbfac5eb756d37edac37368b53e000c8b88" },
                { "pl", "645f89d319ca298c7bce93ec917a4278e6a6fbd8946a5e6349013508cf52b470d5b1e94516a9ccbffba866c6766f798974fade8136616ec2b414f1a448f39476" },
                { "pt-BR", "eadd4758cf320e11226c4c5bef7011f901ae6c4b5e1f163fea056374cae2ffb198baaae56966c0098b761df1899f5fccbc58db79ddeb86e16abeef1d3d7e594b" },
                { "pt-PT", "563117225a813dd83b9dc83fa813349248aeda3121159e13fd10dca234f02799a1c894ba2a1c35e24bf77b0e1586f3402163a45398424c18e7a28e3498977f2f" },
                { "rm", "f94a77a6eec0e817b651a1e2e87d92de565f91a9bbb9f350c023e1353f9e297ee9b00ac09a0e5f2480d9b497f9b6dd4bdb04768259a820e4b37c87e4735846b7" },
                { "ro", "c91fa1604ac56b7af5954f1733f7404cc924bfd3703ee8da772e12ba2f55a199ff0f8b40e9b059a1959500cafc80d9e2c2d61f27beb16cb8f681ef86bec1946e" },
                { "ru", "8b3e4cd5ab0b9a5957511bf707c4c3f50750fae250f95c1287508f88ea4f79a2be344cfa990d898d7497b9be83311d347454f703bf2e7368860bad88a8f6423b" },
                { "sk", "2b6a958dcc85829c776d4485a738912adbe843783d2ffab90da0526fa6e840e9f100b57f4b4027d9d69bcc56f1326e6c60d7bd8aac002e3333a99a07079ea7cc" },
                { "sl", "d03c4accd06ab6a1f33c58f0969214588457775c4f6b0f074edc961398a58f589f72525e730ff6494bceec63a49de7a128a033f944e0acaa1a37c6ecfb6f3b9b" },
                { "sq", "269117bcf49e97aa185ff92b217376da6e551a5fb3ea30bce1526d12629d97cb4f677dc1f75ca219a2ad9fc1669d6ba1e65eafab70ed33a7c221f692112a47ba" },
                { "sr", "d9a1b394f2e7f46f7b022c17bd201689758a5eac642f6be71c8e48d9725b6ee40606390a2dd6597e539bafc3993db5bc456465351bfe7ad2f94d1aaec922232d" },
                { "sv-SE", "1c783cb03d1c4d5afe36e340fca02269f1383a7b3b69cbc6907b4aaf493fbf010109fa0a00c7fc3810da8de9180d4ac3cd689f19955358b9413c7cd9e758214b" },
                { "th", "d5a82d1c6c1dd91669593cebad8655c24208ffb0e015bd00edf6a0ef8e4c31923e5413a27ead649c33b07e8886bb3d241fd57628615f4b02a86d718acf992f0e" },
                { "tr", "9e168538ee7c300e7c618db2875937483354dff628f595b179b2b40f27b0f66412c709b49146c0f57b4b997c515c1cd550a6b75be7ac861023638c9a58427823" },
                { "uk", "053216fde1f0a1dca258a7160b016d276e089844676fb2bcd93cbdbade7ab82749a08cb160ee9e89c7f2bd93981f5503349b21b05915f276771b32542807b1f2" },
                { "uz", "5b0532786a708217072a706d806a4840bcdd9ba7a584984924bb652bc823c3b5c15ee125182256a54c01b50b8bb642f09d5fc5cbc066f852c11b45d4707e26d2" },
                { "vi", "8dfda052d0a1041f77b6f0bd0e39a14c25debb2d0339376b6e5e218183f7517520ad19db25a845ee582c62bec7451f8cac2d7d9a9e5cd58131e0ffe624093c4b" },
                { "zh-CN", "9d30566640d01b440a25dea5dc000f47f6e279a59ede2ac9ebee87f43965a27a7caa2d5e352fe072f4d9551744acfece275cb689dcf9fd0124edbf06509d54c0" },
                { "zh-TW", "758edceb608cf5c7518ea734cafe76245c9b6e7cefe6b138e7a36e08d3e63d110b560e9a5ce25f73fc6ddc0efc9603c23a80d6bca4a27ddfd7461d47e5a328ac" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.3.2/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "4891f48ba341d6229cedaccb3e6cc02a5b7622e6e91f5757a7f61a566d423a4d580fe86c14a199187679b02c4b9fee400845e291103fc7925bcf61dbf5a70c4b" },
                { "ar", "ac1acad347742fbbd5b88b0fc8f4ecc163ab0c03a6fbdf76b6d43837f819bf259f46779abf86fb435070e64da2ac40c71691a04c44768420f1b2f8984a205e5f" },
                { "ast", "0acbaef29b2b7ea50f28ef7d555ca2add661aa5cb801082f1d12259fd0c369e468422a0dd6c95b2943910c39bc8c4506c361abc56f803a0070346711efebc0be" },
                { "be", "0e9da607c6409eb5c0494677dfa544af948bf66d4c9eada33d011bc8fb32e1c852bc4fa9b6098e91d029f887d7828aa3e0126d4e33f760c38894b65457759d76" },
                { "bg", "cab511996d80f2bdcde24daf65a484ad13d8d249990c8612bd247289e76228d384a2c7feaf18578235a4e6861c7af3847b5679d91ad76b1c802655a0d5d2dd9e" },
                { "br", "f159e3c01f15df0f467396a35601f9e6c34da405532ac0f620432d09b9953e264226bc2dc3462847941ef88e2bf62edf8ec1bfc27eacca957e4777d31b6b88cc" },
                { "ca", "52e51725f7944e8fe4684eaff14d36b01ec1d654eef6eedf8c9cc9b9e738cff0b711ffc518ca741708880933e53fd7a18cc99032260e4942b53184f1bc85a108" },
                { "cak", "2b7ffb8bfcc98195e9033ed2928966a8bbf1d848fdbb966db29938e2af4cf5ee4dd83bf397a41c9315343437d8e8c0c270873c8d82072e8fad8ec1035e54ca45" },
                { "cs", "6d13ba85b0cd84e3c13674821f9e8cb8f3d09cefc8fc2171d74d3568fa3667043003cec6cef3461f449b78a28db6f03349ed3d66f57447276c491aed6e9859fe" },
                { "cy", "8aa9305495a1fb1c68ae9a2aed80d25242e0d92e479a3c04fc81910efac04446a06851048451a46da0ee39335c0a3d898806ebcac63893a59683d21fa260879b" },
                { "da", "2d30dc45de15319954ba46326bc0a763871a9f4f2ef5541e49dd287a020f36e8654950d12314055fb7bc49f16d0de82653e8444ab1ea077e4246bab5ced81396" },
                { "de", "c87cb4e490f34c3fbbe4038f284079007933ec39292fe555ea752fe4220d11be406e00accb20fe84c5d43eed02988a14c7a31cdee97ae4b3b2e4665026dd4e4b" },
                { "dsb", "c453d4dfcfe60c69e76751538ebb9e65b99123b12f2704e1fb9ff25c062bdaeb2e35254c50be8b2f39eb134f7a53008106aafe6129ce9577743bbce605a5631a" },
                { "el", "23e6b4b55f6eaa2fcce0f943c5eb1f611a2578594394771b2072cd80aca20878e0483cb5e2c80ac509351d4d299199c3d4e307de8e995869919d6c676b5b4e06" },
                { "en-CA", "17217a40fb98e1af2b853d1d89c64a6198522a13b7f536dbe2e70aa41db77f6a0276b9c7f91884d9688294331835ba5167ab385959faeb98c2e941e494a737b1" },
                { "en-GB", "158d937764b4fb1f678c195944d4c5e7fa4942ee03127aff45ca9dca0b066272ae0cae7d7876284c13d6eb8f0a2b2f3da3d71804a84a547471b3c6dec4c2fb52" },
                { "en-US", "ec45d6c0745af927c5e07d25e8e5a6b8d56c4a5f6dc64127028b561bf56a44846ddeb257f9d15739a8170d0557a86b7b9a40c0ded8fd2780722882159ac19f2e" },
                { "es-AR", "f8bef0fe19aedeb9d10fc7416670b3f12361d5a5d25e62882735223cd21f10c12151561dcdd75c43c5726fe525f548d1003ba6fef0e5a095d3d704a916d9ee51" },
                { "es-ES", "dc079da82ecbdfdd15cb264cbbecdf61d30defc998ca2e95b300cda69a458cf3b779f947786b8316781b083bbd9ecbe574311d277bbd03a6093b5da01ba5a4c0" },
                { "et", "d2296da75c9af544287420b95a1e60789850ddd6543e3c18b6363b2672aacfc1acf7897e37762579206467725ac53a20a791af72b6560127a44d9e012e321832" },
                { "eu", "d61012aa1a20583b361871e0d885f1bfef2032ded2ba9ee50069b8c9d7bb6dc84ecfad523dd216bcb91aac22330cf27018d9305f787c1251fa50d7f915fd2839" },
                { "fi", "bc62357c17f96b43b415528eaa314b3558fcda02fc8bde0c26b1f1b809cc5945d1ca739d02df31d31d94b9297fb7b83a501579c1a43301ef56be61cd9eb15138" },
                { "fr", "04eaec3bfe30ce32fa6e89992b2a1e564523c916fa1a298ecaea18541aab84400c8cebb2dbd0d06668d2d21bab9a6e5e7dad2a43c8130fda020352e7e437abf9" },
                { "fy-NL", "5b6e300fbd54949ecc5cd2f5afdb719561d55133e792e6fc4acdf7ff91d82b8b6bbfdb8c5dfd2488d3a1e0df56714edfa554ee193cbb4ec29a10eb9bff90f1ae" },
                { "ga-IE", "9a1ee4e7259b56a6b35da812baf75a3f5fdc920cb1081312f7cedd16c326a77ec2c8ca34b77022d3ee3445617908a7067aa4ea8d1ca47225e45075b38fc3cabc" },
                { "gd", "213254fdaddfba19f041d30be87bee5cbdbc536ff0c46dacfb7a745fd3ff1ff019f6431d44777ab70eb672b7dd3f9181fbab8bd3a95ae324383de3548f9c6aa6" },
                { "gl", "a6ff2a1478da537b4568f27cabbdbf4ba9e096fceab9f680879f9ae0ce23f5a081488e9a5b4a1c6e86263b9215f4571aa5d4654b5cf01e3eb2c7feaee8407a29" },
                { "he", "7ce9400eb99f5a9d6287fd0422a6fce923672117b56509ade2e3a37b896f41024890aec28f154d262e9d11463e459d966fc8347775b848421561d48bad6d214e" },
                { "hr", "927110e039a73c804870c3295b10758967cd6045169e9c9861664429f0a43ef1b4382a066183d263783478a4a7331bcd4e76a4ce09fd64ebd7766097ecc62248" },
                { "hsb", "49cf1faabb5d0b43baa8945362d71a13b161e57dcd1cab34f4cb52483874c033737a8df3be746b091b0d9ca6f4a441fa62940e7b0f9780a2b77e74704889fa6c" },
                { "hu", "8636e2a6c9523e161b7dd88a1e5ce04fb7cc4bc451a61bb91dd5d2eb6ed50d973544df0ace777735a566c493bce55fe787c839d1cb4e6980268da91873fd0ad6" },
                { "hy-AM", "2e3ca2f840ec7c4cd1f679e964719a541295aebdc3bed5f28e3f62edccc7872320ce32d057e8bd4a453b37ef19c12c6ac5893179b8c58983cf389b0e7e273538" },
                { "id", "259c65344028453ffc3a81793a3796d8802e8357c7c4a828308ba51270f34265ffb420199826bd419c04f32d447a7796f188225078af0222f688d36bbb43bafe" },
                { "is", "29aecb32a5e210a5e16b2087094a1a02528499fb5bb12dc5894b7cdff44985d97a10e92de2c9b3a66601f90bfe340ea698128ef5191334339a04664941fd9607" },
                { "it", "7dab8e154066e366be85eed14683194dc6f50c94c38c8af56cfd214ae20c4f327f59bd8eb7d96cc88f72ecb4762b5fb48e497bfdbd46015ee8c59375d33f2d18" },
                { "ja", "4d5bab72b617ec1396338220ca3865638332e335e8af758847ab218022799c3b31bb65163b16b71f70e76c15de56aa721e68fba9d22bc5e1f5f475666636c442" },
                { "ka", "f70b838958e4ca7070d8d55b8df1fdf960002687e5bd5a6a34f6588b587560930b94ffc0e946f439465848aad0b925830dfb34098bff45be8bf68c7af3cedcb8" },
                { "kab", "939255375b7d377834c4775e0ca59caadb39532dd0fb28e95af5cbc06b74eac58c7cf8cd1d22b463b0ea18a4cc0e6fbd02b6c6625db50e9af232e2390c13b310" },
                { "kk", "232551c152d91b3f3919654ebe6d383edee701d51a824f004c4efda787c4412ee596bbe36900bb1815f2b0a77ccdaa3c6b41a50cf4f784bbac915b3aca658aa1" },
                { "ko", "b6edbdd67e8cb7a610c535a99a4afe162bd2a18fcee4d29e12717fe77f8d3ef31d963d82eac94ee7f79e64d40b2d8c9da70445871df7074608124a156066936a" },
                { "lt", "cf82f36b618934d0e8c0cd13260eb408db7a793959d219b234162f7a4d11d61601734e1939850bdd299279bc7f01dbf7712ca8126b5cbb917bd5c50cb4f50397" },
                { "lv", "b5028baa3defc65ff5af115ad9a8cdc96785a83aa1813f2e57b5911ff556a4e50ee2f55beefce774e585f943ce7468f0a9aef575786345a3bb1f751488f49d60" },
                { "ms", "38ecf6b3bb06e29799edd30923f96951b285857f9bd39bbd0e9ce533fc44b5180f6689f473bf8c9ca8cc8e79cb0b3541f41278ab92dfd8ec8bed470d710736b3" },
                { "nb-NO", "4648265efca853ed057c768954c0e9ecc5ac775e95b761ae7e3d758af874c5ef48f46936c3c8591feb8b8cd1373e458fa9a1a4dd51d074aae02f0d44e17deca6" },
                { "nl", "2ba6d0c613143eb4f825e2b5023535816f591d0e74197fa8919cc5550915cc09e361ee23f5c8369963c5ec753d7e83e948c1f878ba7368caa6b765c3f799ff42" },
                { "nn-NO", "1d162818fe0ec39960cc4eefc6082fbb2271bcb2f154bbab27fc726afaf3e2dec7acdc9915daf5d9a49ecc2495d084c1893dd15d702211aa176a5344dfba3711" },
                { "pa-IN", "251799898096882770005007b42807e19c209fc3fd4b69ddee047ad9f12eb1440d08482cc91c7dc9ec92cf2cb890b9259ec157fc9384b148c5bb24aea496d78b" },
                { "pl", "0da6c71e47ada18fb8916ddcc247fa6b6b1e7aeea5c962d94b2592db06c987c9db7da2ad6929f69b88db9911b21059733c578ae3022af088ee26baeaf4fcdd96" },
                { "pt-BR", "e5fe6178c0a5d181ba72789bdf5f3661b5e6dc8b1650d7e6fd5044c6ccdf03e96c0af436676ad798f7fa1b7e3e9c7df94235daaaa0595cebba3289e240b9f559" },
                { "pt-PT", "44dbe77901f6097cf7da08057bb1db22b2ebd58631a34942c8df91566fa4a8d6503b041d4e1e535f195a77ca9eed4a43d62526e45e0260615ec0522e96ecb55b" },
                { "rm", "05d863960827fb413cf18960586b62eee9f73f127c5ccade4a478f04d5ed5b0edf715011dc041482d182a6a54a10d4e2dff8fba00b8fe2b06b22bdd5001878b0" },
                { "ro", "e281aa4177febf3776f6fafb71ee90ea73da81cd58459f11483a725ea87bd3017e09ca9600265169e9551877f0c659fceb9ca9b703d92ed5a59f8bee76b1000d" },
                { "ru", "fa766cfddfaab17499c7fd1b12630703309ea62cfbee0a397b276c96c266dfb87dbf3834a40edcd76176689b1d0e6bf866d9d1e81647c497e786c67076077799" },
                { "sk", "7f47d7177194f03b1827aab66dc2a0d2ef32277cfa1d5b25dd2bc1bfbb971d0ecba43e4df9bd23f9bc5fabb411fd895af156787dc27f693bccf4e55ae3283bb4" },
                { "sl", "6cdeb570f1781a9679f3d8741026fe20a8c2d4545c60221f669365a2e6db9ad6c9aae81c55e0530992cef9644f40af17a8555e37260793ca06a3204e1292c0c0" },
                { "sq", "19ff50dc6b9151effe67818ed9a77d0d89671c8311f8a8552ceb0139cb029c284d0320277d25276173218e9bcd939bd168c0f2de5f182355986ab2e77306599a" },
                { "sr", "bc926d2e72ca201b3270764cda13c3fb2feae86fd92c09601890a96b124bb8f9229f3bed9e96c784ace44062e9241b09805511aee4a68be0ddd192e23c4f187d" },
                { "sv-SE", "0bfb939081b2539fdb6623a063e4a6424c677d68e297c7e8e38ae77b309921491bb1163942b848c5da9f44f612e644ff57643287f5c72f50ce9ce2ff7e56e8ff" },
                { "th", "1a6184df686fb2af427f0e80b535ba01e423fdc3900b9adbb3ff5976d76c99f9ffdca71ee96a77d0a108a38e2c1f6bc3d87b0bf16387875993cd2ed8eae69ec8" },
                { "tr", "d625e655dd9393bbe3a5006ca00d1c8571d8f67388117053d35914fa65d55bb5f514cbb1f472e1e3c54620fd673d4a802e24c344d77240b752f9f4a676a3341b" },
                { "uk", "f35f35969228a294d3c5d3fbad110a7cf2a2bce1c8f99cbb5f5a9e60837cd80dd16579156253cbd335d82d25b244a376ff5035901207bab918f8fe5ed70ca84b" },
                { "uz", "607a70d5e5375eba3d7de7bc318738182cacdbcf41e55148193e8ce32ab7602d71a97f834358efd40e974772fb80abecab6ad896d787bd6a97d5d6554a2421ff" },
                { "vi", "d3ca1b3b6a636ee12489542fa77c7fb441ea9aa9a948d2a35aaaf2274d8a78225d9fbc6f944f32fb47afb86b17d34fe5409de99c3cd077402ed2382331ec46ae" },
                { "zh-CN", "31144019bad303a4465f312ed85cacecdc330be409c91af4c6a8e85a2a0e71abfb5503c16bdcae5decd14ca6ab867995694229d42756c91a7c907bccdd4af728" },
                { "zh-TW", "d8dea29eb95f4a69e0cbdfc3592b86eede5cff57d4f6e2aa03d3ab8f86202f8abd3e9fb599f9cfb8249d13e2d85bb8e890ddec9162b56af8248481f3aac22707" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.3.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
