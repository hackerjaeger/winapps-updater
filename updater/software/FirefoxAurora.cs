﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "121.0b4";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "98d648a4d644e3b1c5574e21b10312df7c01d4a025b3495f5c4d49220af736cff3de3461b3222f42d24443189e823b4fd0e6452f1609fdbbc94d8177b15d02c0" },
                { "af", "00140133eeec08c1b42b51c05bafd78beb9b954036475e5f7cedb041b34b88c480591a398d2de55a57c9007e3963c60cb9794ee3a548e82e11a513a1a670597c" },
                { "an", "7b612fbc32e0bcc83de9afb93336b5484b0f5c7d2282cb086c74cf3632648d8023b1526ea1690534d8fd036e3a78fa916e18a81e694f86a37e7246bbc9da4231" },
                { "ar", "e7065b4265ecab54a153d92d5d1b62807fdcf476df238bfca4c5fce6d5deaec3e6f1fee0afb74873ecc20d650102f4860d31e2f2ea990a905b6f62358054bb97" },
                { "ast", "9d415e6328e4680006a96d9c27d9d5c9046e44a2af8853e494f3fa6134c38e43bbd1c77df146c0ddba4c2b2b0f835c51484808898d8a1f9f3aa8608be9b90e10" },
                { "az", "ccbaa89ca5a15d5792407920d650b33c4c34d509aa19f762eb56231b9a745b58b3c5aeddb72293772f2b0b0a4881b2895c7aa7f78257ed5d04c1e6c4b857e0bf" },
                { "be", "fe4cb2ba34bcf59fa0a2fe54b558d2a1b89ba6938cf9e6227a0403a4c0dd1a5d79de1e826f56b82af03f407417eddd9383d28accf1505441692a2bee72f31a0d" },
                { "bg", "d917343fb870def47469150c388ece7d2fd1ab04ec061e034d448257d05a0ff73819d06ab8b7ea9e0ef60a2ab88ebecf0bff46c39614e91047ce0814fc1941a2" },
                { "bn", "3e61e98d4d0c8b1d56d3f27a84acf8822b0e6723101d33e67616131eef623548dd78a35ecbf1b4256382db3e495337e235b9fdde744d15b9c4aa3b3f17ef080a" },
                { "br", "95b90f28735abf4a8af2a418e847fbf3342bd6943b6ffc5b2a66ecc59f4329e6427b89428e1125d2bc7bb37a773e8f78b2e0b1bea620fe581af1ecc62ccf0fae" },
                { "bs", "218838d7f6c68c227ecf2a781ac03ad2160c33b530ee2694cb1604f2e5052680210051f95ed33687b85b38383fad4206f18774d19f09a5eac8cf66be6a9b5b5f" },
                { "ca", "ff7f9f064702b12803f46ed580092a8a006ee60a5a536e8c9c8a58fc84e2dc51463af0e0a8cddc1917fdbfed4770bc9d64f826cbd0a4417fbfa941820a46ad12" },
                { "cak", "3723d10626653d636acc672866498a34d98dbd7fd2fcae5eea6b1cafd73770ea75cd3323031a52abccff2956e56a07e17952e91318ff542ffa2c5b8239e1be0c" },
                { "cs", "ed4462218f31cdd335d45957097e157a4b03c835a30fee8db94451f66281a8918203dccc2d855b513edc82c89be3e467d6c8f4b84b41802ca95490daa61df967" },
                { "cy", "922f9069c64f5a0e973e241679f338c4fef9c29242e3671369404c76519edfbb2b6ac5b5bdbdaf9441036a16623ef3ad29ef271a95de97de0cb4f65da928fb81" },
                { "da", "58dfa47fab0b39f80118bfed044ca66ffc90e2adc1e3b5fd4d5a355e002486df5022a92bd617d4034b3e00df69007e713094d80ad8ef5d615427071e265db5d5" },
                { "de", "2aaaca5598e25aa7cd284d5b493c7715b040e8ca44f2b098c2887ef381dbb3550794d905306384732897b6ec382a136006561168bdccf38e82f27c45016c3420" },
                { "dsb", "d36b5e1be4f3083ebb2e8225c0b95ac70ac4c91107096a6cc313a3b4fb76593a1b1f8a1d4d5c43403ae7d5fb598a036bf52730620e2a46fe0cfb89703332ee67" },
                { "el", "2cc1a0134ee7129425403de892058e9e8c5471f4225bcfbe7ba66595def3618ca3c38df74b603b053213f8f4594920f1899361147cf88572fad916283c51d137" },
                { "en-CA", "6ec5e86da1598d888b1013b32a85251acd9602a0e09bc0365c94ab24d1ba7371be7bd5ff701447b102259ad994b5db07a55190104839610da98ebd72980b2d5a" },
                { "en-GB", "9487218a61957b060306baae1bed9b22135cd00509b2d739d01deaa534616d21d6e5b55e110dcc60c77e35b3689257435d5839ffa29667537030708bdfd905f1" },
                { "en-US", "0f0288577b5a0b696953e39534900a7e199c98418f87b3c4bf0058138d9d82b1053b90876fde2deef4de1dfaed9243227b86c77e329aac67bc70393166d4bd41" },
                { "eo", "fe4e7759384a212af567e3f2e8b0df0cc1c0d61eaff574d7ec856aaac9a15e74115294796635f76c33c3488262d6f8271ab8d9dda7d1b4ec6aa7dc25ee52f0cb" },
                { "es-AR", "a4884a8aa22f6cbe95e6c20b82ae55aab19bad0d600937a13b8654438bf9f4c3d326e13fa856e0e3bc608104a559afd825fd1c6af957f0d3a23353ccb817b25b" },
                { "es-CL", "078db5e3d895f4cdac49aa5a817a6b5150d671a60344906122202713135c214f3837da1f7497a2a0925bf24667156ff45e633af2471371d6fd4e4aef2a9fe813" },
                { "es-ES", "c1fb19706652921aba713d417f87364127c3c915563d9fcdfb36dc01559a0aae0d1389bc389ad4468589b1fe4eed1a0b4135e57d0f32bfb6fbf8ccb1ce937236" },
                { "es-MX", "c549d5422197402635e804f4d2989b9e8549804705f030255e503e57d8c8d598516cb4d3047932a5be3de5e77430d04f21ada9096cb63aa85402bce5426a2649" },
                { "et", "9574a9ce7f5dbf4005f8f403b793712d8034854ca9d9307650ca2d2580618e1ab5df59849f2c3559321059af63dcd2f94a69ccf20cb07a047ac8739b447c7b2a" },
                { "eu", "f7958b71c9cee0325036c6815a7def4d455237897f00be8b70f8d88d48e4bbc9e3817c019f41821ffe726913a9de40edc67d76e4bb94fc47188b8233bebf8c8b" },
                { "fa", "a6d0832a96644c4e54d9f45fb4f6882467aa4d6b459212e3e818e54ffd014c6f7651f3289df80fc8949dedf28889a48abd61c2ca67f977c7b4a39b4a58873937" },
                { "ff", "405e484c811d227ea635a45262ef387457a2e2c4c3b9b85effb8483e90b24aa1baa56bbf9a7a3acc624c57b76a44b9138997ae8f65b2b4fc2e26d4aeddf7b8ca" },
                { "fi", "53e87cc1e9ad19e3150f59b3f1bf86e364bc19f0678ec22adc9bd21b9f96cabbac631c35c6b3357714a0ccdf85039def4a9450e9d4f0e01733da1fa786a24e16" },
                { "fr", "e30c3e0ae04fd141a2beda6b8c09d6cf69c5b8afbccc829d3edaef44e35fededea2f45ab229a04a6e614beb34f548d870e81cb38966a13929b05a27f7d0c351b" },
                { "fur", "a6ba66e4232c95b61871a47c33a1fc133cef568ffdc1a26eb60b4c7057982e45f69d503b8b1cc379cabb58bfe3e651832c0b26ca6efd6ee9779f8d08f664105c" },
                { "fy-NL", "5a74f276b6487983bd1d71b02b38f2e30d92ae46b2a5d6495c4fca6c6517447c267506f2bdb66ff2dd239c3f4d24d2a33d58b7714e27f1a88a248b2bf100c9fe" },
                { "ga-IE", "c1cf35c1274be54a563a0bdd8044593bfa4af75a42db04337cdcf7d569ca24fa6b1975041a6724572cf5ac43e82027ef0b88a619da5c96e601805cfcad79f299" },
                { "gd", "4d3cf8101c72ef06156494ac8d6d7d7793c6de3b194abfef92560663024f588cf11a4df05505a501f02ce93af36e3ca3d16aadbf28a0f18aff1318fb97d8d0b7" },
                { "gl", "1e9de9be8576e7dd023444f1526bea6674b851eee46ad1c594d53fc612ea2c0d67ba97296c10ac8b1e25046527631d5cc578a3c151ebaeac29ae1f77adfcfe4a" },
                { "gn", "1c45a609a212a61bfd12b00e9055128d4d805485a8952059d16e3abc440ead1b2e6aa5ddf3a4959b0750b04606e48eacd680c2f5349f5902e207fe74858e1c89" },
                { "gu-IN", "c4e713ac3daf67a8e645c87dc831cea0237c8cbe48676a9a0142125fffe68130eda9968bb19da3e7ee3a138951e27612ebbd3388bba32548dcf36d662f8b3da4" },
                { "he", "5c6fe18b3a9652b5501850aea7877b37d77049ed5e5dc05411b76a14796c3954f26a0ef1d5dfd8ff7e171339b7fb3ea461775602a14d54fdb6cf4da52a615a44" },
                { "hi-IN", "e3328c598ba8a31f76f2fc5f1fb83266e714dc79ea48bb1f54f6a8b192c98487a8f1013352adcdc6b486c4ba6f2c144c8d3664492bb8b33fb27105cf03da0203" },
                { "hr", "c1b34680b98d2bed2ddf362b9c2ef9aefda662054a114750a2fd25a5939a4f56bf8f83928ecc2729ca578fb3e446af6973f20bc408b84d11a66a89893cbc9c96" },
                { "hsb", "eb7c6ecdc2d798e02755b93320028c724cd635863ce085fa6402b561771a9ecb10a1168ed4fe491dfa5c0f3762b0d53394ef8c3fa9270b5109caae2410e10d34" },
                { "hu", "7b2671248370030a6ebe34cd2b84902eae1c06cb01d2f2b52c4da6a06ccf4649b6c6c438b6a174d7384d5eeb66d0d8b14db5ed3cf62a4b74b62f93e589848ed1" },
                { "hy-AM", "d678c773962de80ea463c1ead54078786e541fc7f4906ad4512d6d282670daf2f426668c1c71f48e9c2f465fede9469c3dff2986db74f7443683600803e6dec1" },
                { "ia", "93368563a261e5a9844d70d58839c038e1cc8d86803f5d7f48068666329d1811d6be7bc953c6ce2734d64e3efbf4a717c487bcd022cc75c60fa49b47f64549ab" },
                { "id", "2bdcc4e89e4dfff68ca1b63b8a5152d172a9751e94a2f55ca795ebaa8f0f1576a63990e8340a0d0310412de0c365a25ffeefb124cd956e9e02dc1e77d0f79d3a" },
                { "is", "1e9301839a3f123060d993ce6ead37a9a51a874bb54f12a7735e29637a758074b43d2f9b9801bf602629a50a93c6393e20d24fc3b1a87cdd074e8ce8bb855023" },
                { "it", "ad7ef98627c3e8d47e76223c6c754a55edf188564236db770bf64e2d5fa720c99521b884598bb20d75a5f10388080897eb66c5fba1304c32263651a7592f6208" },
                { "ja", "8d32af7b8c573f786e7dfc7319c3f1fee40ffce1c60a4a00f76f65afcec56c6c32fe7e2e6e2adaf9de4292e78912d2f838077e5ab30c578541cbd646bc981760" },
                { "ka", "8b6c322ece61eed01995ff9037b552512b1e560b80522225bbc608eed007e98058dd234717f476da47633117855c07784d75e31af6fa08d58698e18c7e020ae7" },
                { "kab", "6062791f4f73303ce64eaa77decf660b5f0dc24480566127cb3e6291b3fff78a7827112a3b67167e87fbbb5c284d4f0fbb246c29b625b823d03fab9602fe96b9" },
                { "kk", "aded4c481f4b55da8a2b1ca256ac8aa933f48aa85cef3fc424f4a7eeececf3597e357b6b7ea5f6844e2f939a72b4674b48ba8d0c67310bc72051318a487f298d" },
                { "km", "69ef7751b3293e7854a799fc10325c0bd36f59ae3e82838c33956b8a0de72676a0e1cdb207ea6664d44c4652844bfe562ee577c37a319cbde55a399db543b0a5" },
                { "kn", "756871b94d961b9aab17653fbdf8b35385f396d6d5f2dc8809a762a0a59b76a1e3c71818c484af93f2ff736d16652ca9d40c80f2a49cf197fe01c70a36aeeade" },
                { "ko", "d07188c9fdf889c04eb96920a94540303692e748856d3500af6b2c7ca415b36bfed57b2630dc277311b5adc60828bb2a7134186c7c23b62f9e9e9ffff773fca0" },
                { "lij", "01f4b01eb197b33cb4277acee51b4fcaa981359e269ee567869f37edd9e5f0f7430be72dfb7f37aa47565fce6b3e023c39370fa28f8b8e2fe7eb348aa7dcdb4c" },
                { "lt", "a9b020ab48f4551c6c3880c64cb6ccefcdfdc25773b5be3ab0d0fda5a22bdb042166967cd03f235d087fb52a6aa36ec172c1c8e2579d7c4ccfe9804aed734cc4" },
                { "lv", "430fcf2de5f0d1cb1b4c7d3452868edb84979b617c7c4e2689781cdba44ce07de5a9b13a5530f7dc9674e2c1def95f2ae516f884335ba5800d8cd14124882f05" },
                { "mk", "cfa7ac7e3c59dbb873b802e1d7b43640d8693f9dcec9eb5bc844e8368615b19054c43417e314af92b6797b1d79a8f7fb1da6fc034a2e2dd68e75bc1681548793" },
                { "mr", "ae94d0fc9093d679d1958bc107caa9e7c1b6922f9373bc9f10d6b564ba8dd91f3a53b092958d88c3e2dba611ad46680cbff31bf89ff138b6a88810e0f26d8fc6" },
                { "ms", "d8d1d1090dc8e3f30fd76f0edc2037a08cd8ed8ee062123ac1e6e1c479db0696c9b52cf53f3154ab6a7de7b7047b81b0001b2046c512f1a7677a8a484ed703e9" },
                { "my", "3ab8ca4175dbea0597122403cb928771e9f2091df9c26e0f5ee806c776c3cf4a068ecbc60acb21882d15db7aada8c9dcb5d8888c0e351d19ae3aba6d8b5e705f" },
                { "nb-NO", "8d81d4284ed72c7b87a66547292ab1027313bf98fdd0bcdc1432dd940add5ce52cec8e5f39f3842594bd6eac981ae0c132d56a39f46aced41a655b3270266838" },
                { "ne-NP", "e4e0df9c60dc2f5a635c90c8d493092d08ad86898f3cb0c902bca16edecaf533d8bfb549c536269bef76d8de8a278608edb40d70189f6850e4ab123c4ff9447f" },
                { "nl", "6b8140438ba9faa5c3eeb7ed3d2a1bd06460b11c622c331273c69e26bb73d881731626c4abaebd78e2bf961199d5fda9a942abe295380bfa7496c63044bfd73a" },
                { "nn-NO", "aaffcc81051107ee6cd7c28d51043c01023be0e18892143109585502e11a9ed2334c31506a5c7a322177d12f632e4a9ff774557b65989c7c0ebb72e63ef3df1e" },
                { "oc", "5794f72909386637daf166088e391544da8f5117b91db8896381f5f7064258955f3e0fca274969e2f00399b03c73867654c34a7c58e09a1f08625c989aaa2b14" },
                { "pa-IN", "9e58cf85f752960e82ad43d46a777a42117f7a6ca142e68b38a9a677e016acbdb1098553a5b1b8198d38e1648efc1465c16780d8f417c46e9ed2967afb63aa01" },
                { "pl", "35c139a4296fbfb2d65bfb9674ac66c09d28aca20d6222ac72fed7bc762d3db81fa7978b9f39f07032f840cb8f10ce5c560581f3a54b1dd128e2dcfd549cd625" },
                { "pt-BR", "1e8aaf7350d76c64a8cd856d99c6712cd013fabc8c0c8c2a03b8f464e10ca09116bf464db1dab770a3c63c30edc9488b7bca33a7125e9ad83fcbb1cc13ac6bbe" },
                { "pt-PT", "4373884de04e2ac18743aca7f0b2620e8f25dfb1d128a0ad6f19bbb3018a876c9c4b2c9901442de62dfc124c804d3bc9736300df86dce831246ea40468c4c44c" },
                { "rm", "e6d88c39d2af4de6a4471ab880b6783d4eeda9f41ceecfd3a6e918348b53c45cc2d20b578da481c0f592bf364db81be0974c2a83e8389acb28db328ed24387bb" },
                { "ro", "61b21dec367ebd65fc287d86394d06a4f02fce2b43be6b9197c7d32298065c859d27ebc66b865a4071da9f8a0b7544321f35e2240c2bf3bb3dd530d29915022e" },
                { "ru", "53cd2200af6826a62393a773683cfb264413f82bfe51a93a85f33a37dc71733bce7e9925a1abe32ed861c0e86cae6cfbad57c2618b06e50778801d991477ba98" },
                { "sat", "bfa2daa3ba49e3487953fd2f9b9a66ebce7e7d17941ce3dcaee6b4a8451624b5871260d08c83fa72cc87ad291d244a7e138e0162d79994ed443998f70b0a0cce" },
                { "sc", "cb4fd5d5945f10c7142f28c57ba22c82a5d92804f0e8cf63b49dee00f2a55b4320e11ea3339e72ff21c79dd656313237314049863fe3b6af314c2614dcb38e8f" },
                { "sco", "3a8f36e18aaf6fe7e20989487ce512e202ed73ac0dd352918f9691886ad5209da718f50eaa83fdc46535abb1c5775621bf825120bcde8dbedab8c20f1d5d3072" },
                { "si", "880d071c5a8d1ab8f945bd8ca10a11ccd71d64ddc471d990b3f9b07abfd371845c8b4045207e068b6602b29d4a38c3f5c98ebae5bd7e76dbb55f1718fb6f8f52" },
                { "sk", "a591db16403e353943d8c678d4969972f0715ba796a8c67f0c93d9587c1716b77006145a893e2a8695bc571a4ae989972aa419cb10bad201cf736dda68802589" },
                { "sl", "98942671dec475f5a55bd858dfd7a3e20063d246ce403f410c8aeb6de8331e48606158da824496b355cfdc26d2978b365f22af1fd2ae5a53eab5dcbfa471d423" },
                { "son", "786fed46a44c3ac07011f3a0f1220237c5aa24a2a11c96759ced30e77c7574beb99f6f51730caf18bad568e095bdc168433d621cd232894ea0765a7073009ea8" },
                { "sq", "3edde5c7f2668304555c30630cc8fa4047d9f2bd1cacc6522165a5cc4ac07ef99c58f9e85f0aa15796670ec856f07bd353c6513943b641a257fe4cf8878dacf1" },
                { "sr", "7f58eb289e7ab01e97905e3d684a48c33751509744f152a1060ed726426470b3fc1ebf57b99ee1776be5c8fb44172b11186a5465f3d46044074fa9d0eef77bc1" },
                { "sv-SE", "8c2c56b996760fcf1a92e459ce61f7fac220c0416c39ef70729e2f095616a3c91afd92849cce8aa91a4aa4414f26abb448e0e015ff1ed5525d1782dae711214b" },
                { "szl", "29b5d5146c60984328bacba4d5376d3cf948669d3cf12f1f3a6302b9b4e0cf2ae6252ccfc5e68da174eb0d4f46eefe343cb4a19ddc1274b0778cebebc687af06" },
                { "ta", "e6a48bf4bc30d7f950b192d46441b50808e3c3b9d26468176c424995da9537700e0d4895e4e94fda02becf4d8437c1b79a3cd511e1096d30da609c9a38a2b548" },
                { "te", "2a965c637da4c21956b07f59122bd7032b545af2250a4a3fdca9a3b01e3fc4e9cab82758d160040f6000aaa69a36a196ca8df40a01c58109831f47469299dca1" },
                { "tg", "195e82a9c602a420f187187bc1f431aa57f0246e205e1201b53f559a4c942a306e0a088460d4e872520f4223ec9e8db7d938dcf1bbf92b85e31a825598db705a" },
                { "th", "7c5bdb210eec671859cb66acfd7b44d6946c31b829c77926f747cbedab2e78e0cbf7bad3681efc6fb31aaa0dd3f10c5bf1d9feec3765f9874d84f67dd9d6a7e0" },
                { "tl", "5625e55e5a67282d67cc1e153339c8317c4198570aa3b70954bb93ed0ec0c9c5ed46baabf26ad1e03b14a28899a1b8a7b68a759be5b96541c58f18e06252f7ea" },
                { "tr", "f8ed4ab40d645000d30d075f6844653d8291b0fa5b15d163fbda5e61009ac68bd574ec79be7e824bb1853c1faae27c82d5df419142a5c8ee631abd8c2ff399de" },
                { "trs", "b90d021afc4bdbb60a3b0a1261de8700d0683d594e9795ab9def9feed74cf48f40c141f5b66e97458749346b31149fcfcb14a99738059be5407130660207d918" },
                { "uk", "fa63d80df6dea28723713070b0928c0d08d6b18a6a847d3903bb52858be8b7aafb75d918d19cee335c7958153a9a390aaa839e58724a9a6431b20974944c18b7" },
                { "ur", "e5c5d6493e4ee6ef90a59bdec47400cd83d8364cd00c10c72b6f99a380271a7adada0d7b042acba0868e87a30428434004ec9aec1d7cb22c525c093189923af3" },
                { "uz", "7a24098c7051f14d77ef90bd440c7c07cc78622fa19ba26b373f004547dc820c1ab48ecb35071e7cafb3b1f6d664722cbc1ecbe69072bc9dd0e1547f999864df" },
                { "vi", "5800cc24e6c9bedcc728bff8479e2c67bed50dfd41fc9caef2a68a16f2dec4c3010d5a5ad04c9263591d9add9123e25183b6ae689fb6cd587288322e7fa90f66" },
                { "xh", "6ec02131032b9500e6aeadefa91f5235b9d0915f1655a58a5d74fa3d64465e6319413f86d8680404bbf6596b6fb753838b69d8e4cadedf9a7688544a2b5eaa82" },
                { "zh-CN", "c76384a97aa970d9fcf915b058d31d778ae6a8e86ba24f74f3cfda8588cc3e7a64bb81e83fc4e224b5681e2240307443e29af30c6ed605c7cf3756ba055c5fce" },
                { "zh-TW", "0f2f6a679a01ab3a186a815af1032e51f48a957f468699e8859c2f8c93454ffaecc262aa62d1b2279b653ece05088119a37b7f55c3bffef79ea9848ff8be4f9e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "ee79155ebda34f4a386a68534e50f0670eec2eb0a77e881a586a6e0834fbdc684ab7e9590b5e8fa472428b9e00efe71918bcf90548280f082fbd4b2bf202ab6a" },
                { "af", "1c59751c3e9459b1d4dc8dfd3511a88d6647bde12f3be00b9f37ee8cadaa4ac9762cd8ce9ebca0085de6cc371969a2f8f9f70c53bd9c5418272ef7bff98a7f1b" },
                { "an", "2a5f0342d5c382c8a4d5c056a6adcda26c1c0f2e3df0c33da16dd391bf9c67827af6da9376d199c469fab3642f89d67db3cb78dfc4487441c646c0839837c437" },
                { "ar", "cf74681be37111531c82a45a09fa71cc2d7ef3db426cefc958fb779a649d839e186068b266492d55dece83902c97ebc29009ac5e6c1a9be30257d3f370ceab64" },
                { "ast", "0e73ce3079d047a2512eb0074032cd33186f1e41b270420079d14439376c52952ebf18061e9e14138a40b0604138b0de2d3f87cc8b23022497144d3707ced203" },
                { "az", "ed71a17dfe2dac3f3c34282b072cc2542a545519da55f366b759e925ea113247a36dc7ab2c6f7a46ce28109e140997c0f39d23bcbee0b39f95ef3d7b5d64415d" },
                { "be", "930c4fcbb4b5275d93f008758ab128211d23683a105020dd1d70952d89e6a491f5499b831195174b82cd02972c7c0f137c232d1484bf3ada2c00321f4b83791f" },
                { "bg", "439d66f9bb003d0f0bebc4b11f375703589daa1ca9488ce16c2b8c265b8183f9cee2b51077b9feb7f050b42705ac320e973c2000437c770228434510cec69b77" },
                { "bn", "9505b69b7052c0e08a449cf56c641fce40f7df6df698b78dbe6116b2deffb3eaddb6ff939d691d6204b47ed32897d554780f7dcfafcd20cbb2ad7ee3e6b83b8d" },
                { "br", "797846908ae2cd16d334ae0e5662d101e3e50eea26c2f5343ce23426446f815d81497efcee1194a9e3b8e4c8910e0d196ada3cc4c6a3ee97581139e05071350a" },
                { "bs", "b153f278920d73bf66a5d6a0d63ea47ed2e02f88eb1302cd5eeb140364a2a02e1b1641fbd80a00976ba99abac1fe42ea975b49e8a92c9408cee524675d84c7d2" },
                { "ca", "65223a6a31adc7fc081e0d241cb14011ac1fa859cb5cc4c7dc52f660c2fa688c206570b42c053df5d564135e01f10dd1cb43607cc7d6cce5ce2e07f7b7dd9423" },
                { "cak", "936b815be5c3a73eeae4d922f56bbf9085eef90cf64c050916751a9dcf3f68ff8c3836c8b3cda48a96a8ac3e901d432572d50c23cfcb226dfbb7afcc536a618d" },
                { "cs", "ccbbdb6c07daa023754853117177bba126834c63f60f8c581264c9d984f7bcc664ff2a76c379dced6e325ced723fad1d587896c94324d055790dcc99a9f35848" },
                { "cy", "c2c9b739830fe535beea379e90c5e8484edbfa897ca317aaee3dafa58bc560a10f05271e313eeb143ed7d9c14419d5277e7d768992e928bea9b390b87067ab75" },
                { "da", "a4e810bb135465547d9c50e16ee51e5d573e7407f242070feaab9de5e0cb893395184197f15c4c5bbe2fde199ee1ff9341a1b44c5c700356a8d118069d517919" },
                { "de", "c959236ea04ebc6662b12e3bc2aa0e8d485e177e695ee2a1b49f500826cd5e7a2729d5515b83c9e5ad99f48413a209bc53deb3f93ca8d70f9719bd15fe22c56a" },
                { "dsb", "b22489d398dff52144c6a3569d082fa33fa1a8cb9ff19d5ecf94fdfa7c79fbda7047bdcca2a27c5218c5ddfdb53f1f9d7ce11eb2017a2a2d5041ccd9c8fe43d1" },
                { "el", "b57d9f5333f6b183614c25a627df33f2ae9f86eec394fccd6d8a88acf41711075aa00674c47e7ac39ad6218a00dd3de3e600413a481a56df4760e9a2722616bc" },
                { "en-CA", "c8013c9196a6c92242c0b571b78160e8067c66162340246e0664d54a69fcb9401fefe1e2e60913755d41b9a30d64db4b5ea6b663c3e7302e759d411db444c472" },
                { "en-GB", "aa33cd705cb2305dd87b81d59753b19271d2e04b31d11f4ca49fca36cf302f5f507077f86e7ac432c8e7c503d3018ff165f0698c5a861d6268c194f55cf46997" },
                { "en-US", "20bfcc8b7028af0af73ed316df27a59df783839ef13e72d14153815b1a70ad0fe6e128843c2b6df16307e6f69cb9a0b2fa0ade2b7d2f8c7ecf5186e178974217" },
                { "eo", "18b99dc788cb43c49efaed346f47d1ae781c75965a3eeae5bd34e702f8fb84d17039b4aae959015c98aab3cc4474f04d131aaddec6e85d7d2786a37c269eba92" },
                { "es-AR", "3e25e4b18c9e9b7ee97b23c37440a0a591104d8a6bb925251c9d40178930054339440a6dd2ada988444a7b1df90998cc34989f6f35585f98ddb25007b68b1aaf" },
                { "es-CL", "a428be86bc7ed947b76600bfe29e13424195f7f83d470e7bc265acc753005afab9c084489b393dc07037e87f66dfdc0926f06732eddd657f1878593474ae631b" },
                { "es-ES", "672887cf673d74d37ec0fbd83a4b8e64ac99c50878785fb3b97eed041c83d4d33fdbbdc281fbb10c53136a791bc6a01d122284cc864b4405b4e8352cd425b5ff" },
                { "es-MX", "07bb5394b263e6031d9abd767b31a1b1eb06b90ee511d1ba25c93fbc9c77db5730abba36aab2a028461e881232e4698b03c5000845861400ae8288a4f99c827b" },
                { "et", "2e91896d633266e76d3aa1e9325662555c4610fd7764958b0d4d388eb0e025a9da55b2c6d82b82a40c7e84d05766b369b135bce7bfbcd27b3928fc2b93e406d3" },
                { "eu", "474f8b4dd452e2a3cb6b91932a3995e18bebb414e4f1e0a3f00e4a62e0ef340224d4a27046e5bee3a0b310ec47d70a22c55c3c059bea34d3cdd94cf1d0df3f63" },
                { "fa", "b6ae33093d1d83e652dc4af07a9e6f3fd1ef15924f3a13a4df6766cba5eeb2eb1feb2f3dbed6f5001ba187c6fb8c468688add4249a0dd626738a3f65e74833f5" },
                { "ff", "36ef71f4408368a30804684fbc6594c4cce195fa35b59ae5fe2084605b8c70fa0518c590c8010df677dd0f5bcb7b10dd5619e7f07e9e70b492d802d1815f310b" },
                { "fi", "7bd2ba4a3ec29b0f664599cbec0d200475940e0f96d59f375ae86833f40827d7faa128929ef71bddf27fff23c0a5129d03597795907d5f42e85f6b987aa2c26a" },
                { "fr", "6bb0b8294d10b74ee516f5da413002d0ddf812dd1d11cf22bae56d64280e48195656d9ede29d07ef07721e5ca2fb1ff17e0651b69aa5b9ef2573638590daa841" },
                { "fur", "82b5180388548dd33fe98a3caf2e1b6b0409d6a04dc47f73048f4b5adb213b36f24b4b6b6fbf87ffeef30c9a73db41755eac4b50be3fd672733e6627847562c4" },
                { "fy-NL", "342e0a4510ffa7a03d3bdb5b21ebb95cb88d1ccf4a7f3cae5373ef5a1ea66e61caf3b5711cd91a0fcf5230016255a758ae99ffd449e2a3a8769b48578ebd02ec" },
                { "ga-IE", "db73fc4fc029231ef1aba467b9918f6ce008d0fbc1240c9c97675352dd7350e8a1ec5fa09c3088509813bfe46471fcd91fce28f99ab68c09aa66ba050f5e4590" },
                { "gd", "e5a7dafacd158ef4c10f713ddd054dac240f1bbdddded0607476ed82f8e2d40a9592a0d67d522aa566430a35557b7695f72d19569f2d35725bed19ea575409e5" },
                { "gl", "23757052572cd99fe2666309e0a9c8eafaf1c24782e60b98aae232f56277c69b92049295d4c375e066d14765acb05b962f967c429c5232cfd66d446a094da0c7" },
                { "gn", "27342ae9dfef0a9d378a4b56c019c56415bb13e0c99ca8b3e57cf156e595851f7e0d0b276ac6fcde4f2671861c08538243b17cfaf6f612612a6ed635f9cb7dc4" },
                { "gu-IN", "3c2a946328cfa7a1f01fd41fcb790ba74123ad65e0c5507b4f692a9e1c1c09628bfa22a6289a34a280e6935b9c58995837d5b64083c0e98f1f9e9b69e99c511a" },
                { "he", "4dacf8e3754b37e3b9039d5dfbbc16a4b3ff7e68f4e250472965187bc5d12a357b4cc8a8623dd8049ec76406e7841e85df043d92c938e7534b92461c1abe5708" },
                { "hi-IN", "d9afc43dd8d8d26935240913494343218b9bd07493a85d34d03b10b4afaf1c118bddc23b19aa50ac6f48ad13a858fbb60172bda3ab0798ca0d44cfe3a70b69cd" },
                { "hr", "a4fc11752a528ebb2367eb3f71840faba5d788c7b92858cc6f4caf9f9d3f7132854a5bb0795923cd16752c64650ee78f61cbf3eafc7ef2640b6f5a499bebc0ce" },
                { "hsb", "1b0eb5855c8cc897194f196461cce48ac36b5c824344146532823a1c490ae19c3a71de60074b3e4cc0a403e13168b6720668b7bb1daf2c62b841aec654eda37c" },
                { "hu", "c4112a04d4ef5f5c9cb603bfac5703a718dc4e753197c7556d1fcb9c252096ec500c974f55430f69087933a495f665dcbe46ea67d7b96ca7ad2701ef10b0808f" },
                { "hy-AM", "df15c980e4586b701675f789ae5863f050daf51c92df6e5908bcc715cda940b85deb363fde07d2c1e02f2ed310bfdf9bde98db036b734c077cb288b5a3fe8b55" },
                { "ia", "3e244d04da1eaa27718a629bc525a9bcb6f1af36d9f3beac0f45338d43892a76edcfd68f122e135e1e3d5462a9b3ed4aad67812a048db091802f37f641098022" },
                { "id", "41904b575ce4700b8387531ea40256924a52488d42b7b54b23dcf6793e6d8a57a69eb2fbb6f951defd5104cf8470d7ae44551e0c3308ed6c91c86a53670c53ad" },
                { "is", "5f73f2b19da079b9547271a1edb9729d92b7c00463cc4e908da012d4085b01cdea220e241e5c55a139e0dc4c7c357daa865a5b86348e96aee8b4a55c27f781c7" },
                { "it", "dd6ae3e716d7529d5ac83b9560b098b8737324a1938b972b62eb379c12cb8d06d01520594826b16de0b27392a789dd9330fe95b6233eda5b2f4056e5efa69a41" },
                { "ja", "491f05dbcf9f06ae6149bf5085aa7b6df5bf9cffc9fbf0a35ae3cb7e76cc2391856ec271f46d0072d01ea79cb01d6861259303b69f181f9ec2ca0caa61845d38" },
                { "ka", "c5742ce6372a82a217696437a1cc6f240aca521b7e705faddcbfeb434bf468833816dcfa55f7e7180742b87e09d70e924d9af2f3151a4287db55e7d24ce7119c" },
                { "kab", "f46a60b14b6fef4c8571c2f3647052e0068ba8f68d75c7bccdc23039e57df508e4548c3a1c90b1dcad9e4dfa1af1b3ccf2d824bc8b475852af3deb84c2d154ad" },
                { "kk", "7a36819278c7facc498fa982dd3709aabc6a6642de6d48e88a5c928d3a6cdd9f69278b9e4e50c49a6436250cee43ccc2fff853b01d2d3cefc115c4748291d85b" },
                { "km", "da3349db86a5a5afcd57fb08aeb6cd24e2189dc6e3e7fadc0ff505b4cf7cd86bca0256b14148ca17c020857184a31aeee2f8d65666e6bab192e3228b0d55bb80" },
                { "kn", "f4b8adad61768c575a65eb9fbb2591e9e995e0a02aa7ea1029adc73839164b59a1302c02dc0f778deee1ff8c2c865dcdf1a50e911813baa4320fcf87fc382252" },
                { "ko", "c4d090f709e6f3221cbaf4e087688d21dd710fd6e5d37b03417153fe7f922d45572af4ddb2ec3837d53770c95b80cbdae59f73e91f9325e30fd99297ea1fa058" },
                { "lij", "26a2f95117dde59d86eb86929c646eb8526e00e0fac37bccd96786c345b6b134d581e63ddc674dada803bde2b9c32d35c85cf2257509e760f6bba74b36bda52f" },
                { "lt", "717331e2bcf78a4df603cfd87a9886f90df7634ea26869e71b61684209fca3a68019ad81c75f5c19040975df3bb509c34c5f911201741f8820ae11727e915f9c" },
                { "lv", "2f5f66a14611afcee3587083e12ff8047fd27d9e3d63f129cd84dbd6a71d9f3610301678a197d7d52f85faa1dc6409ca29bcb0cefc5fda4757c1e822a9e894e1" },
                { "mk", "f5a8d47b7a1f2c4b3f773d258283628bd018a0b4d1b12c2e53b54dc977adebe4c013de924783705684a749db952d8baa8e45b689a5e676e049a1d878238957ef" },
                { "mr", "34910ae58414fa879dcefb745705ace22d7d01cc0fe7fefbb5d2e2b212bf74e8f2fed25a6a98b34e59c59a9cf507c00a5f5489ff511f9b59786ad2f1d5aa651e" },
                { "ms", "bbbe6187736a07a9e1da7a721e748af08296db5ed79e88ec4920b3bbd1f637f084505ed9a734498d582856e267c09d31bb44c979f1d07371a2aa246fc0a04b9b" },
                { "my", "4b4b218c983d67d3290c4ad9cc8d78b81481f88a21b871807395ec0b1124057edd61d7fb78ca91673e01d532798fe01c4dc0d14b9fb208759b9a799499dcf95e" },
                { "nb-NO", "3a5349829617e53a655570a0af8ece0552e04096a38a11f64bb0a991f7de1105c47d5844a6acdc6e730566a08fc8349a7bcee8e8669572d173f5199ff881ff1b" },
                { "ne-NP", "a94248506287be48e2a611f9b7c63dc05d33bb228f30e5990e5ad9cd81dcfb0dc11e4f90f4b919e04cfe7d711c0f7390eb7271d4a82c269555bc12109803f204" },
                { "nl", "73f62b1e4b992b431c199dc82cc1529868ee695f00bf28a6c8441eccd13e0422289b42e4c1d2d435af71ba522d855812ac071119be2b406bc93e7a06c2ab69a5" },
                { "nn-NO", "c2f081c13ae9d3ec64430784d25ba9345c155de93a3b1306b8adad7bbd3db84b72c02d7efc256ad49fb8b13c63535789739cca4ce8bb72cfba585946daa87e96" },
                { "oc", "543e4ce4366434c1fc6d8f7457a58dc49f25cce92cc5fecd01902eadf667597d8e45c0c40410b2dfa617e1a1efc3c15d5eebbfb7d377b9b6ab7cc1c764efaf5b" },
                { "pa-IN", "6fbb4c674aa1240f686289a50daf1604bf78c4121a45f9c72063500ff8ea4c651331ead70aee0a916caab95e2e010856852c6281739087dc0b4e29f5dfee2efa" },
                { "pl", "1b45e34720eea5ba4c0ed9f48358b3edd791d5a4bd3872d23346ab81376ae2019ea7e101112dee50a6c8fea6e10a4d2f76e850e45ec817f1674c1c7c284bcde9" },
                { "pt-BR", "c45737016c1c9a914b61ba7e6a20ebffc231ceae09b4cf2b469d64a78dda79a70794fe762770d219098de1f94c5e75601ff761e5bc348f3fed175c462e2a4869" },
                { "pt-PT", "e5954c6fa6d33d282121acd29c911569dafee382acf4ffc13e8cc9a09567a169786f0195bd48df62d2541a8d12d7a44ef56af9e4af587203f7709a71c6f644c9" },
                { "rm", "eddbc04a034156f6a9a67ce2c45b4307f6627645afcf0e985af47364ffd6af5c0b547ea6cc9c370dabe79c6a7619d7e68ef0cf03f621476a414a4acb0e177d22" },
                { "ro", "64bd2e061c323add8e8f025c0e6e4b7934f83ff4d31035866ea72b06ec0ffdf5ebb90857e5878cb8db4b3250f99a26fb563816fc7cbcf118624c14709c727cb2" },
                { "ru", "bd0136da393c0334fd4771f66d8c79036b5be951dd576cdfababf365d7446b3f1e8b5b91d1343980e343601bc658635c4e862c1a212ea0f5ba358a2db596355e" },
                { "sat", "32c92fc4009839b0aaab694ad772c35b273122133574ccf3c97faed328f5766e33cef6e39569b0cc2bd78f3b6cfea0dc2d19f62e9292aa803c54b2ec436fea7a" },
                { "sc", "c1623a2b73282e8ce94587f95d494095a63b32c89a00a9d85dbcdb2a7cbdcd1f57d5bf930c333868130e7a9e57cce8c7cc25422262b2a9ac9f184bac1b60a823" },
                { "sco", "a6fa9cb3efbfcc08da7af6dfa9ca8a532851163106b09e94b96a10dcd65f6d648ff02a4f38447ec287564677a18bbefc728e2bb692c6d0f2954493b959835d25" },
                { "si", "fed3ef172e31f3aed4ebfb11955268470731481cf07b1b19f926680f10356cec86c26add63c5f57a64f21ab5873773f1dc50c15380c65c3a116c590f75aab062" },
                { "sk", "eb045c0efc7da9e8f71916a0a46603f2fdf73be96969c3e5fe31cd3399b8c037afc566385d78a86bf63aa77e44aa5752d62afbe9e514fde2f74333c5ce1ef715" },
                { "sl", "938787854eceeeb47c6114c1b15d701a9a99a8711d37719234ad974b02f0176d188312ff74bab22c48894bad87d129edaa1ca54259ec8831737b39691e3caeb9" },
                { "son", "24ea59cb815c04839b99c99ce1edc03d6db59d38cc1e85d7473ae805c8521f1dcc645eb2b0943280af518d2c06ffc0e572abd3250c8d13b4d2373aa34f34c538" },
                { "sq", "2c75654bf1558f95db49465440e8bac610076293794d9dd1bda5b167b4afc18d8389728fc675be0033b15661d18e49aa1c72faf9400b3bb55f045f346b3f5f0c" },
                { "sr", "4096450804aa56637f882bb25172bd85db69566401d4f0292e4bbef008a391f32f57edb548df9a27b049c30f56a267db1284f2583006c39c201fbdf0ced02662" },
                { "sv-SE", "cccfb6e301c4acc2850fc90f9d58d7d855434547aed9974c4cff42cf0ae9db8bd029d142969b651dbcbc591445e02cab9d3915b2a28f36183770ba356d777d80" },
                { "szl", "4405ec1eed627672b38ad4df2084c67222da6384585848173b401e4bc684016883b6feff834b668dc604d304f555e506c58af5f57e2c57820146a7dedc54553e" },
                { "ta", "47fd3e8f94248ad6683b5c4e7d7cc00caba2ac0222e01d02c0499cf0acea0e9b670c9a22b82dd6f0086b33c41902d65d82b41160c9a70febfe9f0ad6afe63401" },
                { "te", "2a93156be7de3318cb02a898324cf79704c0f493fa1756c2983c865792d0805d084eb07aa90ee0964846b74244bfe01e1d0623d1e8f2a3a6ba21a602a6199c97" },
                { "tg", "dccba52ab9bd762d9226acaa6d6e1e7d58759cbfdf3471e232a9a9f70c3b59e02cea94bec3b4497e2344f9ec89a688db696821d4c26bc0d3058984fa38644cf7" },
                { "th", "bd614b8f50c9a26e17c12e4ec42e070dfcb925c4a26312e21bb744ade25ed489d66e06a2d60bd9ecc5f6964a9189e01ec7a921130a071588c98aef3b3f662002" },
                { "tl", "be88279d927ada5a852da04412bcd4d15c0e87daf3c1b9422d19f873112a97151accff6071cac4d720177f2d0b4d1e57a006fa104781f72772ea78af0360abec" },
                { "tr", "68e0d292c10070b461cca807aa45e09175684ae2b2cc68f811d22b6aeff99d5148c5abb7a495a9ac95fa0f5058972b269c6c1280fe8735928e48d261b65c9afc" },
                { "trs", "d7a7197862fd10e9e6cf9712f3d696ef44d9b7c0deb3f05c54a18bff57478c62e7a11c8542aefe8f87f9e909409df81cce409c0c18bb34a5dea8f46b1682b6e1" },
                { "uk", "ed2ad117a8524c91393ae1a1b7deb1c2e237788e28dbb2920d364949a48e4f2744beefa7cdb006bed1d50db9361a1bad6f1646b5128f6633651f81438dbf653d" },
                { "ur", "a1a849ea4262692683e6aec8c1369567ab68531494538b5fb94ad3762457784824bed595e5c6c99a51149093b8d7eefbfc8593fe129873a72ed41ca88054d441" },
                { "uz", "9e3479ab5936423fe1ce44baeef88b79a63cb7bbc67fd18ea44bbf73a180de5e6a67cf2c89c63a47df791e481acd3b7409eafc91a5b27ebf71468e0940d7259b" },
                { "vi", "de592d14b177b306e3c4df0cfb07c43277ff707591671ac50c93c091586d2b53509f5e494deb41ebfc42ffdb2d9dec838187a1c7aac3b29bfded1656b861e61a" },
                { "xh", "843aa418c2c37a00c5416a88be3895d71fb9d669e6f0c1c466b3bc79b5c3db3226fb11538f83d7d9549a2061c9c3dd315103a90df7e592c63187d6923ccabe63" },
                { "zh-CN", "679999ad619449008650a371cebd531ee6c80a279fa07132501225163b44e4ff0feb97a00545ec4d66af0c4697705e78516777f58765d0c79205cc0c147f9090" },
                { "zh-TW", "9c94aea300210348b13532db7e6527922d462dda83012edae93635d4d5230c0c5494e175e68b46f39679fb6cc2997b85999c7b6b3fe2e4f389cf184cbb192380" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
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
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
