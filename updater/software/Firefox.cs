﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/131.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7b94b2f8e9d9d8cb4d3e1c4302b44fb515d517fa26d0d68a3d9023aa191b6c251fa107ea7b23f320d75baf5104018b92e28745f5aa110c79c77363b907bfe66c" },
                { "af", "fcd6c15d89eb1f731c39675207605b7004b2e27bdaaddc2373c91d07b6a920aaf1fcddf94cc830da8e043e79c51116b43ea84c72266bcf15d91e617964df4aa6" },
                { "an", "cb12a259142408164381f8c5bb97cdc8062cd872b852c2787a924e123eb5e4d6b00507703895fb6c8a8c13c83e31047578fa7a84ee35dd868dfda0f3657bdc96" },
                { "ar", "71190df00a5ca9946759a58ee156d3c33807897299f0f1dc866779d4b2551108ba5553014e995d8aed86aa56dc4fd769f80ece8dece1dab1d5feceec76b0ceae" },
                { "ast", "e8f413cfa5cd5c2c6e17035052daa59a5a76630e6c91d3c508bee957538cf425f4fb8de3e4d8339841c25f670e351ca61d2d97eecca102fe48dbc4dcdc76bb0c" },
                { "az", "e1f1d054c825cf87a8153099fcc983008c4540ee65f74a4ce7f2817d19c1d2bde53799f17c8cd4f2d384415a7ade50760bab62e69e14ac014c3f111d74b528df" },
                { "be", "edea72055abd1a1e3806d74dd2c3a05d576d58b0f7472cb7a4149cd3cfbeb215bf25a5acc7722ff433905f67d21233d6b40eeb6ffd62fd41654730f1460ad1d5" },
                { "bg", "8cfd482748610412314bc0bf03ed6ca9992d49485956f7cc9babee512507b2f86d60d57aa5a4917048fa586b3981a12c4fd97448d24cd6c0a8a8cd64b3f23f9e" },
                { "bn", "7fabc1e7fbd5fedba2af04eaf70584675bbe2fb1c22542b33decc3a377f0d2e9c044560f7856171ec9e1ba95cd049befccec35aa4c1c39c0a5f70f32aa55f02d" },
                { "br", "84221f94603fa9f1427b9aee8c716f60d5691b0fb82abcdbf7d9de8c40a18d703c6a9120e7d769bb5df375b41dc6a4a1b241b03f81df786da8d4cea0724bdc6a" },
                { "bs", "c71226d381e35073301b084a43c4dc603abcf0e9493286a88d4615101685ffc5012e81e628f5d1ae07be536529d449ce23a13b6500ef8bbfa4423285a2aece90" },
                { "ca", "3678814b7408eb31700c8e1d1d973a5b9ab12581d3c3d5cebb1276504df16f499275eae725cec6af3f518ef2d945c5a289d8158746860eb2e6dac0789af0d7cf" },
                { "cak", "9dc2c0769d06cb0fbbe83051b1ae42874b43d25b1c933041889d06bc9f130547060332e83c4ae74e94a3225de839290b86eb86d6b0cf904cea1c8028e3085a79" },
                { "cs", "56ef0bae017663b9438f8285b361dc96c78bf3b6fe5366e6cf1dba67c85dad56c9269ebcf98ebde2c4e1b31abf66a268797b2258a4748523f4f38441abd3a5f3" },
                { "cy", "ce7c4338f79127d68655491ebf78781f79af5f82222710d2ebf14568bf7a5d6d3333cd4d1fe62673dd21ca40f71676cc5738421e15a9f073ac97d12d1df05c4d" },
                { "da", "56922d0e92bdbf42880a288ce6d0b681a44ad3c543de695aaef684220e2ddaa49cbc34060ffdfe7bd712e08165ba0d4028bed0cffc1f0445fe0de8aabf55d773" },
                { "de", "7f11340c25db4870364253be88cebe2cd2cdb196f4e08ab9d348f64fa2c4b570f36a001b1b7f569b8232455c1169672c4e44a69b6035ed4fd87c7b524b9a0668" },
                { "dsb", "3ea6f0d7c8489fe07e414ed703aa9eec23611f62afe68bc1e39c909521c076ef8d66ea22c1947059ab03b626b3743da26fac876e7da3b17b8b6acb279d7c7b36" },
                { "el", "70be829669ba5f5f77e04c48ff3e4e9343442ba38f4f7b8d48eec3d6ee38c7f98aa7e4562ede87c08bf8c2e2372e26a7927fb83404857d0fee647003cdbb4ddd" },
                { "en-CA", "3d7cd4d437afe0e9960921ec2ef7924a13411003c57f9d5cc8fc362a8893f968a590665e47831a8534c5464e4fc1ff0a8068b660c16cb4fa55d32383d980897b" },
                { "en-GB", "acb611d0ee117cc8319fdca30d9c529671d33d0606c46bad0368e6ca4ce32ef9413adcd29582a255e2dfec674a2b685266eb91daf101ecc58e546be584fba070" },
                { "en-US", "32656fe20852ff6955d80555bd85d5725387258fa03208b0785b12de26a322f58f2885ad5139718a089789c9c2698b887197532c58ae3a1f816f6eeab422603b" },
                { "eo", "d19030298eb42416d0623f675bc57fb256121777de9162f106fd4f91b8d2f7245b9bde90ea7444c945365e0a5be577ddbae6941c582ecf63a833d4a5f4b82e16" },
                { "es-AR", "5b16228eda2781da15e216b92ee0c97c57a3b024f4b3b5eda07d19a36d717606878dacdc96b9efed3e741d7b1f356fc0e05f4349b3b67c0aaca71d60de493e00" },
                { "es-CL", "b615c8fa0e3f94284e9df7bd1a1f2ebe4caaed44a6c1cec56005c2233171e609cf1ea07b25e145caecd2e02b66ffb74bc56f918cff7ecaf9bdbda1e0b5a55a13" },
                { "es-ES", "d3c005cce0885a4f387cb476725f0fc766ae087d8a4927f572c3ef90aefaad1c430745b5e6b585830b068de87b6392638cef54cd497da47faef10451849ef35b" },
                { "es-MX", "516f43a707fc34cf8c95b17a9b832cdb24213435e79d64a95fa1068c68c4d29047fb367720fed95d07d4b3957d278fba7c0fe3661702cd1bebaf9a7df3b18cd7" },
                { "et", "d6682963b0ff5f807d42943fe43b651755cfd3ab9cc711dbc2e13fc603ad0fafbe465cf7650c584bc055ec404bd8994d45fa13e3fbd25dffb4981c237fde3179" },
                { "eu", "447cdfe784c7fcafeed2fdec092217e54795c88ca71df68acbf94d26b14c6a7f82524a381169daf0d349b6ce6c2ad56d69d7c2890b86c2804269fdc01b561d73" },
                { "fa", "261ad0b27bc544298266f06aa5d628f4fb4fa0194e527eeae530ea4a4cbaea596725769d07ca432074044af93c22f89e2bd6564153a40c7c3d60b2dc214907c9" },
                { "ff", "bdfde365e68a1d7579ccca1925da07b3d6fafa583a65a40a36fdcc1e94cb50381e1c7a098c0a23ffa4fe309ee235d2d7f0a9240d28af1ee5b7ea429d39b17527" },
                { "fi", "61a40ccd5d057fd9ab9a670385b72d299f4b44bcaceb9498c112337edfdeea798dbfd291a6d052b72fb8cec50cc73885ebf6b0e792d622958f394f0895f39a25" },
                { "fr", "e55b28ec7fac924af61fb61549b37abc6a3bf2dc4726b045dac417e6e25a3f698e6ee6f8ed3a521e09e19392df7aae0ae213c3a58c256276ffddad68b9aa38a3" },
                { "fur", "04bd1d03fdf979c8dde03f96d63c7729b87d919cae1b1b12a96b81c38490e88bc94984f0b3414e8cf167f6188fd3270f4c8e22edacbc8dd16f6ca47b7cb155ea" },
                { "fy-NL", "d27d53e088039e86cf2a752d5541ea984d44048311a6bed1ae720607ed3b8d75f88493480715f51cfce2df9499adff28ccc6f1ece8583b2bf8b078e5e1ef0fe2" },
                { "ga-IE", "70f7296ea4627b0d49228db4225d1cacb934b98e21f812b62cc670199bd33fdc56a189b3dea40d45e98c06886c1f36f2b900caf162786426e49c8a0c58a6fbf7" },
                { "gd", "7e0375f40c71cc210bb410c1ebe5bc10403e496f03c44cb193ed021a09bd7f3973cc2b5d3db1657003ea7111153a19996783dba6efe290d88dfcdb13ab9dfac2" },
                { "gl", "42662e13e0b9edd6f13025eff7ac47767f2377b3249792cbcc1b15c1203d237cdc85f6ddbde62454d2d7c130ff10af765756a177619d140f1332b99d3c211120" },
                { "gn", "f2b59b00e5414105bf31d8d914970b67f8bb6dced77d0f1a2c3ead23584d82205e6dcb5cc682f38e16ff6b2e65b0c7feb2e0477e34e984241ce657ea0c4e8143" },
                { "gu-IN", "972818c916257226aa38c36b93dba3f7d60cfd6e4acc4fc07bb3dd0dce85fea59546469a7e1bae34b9ba725e9f08e1a0e48457d52da396cae25c390217092fe3" },
                { "he", "9d0911f95be5e94c439a28ea71a77b19bb22727476741d722aeec4a3481e593d20c0f7c9dafacc7ca5c810ec4e4aad6e2935105471f804cf89a844da0f9decbc" },
                { "hi-IN", "e4546c866bce77291f9995cb982e0292eb05cdd723995a328333725f70e50ccb982325491f9afe39ff572d0870219ff32454079f47731e2c639c8521705322fa" },
                { "hr", "2636ede2af840f6c1add15046fa61ed662be6d21db8d13c2ab379d4d0c2715d7ea316690d45b7e114734eda8914efa9465d76db21c4f1c671ae7a1181f1be3b5" },
                { "hsb", "855fd479cbf40e2ce39d4edca0bfb0caca74e4b880cae4174e1b2210b05b0c6a325a136b39bde837aa3f852d629a92c8e1a81e6563ea9c263963c66c4991a5f0" },
                { "hu", "8364f10203c8068d0411669fa6e17a9ffa4feba1456d5ee1e328b9f656c9a26b84413a9dd7bb03ba0b7b4fd538ffa722af65ce903dfd1b4daf1a8a53133255a7" },
                { "hy-AM", "afffcb50152da587d76b4429ba2195b0585fd5b254fc67527201a4cd3be1b710b3474e70c6291c2620aef725c67811e8ca65837f33408d5b42b1eb1687d79b09" },
                { "ia", "dc9b9104ab673a94b13eeed957554e8d6d7946400194c793dada9adc7afea3e506c53e6689f9d2f034ae7829db73684bdbac6ad467dd3062282dc184dd14fc2d" },
                { "id", "aeda206139e2c67e4b98a4c1885e9d84370809885ac8216011898fd44a6e3932138ebd2c1dfc3993ec580248e3daf6c180997c8e72130b995f0a6a31d382e62f" },
                { "is", "306db226c9ba468bb61d226d957470a36ea3ac972036854706773d1d1b927efddce56d30e550c2893edce5a611de60bed01ee8b1742790f9a3a7868003f48ae8" },
                { "it", "fbc9a86d3c9a701aaab93836652b5a59ae6bce79c9ad5b98b598dec8dda5ddefbf578e871b08baa214f7c489eb6348f5ae1d9b7e218dbcac81655321de927f68" },
                { "ja", "f90288e3da8aff5562fec2341655522f41712b8b68e518a8d7a6bbb3b1c985c3bfe43e6f451ed4a122c649143b2fcd3c50602f45bff637247d8884bc405cacdd" },
                { "ka", "205ce6f1a853665e683933241ad25786b0ec5000564dd6f2898e6765fadc148438717741474147ee31c1e9b0e23efa1c79e091f340d304db2b8dcdcecf32ba7d" },
                { "kab", "b539fad13fad2f39b75223c4fd4d5d0299f740037a180cee998801fc88c50fc8accbc83fec21de97ef528dfd3af49cf6de136146d230c56b9e3e71a23525728c" },
                { "kk", "fd78c8bd4291000fc519d06694d9a30e91bbce1874eddb273208f96adf3b825e2623b745fc1bb278233410ec48bc06462ed7330bf266806ac9f46fd3f28e5eb2" },
                { "km", "230de59a57bd1ba86e899683c1d0c51bb51f0c1e5c26bba0f8c18a34253109fcf9b964de17f17295a2950784954d9176f720ad9be5fc57bf6b9833075283b6ee" },
                { "kn", "7872f9f5c53fd5dfb20be3e81de31758b8dda2689c9f31815a18153910db9e87079bee5bcf968884e886a6102ebcc348109544badb5b44d8161475873141f16b" },
                { "ko", "7f95f680acc736b2e627abefa1cfbc06bacf37876bdae0d7d96af323d4c2ba98aae53b4d7f8046f3e65cb1a232ce91e2eb2b8f9779345977910e8831885971f1" },
                { "lij", "f65f0c0865ab94c345affe8b2824e33609cca62c6431bc2b019a0c59a6b20dcc075cb35835e5c20570f82a63359475075e9b2707d8e758d9d17ab68ca269a3cb" },
                { "lt", "60b63e8a4218dd5173169b5ba7c834157b6ff17d44a89a3ccce4eb455a8a28a8dbd83aac3b8f6334d781bbe6fabd3cd7a76c9750431070cba36a1fca94084897" },
                { "lv", "d19bda088aa346c1505ab447233577da250d28bc44dc388ed538c076b3b6a9cebcc03aef878114f9bee5dee8e00439a26eb4ad2e51d3b7f24bae9b9bdae3cd81" },
                { "mk", "00e32fbd1a380b0db3d3ebb1d3587ccb5d455b80eb5b2a73d9c9ddb3bc0a21ce2b3134b4272ab795f0ff71946275b13443d7de3abefe5597fb823f5c80c46f52" },
                { "mr", "0d7bfdf5567036b99faea905a3b40555d263c1fc629c528e50a570c87feebf349d184d8fe5cd5b10d13dd374bcf23cf7dc82b78491703a789f54279edd2440ce" },
                { "ms", "a325142bfc39db9eec7d45db3c5f8e0101adfa7b8f09cc6fee1a76a32833e61edfec80b6a1e84685fc49855295c30bca820402f57782c1f560dd94863d980412" },
                { "my", "14836c3462e6a75e9440af3dfb35c1528931ccccac942c1bb53f30848ad41affffa6d9ab2228f25e63039eac9fe873543d6ba8c2ab84a5ebb9dcecab748f1a73" },
                { "nb-NO", "31dbd284dfd765ba36cb459944f1411247114e1651e335cb644f832af2c863b2700ef1d11cd4ef039c6d25842707f3fd7fc83f4525939574b64ec24822e3b5a4" },
                { "ne-NP", "9d2083c6226630f0f09d3a4c8889be1a32eee64c739dc4f7f4335650a63c849acc3754a59ec5b2a685b39c81cc70a4448f423145edee1dc6b909281fdc488d28" },
                { "nl", "d1671aa84838b3758c679993fca2eb7b5eb67cbcd61d67d94505f757e0868907c7403553a52a8d3f803149d02fbca9d4cf62100c6f2de889bdfc308ed465007e" },
                { "nn-NO", "df2ce1cc83536f4ce2670abc71721786f7f38a09833b200cb320c0bb2d6d66b5be81528184eaa0d63c36f6ae41a8ce5194c6e0a60d9b5f77a0045aa9c0190336" },
                { "oc", "611d8093004f3d793e1fb4b9bf64c280052f8899d04e596617c37de555feab4b4537635c072379f754dc1d02d1f8a598603f285858e676552673e0658b71a8db" },
                { "pa-IN", "397ce220f38d945490662b7d0ca4f4bea216824bf17283391b3f8d928ee33574bfb3f52236040ce52ea1102b51d488d4a37fcb1b68976ae37b948d078e692046" },
                { "pl", "0af2f210f1773e62c73e30e6f23553807bf27ba32af6d0698a7579ef9608354f9e66ba4498e6555e2faa086932bdc45eb909559ca5a811687352aa5407e80c63" },
                { "pt-BR", "30175adfce1b9746fdafe7416e896e5457cd89dadda3b78d9b44618e0f19e8784cc282b623fd3ddff59f5cb525c409cb1a5bf549b44d8d87b4067b653bf48089" },
                { "pt-PT", "968d34620fd1154cea272abcaf5574f06b3d7e1df9a7905aeda2eeb292e34d77c02df708fdef3cb4544eac1021135c902055157e03199cdf7450c971fb786f3b" },
                { "rm", "48d3f035c0e94ede438a49d53c682e0b86b5fb6989cc77d719287bf6131d6fea955ef43318fa59ebbe70359f58f0ad0196c79e1e5c3270f36850a6c486cbe676" },
                { "ro", "70d752a5e7eeb64023c0085a1b12d6b8d417f144075084a4dcab35b686b2be48cce527cff40d330562eaf29c9b47f03b46a7508d37fc252abb53ff78cceb0842" },
                { "ru", "670d2490a4507289df1f1f305029d83bea6402fc5e4f7c455ef430f2da0b15761ca4d03d98581be57a119db967a4ad4fddf093e8d1187c5260f551043795e36c" },
                { "sat", "dea406ff89c2d421908c685a9a50541afb4a16683cfc81e8d7563ec10dd51d9bcaffdfebd27fb31d09aaf3bc6a6af4c522c0d008379f0d34c0b16f5d207424e0" },
                { "sc", "3d370c74ceccab26ea5a17a03962b3c63cbba997b8626bf872d842c7fd39b81687369fd42d9017260e950b31403d8ee4dca5aba645e771d58daae9c91f445438" },
                { "sco", "b89083418a4211cac4b86c4d148aab1d659be698bb92090af4df50777d7c94c806b009811a0f9041787b8049fcb951ab8a6339d7fe679fc4e733873b499fe338" },
                { "si", "fb8937ec4e380d9d9ecf2d386cfeb29c2356bd1fc7792936b165a3cbd5665601fd27fab3b599fe05897a572f4ecbbb46e9f5224a385479d1ce26a94e6cece39a" },
                { "sk", "9ededc798351e3b61319bd239fae1eac19f1806680c86328e6a3f22772e19874a0806b91d74b9009297ce4963812bfe59d1e2b2a9f0503c641ecdcc1d63f89b7" },
                { "skr", "6e23f9d818da740f75b4e1309831fae6e655f2366ddca64975e6693682e260531d6f2d050e6fd490eaa6382029acc7844f8d566c979847c1ceb76a73556cbc9f" },
                { "sl", "d45e76208f21472f53b20dd2b422133a51b357c5d290a9b7d591b0ec9f8a8cd72057e3bf932b1ea8bf444faccde128d6bb5727a6b2470807cd11877397b01408" },
                { "son", "e023690f237365adf0d474b064e5ab6c8fea914d61079ac3ed852ab461556171ec8cd0de59f988eaf12bb7242ad8de27f357df20a91542e1736481f1497a1c66" },
                { "sq", "15acc7b4b8275ef347cdf1aa58326edd0b9ad256fd17245611d30a42d5259f233373ae1299ec94926b0e7ce3db14829e3cf4ab6565cc14a50548a00e7c6c1386" },
                { "sr", "5211dd7255e704554ac40bd2ced0375adddd451e6702604cf16d050a7c9a4c06c4add0e4d3f740717633ed46df60a788c42142503198f4d37b403be99459a0d4" },
                { "sv-SE", "0614fe45c506414be15f37ba3b98b3ced7f4eea6652b79a2de55fa1cb4301657df78ac3e1de1ec8b5ab6ee460090f572b76f0de82c9eeaab5ffafa49571a957c" },
                { "szl", "a3c9d3a33a639c53bbf2a25dc5fc41feced727663fe6e5bd62cc1b4d69545cbf856e0727479f189885803afd392b5a218ba765bf1554e2943ade3fc0484a941f" },
                { "ta", "008df563d313a91d6438de69a191e94ed15a533a3829749f80aeb27458121a1164a05312c222a69cb6df0c6f04029cd1ab3940e73bc724462192e78141ca08f9" },
                { "te", "d0b25a4b00d554dcfd0875080c00839112625785bad4efcf0cb0dc3eb71d7ed561f0aa45154a6158439739600abd2f287daf0429cd0cd82cb2d71efe8f024c8d" },
                { "tg", "f4d05823c48a3617a63904a4eef1e1ea35aa76fb170f22699528976f3300bca6f50ea693808ba2842b27a06f22101135896d49ff01832275cd3dc9881a5e4c90" },
                { "th", "2fef42042c744893cd24fd57c806cd2c532fd1c676032725ce9f8e1766735f063cdcbde91910a4317b651024291074ded00330d132e78733cb03349e22cc0e6f" },
                { "tl", "a9672801805314bf93e0f4191ded345a0b03309dc835d60429a36653aa0bd9bcb3a0c2575b7ed71fe9536164f8c2d06ff1c54a8be2c2298eefc41ffe78a9b27e" },
                { "tr", "ca9f7a82fa31225c96e6ff4f4d11464f343ff32c663883df9133257f96cf2abc1474e61ef3d994c9882cc628b74f2bcf8cbf765a6b0215f54a2415a396c015dc" },
                { "trs", "8bb6cab24f2a32244cdb3fecedcc6dbd1c1d8c35553d86a305328fe45fd4d0760e9dd5dc98f82380f634285dec650b5009e93ebc967b4ca4f277481808053d45" },
                { "uk", "d757f10eb32b99e1e3e36a7ad17ed106ea91d8e5a7060ad9fc9a3d75b90345b3b1420a0208a06fe916ea1779645e58c3d2a49cb92374594a8a1c98effbbb6b65" },
                { "ur", "30260106798b63e779060257b81a887a8a930fb19bc72f49d0f618cca6e1ea55852d12a71ae145e578bc64677cee5747de70a010e8f8581e929fa3d39ba4e251" },
                { "uz", "f519d3f67104f35e4685eca12efb68ebd53c76158aa23e079c469a86ac06bd8c745111d69b3b9943051c63611ad79fb7aa23c391709f35ea27e472cbcccbf9aa" },
                { "vi", "f15a6d3e84c3be6e4b6fba1602c42b2fb628ee9e71fd0e9aa26715eff0964709ee9af247f446115844db3f09dc99a2aa15034d940f55fc508218cc940591d02b" },
                { "xh", "d61e7022779640fad49e00ebf0bb869b5ff331f16c1bed42f304190b4211eaa3a86ce104e31cea83b7982f74d5f1af2dc1026e9896f64837233fe92bb26ac3c9" },
                { "zh-CN", "a11ed8af17499da8e6135b2eee188de1b4cf64f03f8d55dff14423ef3a67c24d583a55d96627197ab49314d7b097583380d144c81191072ee13e8a22e4c814d0" },
                { "zh-TW", "ccad72f2a4fd77bf65076b4a1906392dd5ea2d7844a2818f9e70895eff6cf2de1f5c04dc3d3220956718052d387be516baaa026000689cf218f28af13db97631" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/131.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "d2c9fc3d6be454d9c50cd7ed9de2c0c1763d0d401024d261057ea2a263236f154588b659f684f11176f450f93ee02ddb3319dedfeac80aa55a2294a7c35cf1bc" },
                { "af", "9e1f9720746f5052b2ec77578c424b73856c3a79b97ba30a7a942719ee528aebfc2bc5efa8bf223d0b9980c8f3a5fd3d80f9962d65fd2f7ac7e3a9c4c88a48be" },
                { "an", "9c0d4d20a0c9af7f29f4cb7736d4f8c5b6ca4e8324e9020db43654de014e9d62abbdedc4a7832a1a64cee2420d32b9edf222e37befb1a3fc6395cb7ffccdc8ab" },
                { "ar", "b575f2db5dc09faa0666ec8a531e558bb3b3417a5e52cadcd44dd2c2e7bc339cad7af4f739f2604917a5a7565d4631e461a2f4fdb74f959c24af706bb082d683" },
                { "ast", "725c2c08178b3a14c3b519c87a0d828d9e107d10702848ba5a2e96bd89adaa9171d7f87dc61b9d0bed7404531e62dd929b136ab8304a4fc40cf82db7007f7dce" },
                { "az", "66d0827e62b01d2b4724681f3210dac58f8ae362e725ffba37c50282281d834eef5fb432ad03b6c45265ef049b64aacf4bbcf14bedcd3b5e9fe34f916611911b" },
                { "be", "577dcba38d332784c4eebaebf4544e80adb6b5073b0f654201de211745d429a495d641dfc7a2541e86350f2455f5f091d125e9e117ffad4ea97daada0ffe3812" },
                { "bg", "95b2e692160ba01a86e62f2948105af5c1fcd3b8fe475cec7578a72576f320dcc31f72fa2b5fc12c00246bc04af378f1c9479059bd7501f3bf8b493c9f222f6d" },
                { "bn", "17e335c4354fd031bf83afd9b574282f98214ed161c57aae352933b99b0cd856a5f2693edc7e090aaa6a78c75a960eeabefe9760fb09f15f7a91940f195c525e" },
                { "br", "ea162b261ab5266fcfbe89cdb8c3cd0fd13c6821e076eba5e4f7c24bbccc015036a33bb8883d232d588a6e5796a83e46d881ee3a9ee343d0cd32dd2460c03162" },
                { "bs", "4ef2adadd5e51bf227da3f8632f447ab30b6a2b72b73bf1fa318b3a2f37a67975ed0eae11244427dc5a10375d07463f85d3333286ab412cb82293c7adf3166b7" },
                { "ca", "519d01115b36caeb264189b2cd13176416da2e4c90779f13826f6c025e46ea810785ce2489e0ea9a489bdb857cec60b5c7577decd6dc2cf59a08ca81b4c227b9" },
                { "cak", "9c25fa51608f9cebcc845f52ffdb70636400540db5f46569ba8418f8f567e2df161e87ce9817abe8cc0c785f50ccd8d2a757aaf5125551e33cd4d273d7764a5f" },
                { "cs", "50ae18c65ae7d0811ffa873e05f6d71e71a9be97cebdfe1c2b25318058049a0ec61602d01c787b41d810a8db638f4512a689b31f98b254d9c838d820dd4e7f20" },
                { "cy", "5246731f4763f69c893dbd4ea8f4109d647fb0a83129f56ae5e59393f2c3a57c03d57a985bf7a576f30eeb4a523033319e2a50777a59e19f1764089e9aea489e" },
                { "da", "2b3f19b314c9b07b688729dc825e1c67e41ff3a6c55dc03c2fd8033da6d7e3123d0878e0e635f4704d14a339413a6e33f10816f1167a62d12237017e95259e94" },
                { "de", "2833f722653b91e6c5958b9f285de8d15a812f7fca17706b661c8ca49c045f9b681986ce7ae0b564ed79fd937f352d0d426b7f764f3ac75f242eb3f32328b5c0" },
                { "dsb", "aa98eb3bf39eb239b8318fa8606c66fb0b47d476141aab8c1b4e796d32312b87dabdb87137732f28be6fdaf9d933e71bbe81d3d34ed46653d7e6dafee0a55a17" },
                { "el", "12df7357d7bc29dbf3dbbdf19ae0a87a2adaf26709d38b989ec144d348d4ec4a734bf271d9ce7b6e8663a0b2502de3b0a83b395cdb45d86b9a5f1de8fbbeda21" },
                { "en-CA", "458c083d101ce21e6e1b034206d482111456d8ea1e78fbfd9ec686f4b3c78b2d55b346c10fb299a2248a163289f7fb9508831fa4861c2ffad4af844f33ae6890" },
                { "en-GB", "b6a941c88a05a5407c5d77f854ed9b2fbcabda87aafde5f721a4ffbc763c7feb350b12aed71c958a9fe8a72910eab998e1994a30c527aa96b1660b40fa691598" },
                { "en-US", "3e3b32c84c0d67d5aedf69f7f477bec084656c030060bca98aa8c55b719504955b17f8666e695c2f6f7237b0effd46d462ef2191bf054474cea88c38f766dc79" },
                { "eo", "85847ba7e2b28247a38bba154ada0137761d2b7680de8c6c7fa67c9654dec6156560bef941033b80570465af53b463e94e02d20ecbd8cf68601039bf1463e847" },
                { "es-AR", "741858059ab35c9e01e5d7fa6f65da15582859d5186e6a1f786c44e719080e560fc2e4db526c042a452916f1f84211d65b647da6eb68ac61443dda0f2ab44bb2" },
                { "es-CL", "d66de01d34722242c4fe6d0a556fe1e526f61e05f47ea477dacba911275a055764f01ca997acd51a9427322fd0d71d61b8479221e042cd9e4f6904c878adc3ac" },
                { "es-ES", "9f8e85d09f9db552fd06f825de75fa052dfacb2a3cb7e10a51558c4c77203050722ed1f3c86873e29461436e334cece5d8951fe2d2a3422afbb25d1fa0df465f" },
                { "es-MX", "61a62fd4922f12b47324528d9eabf947b12897086c64cec7d3a200cf292f25f8491ab1c508e0013f91839ac1adb8656e9224ae8393cd5fd4296be6cf0934c44b" },
                { "et", "b4570ff5dac9fe6f9c8f240b1d30f5074fc2da2824157c8e7003565c20f4022fbfb17c1f39f1743f77813a42bcf8d61ae048976c8cce6e8ec66fca857a8b067a" },
                { "eu", "cf78d6f66e79c1c1d4e46d9a544688add33bdb400d28e4f02da505d4fd9cd73f54157d5e8cb4ab780da45d7a01350fc76208828e3005c7deda4b070fd04bc302" },
                { "fa", "cdf0418a99c9c5d43570fcb77594228066eb326484acfc36fbab29e293e0ca6fc2976ada41965c455a5b73191211fdcca164c1dab74f6a6d73daa2a3663206ee" },
                { "ff", "e5d635fddf6a30c8fe7ffbb34a3833261d1f905f029e6819b5f4ac669b4304de88e16580192c711aaf04af1c41df97040c97fda26c2a2ebdf81cca0d8fce2e05" },
                { "fi", "837ee4ea1b872e08460930a14f14af6c660c9b535e7e4f9a47dfe01034ba5ed4d1b48c0e83667e9f3ae019343ff5021062b2a98ba56a91edacc494014e49f754" },
                { "fr", "c546830e7f13b03a89bd5b6d0778b840e05eb2ccb9f8a1408afc9048fb36d99626549a6996500ca074154524c857a4825ae14aab5a6cc34891c90efd5aa41e93" },
                { "fur", "88571d24cb0c87102485a9a7079411c0a69ff3ce328a3065ae0f77a2d5551e94c6a75890de7851fcb095b16ec7546c021891fd7b5e009081e81b6091760250ef" },
                { "fy-NL", "3de62f2bec617d77f542403918e4fac8f292be067a19e75391ac5e0a1d953b7e7ccdb603c8b0402d37b52bdca50953f63560c6112c62d439704b205442d21771" },
                { "ga-IE", "709f4493afce0fad0703003178a052307726c180ebf8e0f73fbda4cd3a85d712e6337423aeb9a17542a73a7d92134d54ddab2aab05ff30beae9ac4c8e9b238d0" },
                { "gd", "a8bf919d73aeb206208b4e88d74588bba788a4453c8424cbcbc120fa9ce4de035300ac4ed8e08c0f19f99e9590901f3deb98fc88e64f9178524294c3ffedad6a" },
                { "gl", "57474e2b06360421d774743372f722dea20bda032cfb263896710972195cb0c832a3b1f883cdc567c823a8154b9e85457e285c3ffb1b7b4301d0c8de26e8a00e" },
                { "gn", "246e80e8e8a7391bf1348b36555c2c1e3c34b7e8c7858d02c543b81950a42f25a3559b4c7ea7a27e8ea2271f487bb02d1a4760237ca601ba3b49914bf03dca6c" },
                { "gu-IN", "7bd89db3ad527da85fb7cb679cace09416c398e4b735a15068fbf8bbc3a38a542dde54e1322db44f7df6561f51f5d6878d86b3e85cc70ac4966842aecf7534d5" },
                { "he", "bc78c20e83e990e38aeb14fe3fa4731a3102f9c64cbcf31221582e4d23af2906f70ab7e9805051273eb71eccc64ca1d26a80a73c1ee741af8ed5d01f5cf9048a" },
                { "hi-IN", "9b8c4334d3f46a3c78e88bb29a73e0db5e1eca917239d5b84942905fa38f356db778457f2499b684d7e0f012235d0745fa06a0cfb8e6bae94a07554976f0bd21" },
                { "hr", "8b932f8ff16ed5b09d62106f7cd141277c21b20c92c2e8ddf8cace4af9e3b1bf947560f25007a659f9c7a18328d16d004a98049d0bed4776f4699548a1927c7b" },
                { "hsb", "95b32a0d826192c355e6425b86c04b490dc1f7c1d768b04bd11c5b4aec0010dbbcb3371499b6bf64e8d1f3501efefb81c5bdec6e5d3fde9a2116190a0f47482d" },
                { "hu", "683838e7aafbec9c1238aec7f29d00e98af5fbe8d504df8e18409512f79fdd11b4bd00af52df3230b580710668b920ecaf19dd6ed8041f7f752244c49f3c4a71" },
                { "hy-AM", "22f694c9a6b2f2399b35b40dfc8571a3717a2ff875357a5a76cfac45b2124df34a81f8efa113d71569d58f01524767e63a2d06d1c592e6ff90203a45e60ab1bc" },
                { "ia", "580f274c98f6919bb3f8fa50a28f9678e817f9bddd4b30811fbfe36610a450935f22b6238347da91f02e2c1d71688d3804de46c540e0c28c4139864821a0a001" },
                { "id", "e12310120887dcf077180841c64f5cffdd345f234e9fc7347675b6afbb5045b085e14894010275952cfdc1eb3f509455bc8118bdaad9f4516ada3379ec2d91a8" },
                { "is", "9518d79f47dd341e4013f5e45aa02e6cd865ca191ae2b56e275216b85f8dcdd68376a154b672c5b00cee6d3eb00dd0aab49533b74c86a4efbfd184e6ebe90025" },
                { "it", "f704f0fb8c9be3a060d9bcfd010c8d79410f02efab74d4b6409275a6a3068dc68803f4781c6a360c33c62020a4f95f0450be90de71b3bbd2376cee7fe9dfe1ce" },
                { "ja", "7bbb61039f412ed6da0662f3eb87ff70c73e0b3764c71208d6a8692536b1e68871d9854542e771d635bfcfe2f4064bfbbed97ba87b43e088cecd83507f31a876" },
                { "ka", "3b8e204eac58e68629973179da4f3760cbe57c4b01224e30b6831305f140c059f40037fd1591cf5394484f08c6b15c0db78042a6bc5bc2b2155bb86e77150e0b" },
                { "kab", "c8b65caeee793d22e97ef17cc968d204e4247fd2918e2af085a927a7d16a7d0246da6b0d20e058c2f810596dd3da3747c5b3896b9a0125a48eaeab84fdd57fe5" },
                { "kk", "1bcd4140b383b629d2a012b96719143a382cedc3196591524b635c304b102bf3b2646e0f8871ac31572d4f40f0ae1800978c1b61b5f9df98147c35b43ea0f650" },
                { "km", "02e9a15cef28a63b0975aedcb1655c9ee31d337f6dc066d9e081351c0e36e6f1e62660892f729dcfd5cc38fbe001e7135baeb356dded20cf09796828ab660550" },
                { "kn", "299b1d2bb306d0b65d7996b0d33ef9d7231683cc9e056bf13aaf0a776a8098df11cd7164aab30c64be4cd0f182c54cadd3a6bfa35891584044c2051abee7d692" },
                { "ko", "f49ac09cf5552d8598bd6790fe21aa25bf52a9368d673b0b958bb6a87804c09b334518198e1e291d941bf420cb5fddb287db4c87645f5e045b9a6fd6aa53bcd4" },
                { "lij", "10b52d99164d386a1060e11eaaba468ed6add66c952b4190003b54dac856be5289ea1242a29d859f79a3c3a92c2235ab5d7b61d82949f4649370c34360ffa6cf" },
                { "lt", "cf10d7511554d2e4c1bf1f4d8d848a6daaa87bec9d48611fa0306462893f75cd8cc5650619ed5a646c1532c6bb1fdad49f4348f9e29cd54ebe941ca3149ce1d2" },
                { "lv", "ea5ca3c68d7749f652a34e10c80ae9c7d3bed6b11bd526f90a761dba769ef3a6518c20200ac896939d1b1b3aa398eca4ab09bff12db3b00b8d2741c59e3b6ab5" },
                { "mk", "5c21d5a813f6765875b775f58dca7b9b0ad5484d77a29c7fa562215fbc305c2084d49eff4819cc52a9f53195b15cdefeece9c317f3bdff24260908144de788bf" },
                { "mr", "69ce55d35159053570311518c422f6d1f149243551770188a087040064c7600e3a885e44f06438462c2420f7bccfb2723b221a26425fed75bf1395375fb05ab2" },
                { "ms", "aa4363a64980014b88e225cd116368f8af392d93e0d66354716a88a7823b0587c71fb64d438d5d05315223bb20aea34c356a9ddb3d242ff7a7999bc3e4d31a3c" },
                { "my", "80bda0be1894b30bf0b3ccef7ebe0ab97a4a2b9dac024ac6b67923c46ad62a4fc39517a77ad519a54ef4ae51f156ed903471620b3c62949cfeff57720bb7308e" },
                { "nb-NO", "d8f94a1e0a91b280caeb921e90fd1b990bd2c0885c827a80f7ed54337931035add4894a9f0bf9da190d6eec3953a35528a5bf453ccae51c44e30aa40c6fb37ef" },
                { "ne-NP", "28d7143a8b60778004fca8f6581b07322c5e809b183c398717dde152bec20196d68402b140d94b59256f4e246442ea69aa9ab26d2a28beb565b8e6b4c263ddd2" },
                { "nl", "e68c62929814b18d1738fa985aa4ce22db450c98ee47ba49bdacd8926bb77c22946e5bfdf1b229369989c067102764beb20e25c1074969cb20d34f07a65abac5" },
                { "nn-NO", "bb2967109f823f4acb8a9ce4cfbba8dbc078e192ec592d0311353ad3893691f9474e9fc08dcec529340c0900e806d1ba360d967260033febddf736fd70bd2b52" },
                { "oc", "5d89ec31a015385a67d3f0ac228dd3b71badce8b82d8f917a7234ea3cbb32b0bfdb70d620005a632ff391510f9030a109f0ea50ce6e2a3a45c44cd2ca72c79e0" },
                { "pa-IN", "56e6554bb712c59c0ff56aee7ba2937118c87635e371b944eba001d6d10367d9d06d60494a7fe13fada1db3f2d8da56b8cfcc48364b87eebdac4d036854ebed2" },
                { "pl", "a91059cf95896a20909f58623c1c92b26bda5f606489a27597191394caeb39446206b592a0ebc8376d97270d4d1391adea907bd7920eff78180c6c21e7f5314e" },
                { "pt-BR", "34458c8b22655c579731348905accbd9fd364f0d9d56025bc0b6cb06740b36128fada6b035a8797c0580363c0330e9da0d346b15910af00bf9a0f4c57e1eb52d" },
                { "pt-PT", "2f7b32d099818acf4016a72ef3090b71a13c97f5c0a6205fec214fb9d2ab7a166507847a09ba090e4764a3fbb8d44326531e48c642a5f421dfad2fc1ce8f1832" },
                { "rm", "c56747e373f7ced22fb6da87f0c8f455a52fb361123655a8719ad0dbe7bee0d94a19ef7e0afb22f50eff6eb0b2c13c51658e8377a8bdf58cfea32f826497c03e" },
                { "ro", "0f22e35ab94f0df4267522ba02127b4a953d9bb079f9670cf6fafe634cd649f49ecf1b09dcf38f2d27b5ec54a9bbcf94a46c8d334eb715e0313c9f5d5643591c" },
                { "ru", "5fb6ed0c7d26e9926427ac526d1d90cf78cd0c74f62dfb1be1d35dd03108956325942542daebab49ecb84d5f9ede1c515a7a7ce47716fbeef752326308fbbfd5" },
                { "sat", "bf60898b4f6b3801fb165d63bbf38c76b29c67a10cc5a9d3173796f2e7a9b0ef4d47b0c913ebde590f981cad55e09bf34a0f25d743c2fc5dbf4f66d851e031db" },
                { "sc", "53b78391e1f983d1ea65adcfc149a38eb306c29aa7cb4cbc8e5d21f68f019254514bd02eaa3ec1aad57ad25ad97c7f40cc026e9a79cb841f4ba2224b5db955e3" },
                { "sco", "eb0bd07f7a8e0b16857878f8766ede2956f2ab1d431b2734bbda5498c803c2934899900f696c169cb20d64536f7a54792b2a259f270a396fa2f47441e68166b9" },
                { "si", "8b0df355f0a73c6f4033fcb2a116d80e355d6a17c279257af625540002c33bb67004146417f8ebcb8ec15128da492700ff74c7561ff4a156ef55ff0b040f8b69" },
                { "sk", "cf498d279a8f9a2a66b54a2b530350531de368b4718f6e163ee05ff1fadc51d99008dde42f170f0c1d2a5d1d6c145f3cd82d2944d460379861ad56d1231716f7" },
                { "skr", "409ad18adfdcc3fc264914b974a185f6371e0bea0ca11be6705c2ea386becad0709493e566595a8a1f567207180800c508ce223998ea1194e46d09e5f2c102d9" },
                { "sl", "2aea6f6c91ba87630ddca98c49d0b3ff62cb8ecce82c894d120ec0f251ab2b6f616f9580406d2e39ac800fa46cda22e21f0ebd3917a96dd5f5de4b47aea092c5" },
                { "son", "fa93480cbd3fcb5c5bad4263a868eabefc310979df9f190e108772a4a6ba98920c39123f2b961079ba6d634fe9f1a49f36c7f5bf5b0237e42402f1ef8ee086a2" },
                { "sq", "8bc5f7788ef729aff63c49f585493032196349a2e3d179e73b7d2f079c6992b041b444901894201b01765d9539af4ce46c79ca7cf34a065ae3d180d1ab0feedb" },
                { "sr", "0d1c6e4b25f56bf9436b1c7f81c0bd588e785fbfef8e6d9cc07c310e132e4eee5f99ff2f927e3a16bff02332c5eb311e4cb9dc9b28783174998228e708e7601d" },
                { "sv-SE", "606c0882c7ce1392560ef711b04d3fabd3789db49814755c8875c9d204b93a3fdc225cb8b0a5e080a70f867997be4bf5d37061ae66dc055db1ac3e1ed92763bb" },
                { "szl", "bfc324b0864d7f6b78154b2a0189ec534de8161099dfc17ab77fce8ac6d2d025cc6ad487a5b7abfba3f4c587753a6cbfd871de7652fefb31cdebbed371ee60c2" },
                { "ta", "b1b7fb42a73db5789e15cf46fbac8e5f4d427a38f6721e7a69a999fb9a570f45167cb6f4c5746a49219dc7d3dc223dff3e5ebc78d3812d134a3ec555a191dd05" },
                { "te", "2048c3c5b6458cf71ac24acafc7bd7cee8de408c2528bb1de2a501bdbc56dd620d796f693458039a2047cc7c91fc79406ffeadad67a66c5b61312852d8362f3f" },
                { "tg", "ca5d7db7f95281adf373938ed131b4e5916e25d3e4d9611026541ebaa27adbf78771e03df0e7e894b045b9ac0579b2b96b4dc5bdd64721de46db23fdaf2d40e0" },
                { "th", "e378dd291829dee9aebdca2ebc99895e5c1365914b375a742afdff90e65635a8e77389f2a410b2f741fc577e17e09ee7da95c1b268d1544a6d57f7cd63b19580" },
                { "tl", "30cf05f131fad6716ea638eb4271892555b28691eb88e83d1d54bcf9e11c374d8e1964d3058b0aa413ac084af41e28d437525dfd8cb3f8e78e53acfcb7f6c88e" },
                { "tr", "5ec0136378b3eb1e892cbb7e1114ac4790e45fd9520aac04f90e64e360cec65d25b67a640078eb23625b21262b820c15b569e771f12d1a5a8553db2b23589b75" },
                { "trs", "66b25e13f4d69b2740c75e04bd1bb42d9b1abcd642e7fbd600f9f87d094916909232596e46b9d5d33be4b23785a04baa5249519a551058ac40c92d4da517f43d" },
                { "uk", "8a09d82a51387af8f08c598b597e4ff53167af890a10462082cd01cc9af42883bf6ef2abcaa6687b5a0e5b47b9e85d0d5f6848a025bffb49a9ae04255e86f2f4" },
                { "ur", "8915d6dd7b2bae74f7e2a827f1c0ea2c72a3fe0b37e80ab8cdac8fe0c50a4941e2b7fcdcb0efb0863ab4383eff39debbf83de7bd2e470e657efb33a9282abbeb" },
                { "uz", "82436782c17f7f6dd1e4a5cd6b330f6d8dc5fe4267275c7a98fe638b0ce29de8a2debeef5b0340b371df9eb73156acd81f211ad0f1ea0525a71f7fad5b2ee5df" },
                { "vi", "e1002bf0f9af90c51e217e62e1fba0f8bbac392bf8213566414b913e52e6fce7349f503c2b8df10f9d0c8e923c1ee46e7e0b881b33967717b975d03e3e9715a8" },
                { "xh", "eb4ce9a563ffa1add97e291120daa2856245c25d14e33294d6cdbeba28884e617bcad578ac433263d20a52c448bea87d5ab3babc7e238f5b3af35e88332cd74e" },
                { "zh-CN", "2fac55d11b9a9e3d5a264199123f6d0fa0c94c33666a56dfbadee673c67c42655e6b7cb2b0e75a661b806da08be72b10e91687b19f8f61c6ee59cd763a3cd959" },
                { "zh-TW", "13d36deba358efe1769c40413a73d71adc315af4fa62df051c53e3d19240c3b880d458de65403d45132cb9908837640142ea0da5ca8db24e3cdc95301eb63457" }
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
            const string knownVersion = "131.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
