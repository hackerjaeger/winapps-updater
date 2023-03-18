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
            // https://ftp.mozilla.org/pub/firefox/releases/111.0/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "4408a9cbd1dd4d5a3f076594760652d23ce5dcf0745f296192e6d5875a956674fdb200e05600efc53b9dd5b8accad279ed7c99e81f0ef6cc424bdc573875e222" },
                { "af", "0ae72a488b419768c941435a11d5dc8a7323d06aa0b3de6d43c2b8c42e9b9ae14ec86e8b738a16695ac697beea64f433f7a461a6abeb107edbb5f4a14b26145e" },
                { "an", "8880a34fb513ccc56bd8d34c766b1ff57586dbe583d4dd4b2fc13311d89a0ef9173a3f53535e46c8fc0e0acabd98b9d258d9778427fe4ac9ea751a79e2db6e46" },
                { "ar", "347750726e918e8ea34c0d541916c63a297f69d3d879c3b8cbe48a216158f8154868fef3f96b07f15777fd0992d674b59b2277e5eb4c374db804a355b2c7485f" },
                { "ast", "31af00be863a50d08542e453e7e69f875e2e0923655fb9bf509c7e6d54205065f8abf072540e3df54ab01c3ba1bab3c6d22ef0ebdde4ae70c139cbc4929fd029" },
                { "az", "054b5efd2238b3f68c0cccd8b7af41681d3a20b523bd61546516c4faf403221de213882ade3a49a8081c2b4e2a4a334c0d5b257ead59fafe4e81fa0d13ffec3c" },
                { "be", "dbf954c1e647e65ed6dc0a4270fcddded0493d386c438d6c5142a64062950ea85310ef1896bcb1058915402777b73bb78f23aa484ff5509f0d217e1bda484b9b" },
                { "bg", "619996139c4a63ed1fe3447b6dcb49f976db96b9a2c5298027084867e6386f1d7b5e78a62b90dadf8a28552363dfdb7e7fb8cee188254532faef6838db4f887e" },
                { "bn", "80e533dc3173573ecbe682d94c1290b14c248d041533fa1afe66d6d85fdb16660890272846653fda583a7b30d71cae38945358bbb69eae589cedc08fae8d277b" },
                { "br", "3ceaf6f0ccf845a80cdd32142cc4e0d6057c522f2548e69819dd8cbfdcb5ab10bee38de20b4b8de85d0547635edd0ea1508f26917a783916bb045274def051d5" },
                { "bs", "8dcaeb411dc318db8f0e4ea12044aaf0fd27ae6c871660823006a6860d7795dff372eb5e827b6ec83f62aa8206cac9c84b6f1bbe8e7ba27c6579e67fb05765f5" },
                { "ca", "d85d7664910939622b7a7dc5c3247c0bf1c702d5fda69842af2c19bebe7f93d19ac046320a37c67c711d9618f774c10c830c5c62473d20beab4dd1f68aa2b686" },
                { "cak", "853e5607976eccd9c97d3f6f4001f1f48668832997865f7e2bb3b08d9eea307b8605ed702928796d20330c113bfdeb0301282a4a94fb2cf0f24e64ddf878228f" },
                { "cs", "b4be3c0a9184f97da3ae14a9da36162b08478c091a49a2ad40900abe034847623191c38f754bdb9e2512deb5bf28d054d2e897d333e7758891922f2f1d172371" },
                { "cy", "34fb6af74344ca6f5222117223a27197088105d8fa9c3862e162c942391b3680f2e33af1235ce0da167a380170c40adc35e97de26f11d78c421b5f2937fb22b6" },
                { "da", "7f2193e6d8a564520d8e633983a5f77105ba3c7f0574afe4b9c1e7457c5b9dd37a4e747571ddb304e73fb9998acff63fbd540573b5f209ec9f216ad0fe161f19" },
                { "de", "dbffb036286b5a2bd9674bdd81deb4cc49f7d008300e1e435c5a1c37501621654c3b9d876f6fe4214271d18e03f0fb245883eebe6c4f57890e77940db0eba1dd" },
                { "dsb", "dafaf62e1c6cb5791909d689b2c291f6d6705ba74b0e224247a1574fbc453d34c03f22da99af8ece6b6898b9be02cfb1db5c60cbafa29732cc0d284f60ed8ac6" },
                { "el", "646a26ddbce472dd3eee8699f0521a6baba14fe82afc9c741d329c34fa852319541cce11bbe1103eb06abb5bc9aad640dea3bd37d8ae4524cf445893f147f7a5" },
                { "en-CA", "ed020d18557829df64316ed01e1d4ed54751d7893cf88cdfd89ceb32da9edd08c0f71cc3a6d69046796993c11e8c9f37fbda5a332390904316c91c1374b2dd70" },
                { "en-GB", "b03e4f6856635c1b86e43fd3b51af32455b61272c5e6b7bf65ff489fd1f884637760d2bd76d20dcfa2eb206cdce2aae174e93a69687b85f68ecceeb48963f74a" },
                { "en-US", "69ddaf00768bb74f88ebb7b38a520c8f449057636981924663aa7705dec8a113caf5b591d4bdd512ce3ed639f402ed847681f02f4b35bdedf5a29d26d3bf5371" },
                { "eo", "efb8ee08dbfdc9bf07861a77bba616343803f7312e557d0daa2377d0dc5b19d695ac62ea6a445699ba481af8ee5423c1eb21cc6c676140acb7551930d1e75a19" },
                { "es-AR", "7f5a754491a357faf28b6e2c56dbf32e1314a7014a53091e9f01201bb9bd4387e7c9703d69d3fc6886fa6cef6f06a10767366140acb865dea4bb555356f154fe" },
                { "es-CL", "fb54dfe20e17d85aa67b8bfbd4105a6314a3e4d1e1d4210d43e9fe8abe90e79513f2c6671911220570a94ff9cbf4fbbe1a5cca930c6e8753b31c4db4aac929d1" },
                { "es-ES", "510e106b4f5aa6e2a951d36a3376cdac2de0c3cd44f59126bcc8e1fdfba6b0276d00664148ced2245c53a510ca36d3f0c2982e867b1b65a78d5fafef3f3af1b6" },
                { "es-MX", "c104c17cbec4089ce0aae158241ef5f61a0c1d6be877a035b615dd5dd9fc461a700796e7fd4a3f9b231db99a451415c01dc5880ccec8fdecaff37bb8f7067468" },
                { "et", "b31dad3c31528e6e4762453e9219b886fd025259ae5cd840f7fc0e4cb69cc783089576769495c99b6aea5eabd5d1ca311074d73fff228723d50a2d68b3d770b1" },
                { "eu", "49ead9e1b8d0762236bc1c58a0538d77b32dff01044b6fcc87cf83cc1e1caac55cf067f7c295bd90511cf26079a8756b42a612ef1802256a932b90476a3ee2c1" },
                { "fa", "c7d15b939fa2c6487dd2ac2d45823c1310fed321600e652778ed2eb0646e0717cdf844fe10e6b6464ad89d3d3476f3dd24b2fdac491e976eebcefa0dc1d78dfd" },
                { "ff", "ff4300f4cbab2d7ef5d5c60fceee68ecb975e9b898fd0651f680f422f004916caef12dc8a5469fa090675eb5cec6f06f2cc013dffbbffb466d911987d674397d" },
                { "fi", "09eab3fbee5b669a04cee1ea68c22d2bbd176ce30e0d3732466d8fec2d117aba7cb5bac357fb053bd6447166fa68c3531e5e076757eac6d32f70ed376df6b6b8" },
                { "fr", "7c8a8c8c9655134df529772ed9502fb4eced8178c7ef494b6f33effd4dd9026027de82ab8d6e44810dcb54b1512df8b6190fc55cee95fea9e24fa4f23cc1763d" },
                { "fur", "19e60a66a7677375733f6e84a8999a01afcbb5f1159b6a564f7b07457c3bfe48112f621ae6b1e92c967dfef2f8d54e163e5b0435200593db052f1cfd6aa013ec" },
                { "fy-NL", "ae317c124a5c50077f3b18715d0ede31f1a13fd0eadf449770b9b69e24aab67a7580422f67931c195504290ba49bafcd5cba52a1ea3e3d679b39ef71801bbf73" },
                { "ga-IE", "4509997383a74c772bdce501c546d3c5642e485d96403b661cdc075db5f266dc2583edf00371b408473143885a61759cbbba6ca460ca1f9e9d4a7073f2efc2d8" },
                { "gd", "a53cbc44cd1f042d796577a8e2c6d5c26733d9cd4aecd59e81a5c1ef71e9872a050b51a4efb9b1dd25295f8bad93144eabd831bd5d8121080c37aa19a92b427a" },
                { "gl", "d9a951b9746762d8448c48e275946c880a430ce0818e870ccacf40a2444e6a6955f23e47bf9ff0a7260a627a1c980138f4694e699fb4a2a99dce4a687e070ace" },
                { "gn", "19e6a7e22c5ce7a138ee167aed6d24d023624287a4364ab0c044df4a8c7db247e952c676f0ff48f0a1373412ccd7301b835434f30f5ae30fdecba72e0d3406b8" },
                { "gu-IN", "d0943f2fca99820abc1ba58dbcc7caade48b3fb2c070eae968f1008bc0d89c72f0e6486e4d9517b68259b05d27b3762506bf90eeabb2b2731d767645e79e12b4" },
                { "he", "c16018146d1501680cc714fce3049d574d55edfd5bba96d58ebae0103f8877def62b3cfc64c08e2beba3664131fff799dc8fb5a932a272b705d3c3b0fefe7962" },
                { "hi-IN", "11fdc9a0463bb6e11f6505386045ecb563a46e26e36dab61aace54daa78d0142c59c3f7bcc3e449715d98d0698c71d24e20ba541d584a5d21d9728b66392a689" },
                { "hr", "346c7c04b7f20b1bf3ac1567527cae42d47ead37f6762aaef829802f449b039d953cc8ba4390b91ffdd043f429de35adc95e526636e0ca53657d7570b7a55773" },
                { "hsb", "568b24dc988fd8679d1a0f292f92f5679a1857a20d8cad60941d8a42ddebf26402a882c7b1d76e7a0ddc7ff7d85d2389a08706ac12d10aeb6ea0cf4b367181c5" },
                { "hu", "96d5ed3683ed104cb5d3becdbe1599994ae004993fe2c5d3a5733d0eed26106a706a6aba369352c4d78a9491b8d2d9427ec069d4815ec2929734194d6eec1e10" },
                { "hy-AM", "4a6f2801e9e06897fa3b13f1cb8b36eea32cd37fbadeed2db4be0937ec99cb5244e615eff0af4fe4347917d7f7543d2914089736f64e03178792142c273304e0" },
                { "ia", "3dcb82b19385af3ae0c046ebbd22bd0574b0677504576162dd3692f6fcc9c829f28603123832553702b07556839592bae6ab9b63aee70aeb664c99745828d9de" },
                { "id", "e256bf0344d599b0b7899478bd8e238d56b7ecf3d988b6e28fb3b22805b189dbdab1da6937f2d803399fb1058d19ae0be0af8a7a3563f625c51cb93aa6bcfd78" },
                { "is", "54658fbd47bb3b0aa188f6e49f5296a47f5266c3037d16616961af6b5afec0f91b8329d53b672c0a5ec91ba006bf34d147bc2e290d30cdbb7da59ea59099a101" },
                { "it", "e4f58a8c1ac81fdc7715e0ac8c30145656ebb0aa21e97746993f80854f1f68777bc082b4ce75813a83a8174f8bc75b31960e2283a2223cd96f2928f707037523" },
                { "ja", "1ad46b20690b80036b5694f81a7b153b53f3cd5e9daffe144adbe2d67fdebc82ce8265b7b3c507332a594bd2ff2a33bec7ad8fcf1c576675928484c25e8c9d3c" },
                { "ka", "c4623edfad0e8161589483b9152ce4b8fdb53bb5571a7389eaaa2aac43fe6a97347bab1f0fdef206ce934ee034ecb74062c05ee0c0932ccf3dd951b0d92360f9" },
                { "kab", "57f095d2d2f4d720d6cee32e0a0992abb3190fd0532075fd1bf1186391f2b58c107587f2c5f67c3deaf6f7890551afb705d7b20596609adfb14d60e00583ac04" },
                { "kk", "76f1d2b4c0336079ac9c367d217ec0553c918414cd204b8f464dc09ed35f802b4655e26a02f1229bea7aaa4d035502f6d0ab2c34aa9fa4ad506cbd4c05062377" },
                { "km", "9ae4cc9085d17bbb3ac586ffd9db5d3e6efeb9cf05f4b768575e0b5cc717a4f07cbcad474b5e1d06fdcf5783b875ac93d46116dea5fe7a687774ec6a6c0387bf" },
                { "kn", "c00a3dfbebdb78aad56557d385f33bed523ae3171c619a60b59a2625edf51bbcdc887bc6346816a2b9c84888f44163b48984c006a656060ca8a71df3dd7a3c72" },
                { "ko", "ce8d6e40d49e81d23e863bee3ce4c3b31bd6452b35fdf1af6ed453bc280f3b3938d846a40f98f9e6ae2d88c524c494fc0b9534e8f58948fe2f3d2b35e06d4a00" },
                { "lij", "014fffa67e87de9946462fdedf87bc12a9f151e647908c51b67089996f90a0d3fd33078e6f3bac2e516407ccf701389674403a4f145f94e0bd0549313ab77062" },
                { "lt", "f6cbda6cbc6a5f5f454cdf20c34c110baa370167c15f54077961ff83162fc0b012de678aadc36e4bbd2ea973969f63e82f1b2bf76d0b0d93838240b3d3b164f0" },
                { "lv", "3ec942b4b139be9a1118186808e017e84cd1c1f399154bcb5f0981253b7df11c9da0b6631177a10c6f3fbf79df21f259c2ae8cfcbf15e2541a65e506df270ebb" },
                { "mk", "7e7e2a394d42659e38ec41c164191c0efe31f5c5e29eb2c492970e9def0506da6f46a9aec7cb4fc4811b3a89971b9b412fcfea00094c96c5b3e53f3c21955f55" },
                { "mr", "604052964c14ccdf638247c16ceb6b98c28bff011beb126403df0e052ad3a4dd10be4a6bb10834d39294454f1fbd57bdb2401421c94bcabcc40a51f16f7b5ee6" },
                { "ms", "7df7294adad767ece83d1c3db255291bd8dca9dccb7e765c2866520ff063c820a946516024e3b0da5fedf40101f6acbf4a2829ddb16f50eb3566337a2faca53f" },
                { "my", "bdbd03378d9cdd207c2064f4c297bd97a5fc555456fb82545359aac9cca1aaad812d886d4a4a9146d633e2fee7b4e884b11746129b68d762782643e2f05369de" },
                { "nb-NO", "27c3781bbedf0b2eaf7b092571c36b5c3d031be96d569bb9cb1ba441776e358e61c0a96ac385b7096f7f4d4708ef291ba34095300501b03ad2e376f777d1ae0c" },
                { "ne-NP", "e429bb78822ae0f327e8e8eb7004393807d57716c423a8e04ed558055dae33bec80afea7ad8bfc4ab0160138263ab98d53a1d32c7078064580d386f9c792b4d3" },
                { "nl", "9e337fa4a174559b472d8c8f357f25f03362eb9f1b29ff4031de5c4926b963dc039cd30aee3ccb59b6ddf5c99f40152ccebe5dfd875bf13e3a17531ccf547173" },
                { "nn-NO", "f1fbc0021e1d9997f8a521a624d4a18cc4569b9428658d21db9178c9039f4a9f822940a1c8d0b3430c77e6bffe4892665f5e16f848f594930d241c7105458c25" },
                { "oc", "96b56ed01f5129c100b90862e541ddd1e2db6c32b4b1e1b16e32ecc34fd18eeaba91a5ecc6a5cdfa2eade941a786597c0014339b0d05293310f2ec62cc17ff1c" },
                { "pa-IN", "1e8555b23cfa4344da609e6e57d799efa1528f9d2ac4835fc149cbc596fdb4f31de39d7d03f69a9534d0c6ff5ecbba555a8d47eea28584f1507339853424b401" },
                { "pl", "46c755c1e6ce18ce4c8a958d35914181bcff43723224e8c180883c2e336a600a22516e98d7e1170d08abd07c23f0d38f690315e542e19622c6beda361779e2b3" },
                { "pt-BR", "57a27c3644de62547d64cba50a5057fd1c0f8fa9efaa5d175265774c10c78fe7d9874bac0aa5fd4a9fbf0ef6f66127c8eb789ed20024041e83feb06518a00eb3" },
                { "pt-PT", "7c98e627203264a14d676e72b9c9a418da4e58a8d8dae13e6be675ea2ccdb5099ca8155cdd345976ab702ea79fdc7fe6737fdb33e72a0c18cfa1783c3f729b55" },
                { "rm", "f6794d7e77669d055b7b5b65880340cdfc5e14519ed84ff4a13cc5e9a87726be84a80f78e03dfa11537daa89cd5b5131a89147f555f1fdbe33c39fc9e1fbe56e" },
                { "ro", "580e8433f5a6c6f2408148bc86e3f63274ae324df520183326d71152194a5cbfe8cdfb2b7437c04e75f797f93dd1b2314ad47d811e91a29309b873ab7a045f45" },
                { "ru", "9d46cbcfc9371ada8301b5b2165701c007ed2041c8bcc732cad1e291e3289dcd3a85dec01df852eb39fa0a61b0077f0c888bc09731d77f5ed9f321207cb32907" },
                { "sc", "87656660780652baf3cdee3fb6febd5af3c1db3d7d92e2eaa9d45dc00c4b4a36b3b30f7248460a498951c2bfd7edb9d37a0f6eefb641f2e6938c7c06725f8152" },
                { "sco", "668592c086124386e9becc08702a11065ed444d4f0c10ffa850adb59e22bca89b002c3cfc11849a7ae5f5c154823e48e24900817ef35a61d52efcd6c3c67a30e" },
                { "si", "8bafb10f27d654259233c6658af0cee8897b6ffe0b110cf580fc63afebedf62679967a74397f1f693d641e5d03321cfc56a4d74e2016ed5111883b3727f26603" },
                { "sk", "2f69583e98863ad72822d2b11e3ede092d5f33a2cf5bcbc304a9d55e634cde5f7f8716d08ab72a7b22ca74d74b3505bb5c3d4038779f5ac29d088e484df1f717" },
                { "sl", "25e7f4cc8323dea03a87fb5e4adc496f3c06ebbbf4a7a2ab20e37d42f0009904b81d52e3d448d6bd481393c5b422db761bf976e1b2965132bd390ecffae661eb" },
                { "son", "b9bfba53f92979652359d0e70884b8c25304e5f930bf11989e1b1036ada8a289b39f9b17de585688e0bbef2fa679aea37cad931ec04c586adc45deca0f848a0e" },
                { "sq", "7ecb3ec42797a3f4d3c1070cc9a87c38a0386c3274fe63412d8f528edd9ddbfe86376a5c9be4af1b59b38a9fe9b027d88d110f3c655a836ba126c1ec6db77b8c" },
                { "sr", "6fb5b8b45887be7a674b432f35c0d5f8fd616f637eff260772ff9c5f8548eb6a65062ed97b1029ce2398e7151246bbf22a3d66ea8090f608b6acd5088bd78a27" },
                { "sv-SE", "1e182b135776f48c211a65cd8ca424307e25fa22a3e6ffe1cc06093449f66d39194b059373078d329a599df9ac3095e805ad4c3372b82b82f42cf97b647f9656" },
                { "szl", "d830d3793c80c288bd37bda63340d07a192e56d4bc0a85114a14ae513ad74ac9bbc0a792188d7de28a24f6d74f16df16654dca5c448b54548b6cd41aa6a325f7" },
                { "ta", "bc65b8e97ae0eca7d7f165e05bce6f522d436e7b0f75e8a017675a2774c99a50860fdb70bbd9b73cab7718be04f892546bd2cfa71fbc6cae96309f62a5afedd6" },
                { "te", "33488957b943abc3d992b57cce17c1f1aa4068a21ba88363a01a14eb00862c196f6d4424ec2792464276e84f9e9f87ca0b48cfe7723bfa41dcef22fffe2d123f" },
                { "th", "1ebba028ad9586fb40fa751aff5a55a2e939084e6318d54ea9e67447b174163d5414e16a9c980c53582882ccc3f26f72dd0c354b851e13ec115ed9878e58870d" },
                { "tl", "3429a960c1276ed08b8fa0680c5ed68e71d98edb4e0ee1d13890db036fc5f9c5c1f323aeca682d3826f384c8ffa3c7e92432baa98b2fa57d3f84b1d8e400eead" },
                { "tr", "c0a748ac679d644e3c45929a67478f3be228ecd57982716551960943149941b146312774df68491317e30e82b935426b530c27874f7d3f7588fed4066b9bf913" },
                { "trs", "5393a8eed56b171ebdeae887a9b47632ae7eaaf8843b843c8c4c8356ad01ce68ec6852866990a01a14782d64398508713292da498461e6b5b28234e3d9967787" },
                { "uk", "aff87eea99ded486f8c6ad051a1023fefe6508e5c8a4cc82591a1b87d5f98b103472df1a5268b9c3eeb0d48b3b8394250755b790c6c18934bd3e8871855ec7d5" },
                { "ur", "88223405e70ba1e34010d37c2aa89e76d8cb8a1b284d1ddaa6381ce94b4812cf849c754a4b7a3200a739859682d591d352fca8f13fa375710e4d990bdb4b0058" },
                { "uz", "a3a39e2ee4c36954583a6ff54574350cac412e5ebbc145e18b08faeae63ecf7e52f13b03ccce3ad90de7df9a6c640a2c9f0e930f6f26a2489dc1feed50db8bc4" },
                { "vi", "2ed47adcb3854a6567e2915f083cf4ac2ab46a5bdae69bf08561cdb0738b6ce912e9e4999b5c0896b6b7e9b78721bf340ea4afab06fef2fb46d491f8720fbb39" },
                { "xh", "73dbec096df38583bab24858cf98a8bd2b1a63e6fee5fb21073e789ef019f53517008c5ec216e901e9a9512018192b119c95ca1d3b8b9720f34b71535ad143f2" },
                { "zh-CN", "4af10492bb7b43f05ff887fa6ab466d6efe3a8319c3efe36b145e892124761509b5c24145438fd98433a72f8d7b4d17b25413b8cacca2da63253efc488ad959b" },
                { "zh-TW", "c6b92332e6cc41ed57fcbb874c47bec9d6fa90708373e87ae773f56977ab03300ddd3b5f6bfc2b35b6eeb838ebe34df6e2ff9a49c2b2d2932cb03a50bfa8b4bf" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/111.0/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "69fd5997a46084922e911c8453410d85520af41652302fa83dc4de874d1869aa3df942087c32090607980af81325eb08a453be035a1894e389b38adbec97e472" },
                { "af", "ce594478433951c91b75b43a8769279ff18182e9073a51af1a181abe8a4f56ac786caf9d35f4a9217b50d7696ad46815394d76b584d228b68f06a395d8af892c" },
                { "an", "83356d1801ca9ec92cd4c22207352543687cbb4616acf5fb968b6484db41210b0290a11f30cc87016f8f6019fdc6b98631c7bcc37fe3564235632810e2bb133a" },
                { "ar", "41427362a5de4fb946405dae428dbafb421a6326944853d09927f55b5793ae1d87af2f52fe958b55d676d9ed003f16e8e1fa6c9424b4372138a951ed961bae25" },
                { "ast", "242c1c4b41ac9310a3db7abb3f7d6054f5f74e18a30fb915655b92ff662610fbb24edf22c1eced4574e27bdc9023907c119c595d30e8b9494ffcddb9f90be8e0" },
                { "az", "f7b9ee0faf23dbdd374b8859f305e743be0b2415f1443bd0f6df82d40ebc33e89a71f27532b9d0a1a2633070eeec6143f465831f965a6662df68f3ee67cf1f5d" },
                { "be", "cea721653aa0b31de882ed25fad7fc26a0a29219e265ceaf4f015634535d3204fb1189e6ba76be0726ef922db1048e2a540a631508599f360c53d581d672655b" },
                { "bg", "bc4bf60aba56b2bf4494e37a74d92a9b821a8e6cfa8ee687cb14f79015900f7576f242a25fbc61266c1d40f6ff376544fcae62d9d1bd1d0c7fda2cfbf732cb67" },
                { "bn", "559a62ec2cddef5c704ccb185cadcfd799fc424682e15550ad416c55d85b3e4b9ac1fb7e80a277443ac8be426ba517d8f16f08f7a52bcbe30352a02f63bb0250" },
                { "br", "86ac7f48962910042b38306c9c0501051c2d812a6ee6602220f6f0518ac718ea07df3b4893cf27498697546b96447c4f0a4bd29603834d053b95c3ec34f72a4d" },
                { "bs", "f4d212e9dfe8292cf74aeb0da38da0dc4cf7eb994de2d9a9ffc2f591d148628a9675bc52ea641b90da63b66fc6950301e837c78dedb743e32b1ab127c9993759" },
                { "ca", "3177eefeefb65ff5f3d7cd8330cfe06deee25e523e3378204ce1f049526df1c7e5ae7c3732f78932dcdd136be19aec0a78e2057ef0afa5313313cebd033ec71b" },
                { "cak", "db1c655769eb81fbcdb7a398973b89eaedf451aa5e5249ff1537a6b25687d511f371bed0d70f3c83218e12d07ba2abc8ca677977bfdfb3048692c1e0afead1ad" },
                { "cs", "7fdd7692c430ff9dc2922bd330bbc1697f12d135b3a8ffd06f997a0d4abb1c470eb455f380bd9df5d5c2224c72a305f713974ddae51316e26bbd8c03cf31dbc6" },
                { "cy", "dea059b370523a93f218c9bcc16e637075d2c05eef4af1090eb77fe552c30d1a3bae24657747e93631befa73f417c773cf822890b8e63fa3d2bc0d1785e69326" },
                { "da", "16df044267eb78757cea95f24191aaea4fa5e429074ec830834b7d3ee5355084a540dbc5a12f078503959539fb71e02501333341b9868bdd50de931665c27634" },
                { "de", "5b5c4ee9cd776f34b6d400b0861d1938a45a8fe1cab0a66706f56a936aed42e4edc13cfe27081447ab223d721957dfb5b1ef43d98abef2af6e88b6281be02d0f" },
                { "dsb", "e21edb7ffc5ae5ca2adea135243c9f1090f1d778ed72246f53eb0f5a1b51d294878f302527f87ab722a15ced2029014225cb72dbe273fcab97fa2915b98403fb" },
                { "el", "8a91cdc2775fc57d86803ef1c8918559c205c064aa8aec24ec6dc418581f89ab1a2669ec1b0be7e032c3ba74359ffd74d6f489f377de1a2a8250f0de8f7c95c3" },
                { "en-CA", "55438bb9f79c2348d43bc2992c5f9058cd25b9ae1386d8a75eed5abff62a771732ac8c52bf1e3e32fcdd1a0fe2be26e1310b2d41d75de69a59f1af2517168739" },
                { "en-GB", "04cb897e935119157daefc1c21b34cb7dbd2506c658bbd21e931e301c58be7218e41ec86885b9633f9e415763924ac0fe7f9ea476e47a15e41f6e4b389c76911" },
                { "en-US", "5f88e0def6613bb3ded8bbe5c5419637c5fcd7340b18120da45e12870ac50dad0ac3fe2ddb2523980cd65375bd74dd2929058bf3c277c7891920b717ba389804" },
                { "eo", "7ca8548e4f27c96ce7d9fcdee83952b259774835adb1b487b8d32c32a99903c1d1c7d1f6f24960fdb0b7b81495ec27c0f51e702e002b5d6ff6b377cf6983d291" },
                { "es-AR", "96e321f2fc466876025edd4cab181ced952f0ac351e52e7ba3b888f7b44bc3abd3c348cc9c3d89d59eebab5eb228bfb9da07c936929914408d39ac75b0e58b09" },
                { "es-CL", "288f280c032197c983c10502f3e85f6f653b02da4fa6569d624bbf24d3e8e40596149722493b1ebdb313df226b904fcb565e19757f3673b482387f483325029e" },
                { "es-ES", "3210ebbf4a12542712b85eaf52c7f7708e216781b8a67d4fd15341093761a00bd838b8abfbd5bc051cacdc5eacd11f8c85d165eca7e1f9e9e5736c4d897e7005" },
                { "es-MX", "b8ba1d521d9520bb0004fd2561302ea82ad7c1ed58778b350042d8615b2a576657146f0f244c193cb4b3a85baa4f7a10162a76b92e0225c4d35d86eb4d6ce090" },
                { "et", "c2ff273f86fd09892ad5d1beaaa2b953bbc226d1dfb81bf5c002a9f4553a614ec212ae99609a94a8fe0e6304c5a3b9316a557b2721c632382dd474dc333b586f" },
                { "eu", "14f1308a674db7db550f62c447f441e13648bd35e262a58267df1417f654bdfac84b9483c066ca91dabb09c9c77221e093e12858e77fb9a1be6a7d8da2d7cf7e" },
                { "fa", "ec8c8204f632a968372176983091bd70e6100b9dcbb6345b82083c69e6b200986d9f5d01b5530a0d6fe411f54b8415e2d548baa9280f3e9230b98b68162216c8" },
                { "ff", "8587049ca2491517df1082a6deb65218c4cd11b8d5a3b35f2a18670a91c9a22327f66bd1d85dd6a60be3442c097ca29727cb1d3df2446edcc9bf7fe0e056d03f" },
                { "fi", "2862a706199451442efde41019b99be8b50a789cfa202ea99666903d170be3976df4dacc73650c1a13ef6ed1183811047b96cd77f1d634cf633106a0f7db34b0" },
                { "fr", "3b569aeccc2875a9e527d97d41d8f9a0b1f6897733aa9a22aceb63868a79a851d6a7219f9c33d984a61bb3f2c07eba243bea2c4780bf52c934200f84bdf4e989" },
                { "fur", "fbed202d7782dac38e71bdf2abb00ee6e1300f20bc2bbed8bf2b614efc8fbf20f69daac5d8c4fbcb7fa0e2b94cf244661b469c9a3b00c39768ac219767f9c983" },
                { "fy-NL", "92f1b7b605c7a831a2ccc38ca618454bf814337a8126abd0c08ca3063d2ebb58002b4c31ab0251fbe3d2f5212516834dae69fcc98773b707eb5fe698b6b6af71" },
                { "ga-IE", "6b6f59555f7ab055caec4942e797c01318212c8df21587ff6fc4489bd6b373fcd3a113ea49292906848d59b26c296fd5605ba756ae317b7f189c8f155f071669" },
                { "gd", "86f0774cac66df92073064438c006ec1c0cb86a1c01eec96ffe076b39f2ccc4c0879ea3ca7ad15bd5ad3a43e6bd327dce38724726e79cf92a34a02817da7fc8d" },
                { "gl", "99111b46fdcc5bc33464f9fb32362e8e289627b41dade5b8cebe7f64b593dfcf71c9f7c37f4788a6def2777e6bd428e158d69b09745d89a52293e6af34bd118b" },
                { "gn", "1e8c13af0adcacb84c445ef914e88703ff4b6c10c6e9f38e4517a72b7c31785460eca53a4415aed00529b5e46782f8cc5b8915b508ee9ab6ae30f7cf2df67da4" },
                { "gu-IN", "41202943bcef406503a813cbcb4d53577e3bf5d3a6a7550b3f392db62031629c9919453ba2e015aeae14d33156ff4ff59553bd21208da23f14826f2273a8eb1c" },
                { "he", "303ee56b6433672687a58c4239db458ed055de7ddcca9e46bd4ff9acc2704c00bc0114e72f2520346203c739285ea719644aacb794339a0fa18fa6e22b154141" },
                { "hi-IN", "f66970e934239090b9f7a7adcca0609abef9ad6b27a28cf201d1877b98a6d68aec8d16523b7418e855666d054f0d09db2e12066d2bde63199ad7a107189a5f09" },
                { "hr", "987a3db3a7df7223105dcc2bd2c2dfcc7fcb860aab752638dd282dfcb27bcd60fedcdc9c936f044df8e0d5ef32bfddfcb767b6fa87d929a62005cddc64cd9df2" },
                { "hsb", "53fea4140249e5aca9443ffff5cb794e196dfd59d605149ff305952e2babb9a38658f5e0a387f211a63eb18823b2d9af4a6d4fccde9f1ed823f506958640a6f5" },
                { "hu", "3524edbde1fab38f04701f9a908fc5ef902e2d906d19a291da0ad9c9072a78a1cfe2b08265b1430be04cb2ba60f24143e77a898d983998a9d472e0c185d3d6e8" },
                { "hy-AM", "da0c3d528ade5fecf08a936f35271230c5fd569eede34a58010a8eba0e53ab1f964e3314f400268449c72ab7ec6772e9baf92bfde95662ac61f13f1b841048be" },
                { "ia", "88f7c659b466ff46f16fffe7c8b2bf3d8c0f89c3235222833de27b1ee1c19e85e7ed09c25bf30b0fd98170d727f12e6893b50dcf54c94755f9ea0bd792f1d31d" },
                { "id", "ec25ae6147ebd818522a18057dba86c96c97d525e8c7f6a521578ab7a29c2fd4a9620f520897ef676a3d50dcbcbca02d82316b0ea7437e6af3d3562a39632808" },
                { "is", "2ee02e42831870ae4ba416752a58eedd70527eb3d26c227c9bc7a27fa697355de99f3c5f0545fb4228e3162a4a8c2073d4a00bcbce6e6305bfcb58809613b27a" },
                { "it", "85556a6242176b13453f8d4404115500be5b517d44a3eae59924dd2f19d4d01354847e04b69e300904df84bc5a84f1f6b0defff84f99f06b4e4a996c71abbeb1" },
                { "ja", "112fc88f526a562955c0ec5bba672488fa846dce5b5fea7c63f9080cbf4a5ce372f79b412b5173571796bcb77afdb2e523460b371f62b151129bb49fa517750d" },
                { "ka", "e5159bc89cd3fb12a8873584ff348e4f0d2471e35f4dfe7cad7d5c5f4c4375c9e0506bdb751751d660a4170ecccade62cb2e0ffd474afa7445b3c49a55865f14" },
                { "kab", "3d1b6a7e827df035c12ec34cd7407b4142010a52b530b9c6c5a423f0d5c269b78ca5319909b928069639b03982288f6ad1f79c597b1d6f059067de3d37cced95" },
                { "kk", "40a191a3e8327bcd931cde6e2a0704c94f645cb0aec3f1422b898e97f6788f762e248115075248e5744e9394cf5014d42e2d157c6b3269415ab3a71c3a83d2de" },
                { "km", "9173322c088b72764abdc4bd9d8f9a51d0c5085a66b9cfab45fc4d9f62ee6f5edcf2e474875fec3702f12bc3e50d5af4a29aed4a510b583105f229c51c4f0908" },
                { "kn", "6e38db24324c75903d8acc74586ea0dcb1a50e4f56f3b6e669d7efd1b2e211eba16a5f834c1df435b5834d0320b25d5af58f57a3cb03ecd4c20aeda1a69cb651" },
                { "ko", "22c61fde351ee5c5582a164fe45fabdb00bd3010fe67f394955b37608b016ccf3cc8b3b0cba795e248025dc8b2019c34dfb4dadc75702100529064c689455ddf" },
                { "lij", "2010cdc047a2e3485b5f7e9fa6aabe405b7b924d5e71b11c6e187039736998513d520cb0604d99b4119d708cabfaf51a87409322c2137a27ae98bdff77372bd7" },
                { "lt", "00079799b02ed897f7942993ced43e4bd008ef75718c82bfe27c82e2b27add3d186f2031f7c4708b6b8b3ef79e12f1afdc5408f1d08148afa2871a2821afc36a" },
                { "lv", "3846d3114d0ae975bb884a7836e016171c7dd38997e2d06abb515ffd19864d5c2a3f07e129c328a172e26374ad7d7a1bbe60d0bae4f26bbb016539354faf1af9" },
                { "mk", "404dd764542428aa7b599cab68e1fe91340b68a321c34f393acd1c738972f16a5f0c21214c00133977f617f26422796437b4ec4f6ea32375fb36da0f360d11cf" },
                { "mr", "8b596422c84c733ff94709e2dc63732f8a49b04baedbbb49ff5f793d53ee9c12f597e10cae3a9253613d114ee5196f77eb4871cc3140376ce263ba9ff87a2fec" },
                { "ms", "7cd181fa1d2c2a493e6007f9452d0c4f0f512c3a7673f19e537480c294ea1b98db0a0ab9910af253168e576ff2f708a3c813451838de22615b33fd03eb796bf2" },
                { "my", "3bda61a1ab90786632e0e7ae7655a7c8057efc5e154f3e0d1e4428a2c9bfdbcdb04b3bea59eec94eb30d0967d15b938f815062296d8d3d75e0863f63bf561371" },
                { "nb-NO", "1ca98c598c17ecbe757eb1df1f05009933d077c15254419d92aee7e83c1bfd8a85bee2dbb6d64be9418fee54af6acd9b04c36d048637ff14ce655a33bd5c294a" },
                { "ne-NP", "103e97db82563a28d922544aabdf26c0c5b97b7ce363d8e492c866ccb6a6e210db1b3971ba1462155b76a5b273cac0b753abdcea39e90550fc22302378f3b3b6" },
                { "nl", "74f4b8bc6598607b090cc8899b986e1ccd60b6e56c9de1496dafc5a7df40161b055083c32d386c0b12a0428581a55e33f6d92effe2321411fec2a255137f081f" },
                { "nn-NO", "220844ebeb4a5e22779065ac3cd0437b71e987681cf04c07916ac79d623d2e880908456e4061fec501e1a88003ab5ab4749e715f3033da1486af70e4b2211f0d" },
                { "oc", "ba4c51c10bfb41ab11a690c824e0066ab5b06fc18cdbc0fb3330a7f5146c051f389a83a0cf991d91d73282376a6e07ab150f7800a59d14a5de1397fa4c680682" },
                { "pa-IN", "e7e47f3aebb7de1a20f30e9c974cc907fda9b351a3c9aa503b3d00e2bf10d8e614b21fd6ce377e8e6c58d6891bcdd1d75e45a7b9ea862dd9bc3f5eb8ac2cf02d" },
                { "pl", "72ccf31d168d81d6bfc1e0b7358fdeca455a633767e8ecc298e2eaad7f46bb72917b87838fabd58afc71acaa63b8d8b6c9715c0df922d529c5d39291e1c7c26b" },
                { "pt-BR", "5f71355441c6812519c5d0250d33505217399f5c8f590beb632d64a9055fffbc8c58effa7875efc5c9c5967b59027957fda46d569a854a659c274fc2de80d0af" },
                { "pt-PT", "c9bcb1f1db49286b6a63d9c829967f246852894d2c3d4fc3442b700aa52dcef9d1c9d16b83a77bca42091a1b014a2dc099fa258d4131924c89e173187d34e3b3" },
                { "rm", "5d9b6ac283d13e390efb8cf0885d94a9ee26812e0d4bbf8111aac175c5ccace4947f37d32f8b2462b8e50d9c648815c3c335981173a5e9c1c57b2b4af87978ec" },
                { "ro", "a5933a159d1361a0cb0efab5a220dee6b4565cb104e11735ef47db89a4f74e6e077062b602e0923d1bedc9eb6996a93067127b788359566fcefff591ca07bd95" },
                { "ru", "b77ad875090d916d4b17a580915171fa42475c7f42ac9191f39aced2d456d716cd1b795a3d55c8c7b4518614b4f1b1f415f7d4959163caf1c9b6764d673bf077" },
                { "sc", "e8adf2b4f829e7b3ac24d6a25d411529ef76ae31c3e55ff974dcb6b4cea212ba3e32afbc13d84b0c78301166bbc1997c9b0c22d95afae02f1c79f9d7a56af728" },
                { "sco", "6bacc5740c0efb276debc4fa2201fb7ba80e03d823c27f10add9eaee0a350682bedf76f1d5dcb8b27e937cc8bed94dd566959dcacc47d2f2fff46f8915a1ba94" },
                { "si", "43d4c440cf4a8397a1473da73fe4c14c105c599d821558679aa98ebdab470441fe1a0e3752c264f2dd34b1dd04794ac5259135012b0543012719bc9f25869685" },
                { "sk", "36d3ff4f48e3f19a13bcd204372a2641089a496a0c0821e1ecab8a47824eeaacc778fad45cd48d63a19afb586f316fa73ad15402becf34caa52ed65827aad4d6" },
                { "sl", "c1a1070e8e849cace5d23d81e1deab43e4d5a322416de1a48dad3fcd271c1defad654522fe747c5c2e49bf8b2c09b70afa6545caf1f19a920caa516f075e0f61" },
                { "son", "582aa4a2180af36a051622f66abe79c64f4398f5afaeb45f1eff42b5513d0ed159ce1d948c39415ab9ebb4357e5bb5d6c8de8d5596f938def50a52252ef6dfdf" },
                { "sq", "9f68fc11548b1bd458b7d17866b37a38c612da5367ede414f1d56f6ebb973c1ba8a7b2de7a9b6ed82a2317a7f1d4ee7a81e3ac277fd7f3f5368e136e671dbcd7" },
                { "sr", "482d51f87e482ca02ebf64776802d96f7dc82c8670e72eaa45e142242702bd74c17e245759b2fbba588820f981219fd0ad8f38142dc547ec8f38665f219fe184" },
                { "sv-SE", "2b8ab2b4c93d461f36a992b122a7e5ae90b48ab0da1b26f869f2ff7f08c683a0d91ebdf795342831a6bf9f0d15792bc5817e65523bdc3c9ff1140178f9907ba7" },
                { "szl", "41114b1515ac4c2fbd9d3d8bcfcb430dbfb58be3bd4cf2e60fdff5187124a8fef04e3716ae53e08ee096819db035e6353d3b5b1bb8355b2012ac685e852c587f" },
                { "ta", "e723afc169eab02970d4519ecdbcda247f49e19aa66e5eb614ecdbdbc9eeb44e2a457c5aed74ed24bb2931997d7274cf6731fd8375d40ae89e32968ab7c6d19f" },
                { "te", "4fdc967728c7557c7c5a4480f9b6e07575ddee4350b1e079fea172183ee988dfdd40a9d2ca569791b86fd90665354b95821e0ae9cd058730e91f4ec4a16ff521" },
                { "th", "a07561bfb058ccd9f7b96d68a4be9dd55d93e8c023e0a7c4164dfef030db9d8af373c2aa1e05503b8f164bdff65f78f04068be3a1d80b4ec509d828e73ce8f22" },
                { "tl", "799b43e48c7d85a59bff7cfcd1fd7ee904b6b733134e666ac981174a2f3bb2176d6ff444628e21b77ca0a59c3a804feaf5fa883ca345e7f2d22ec063b44e92ca" },
                { "tr", "dc41cfb13ac84c6579adf8491be238294c29140f2b8de6073612b83a6c466605e5dde3bb4a8a9f7ebe4394b13fdba68dd595f32fa451b9bbb59a65f945f49a66" },
                { "trs", "890c265f8bd9bdbd29d3b4ef230237ded96736bd51a0a48000c326629f50760d9c8fb25af154f40b22d5d374b49b9624aaf41075f99d17bc2cf2ca882c1cf0ed" },
                { "uk", "9f5157dba1875910bac509de4ab57202c685a5524166fe77d227fb5283305a8effffba0ad9f685bb066824de3877845304dc9890a611bcfa446a0615f2887efa" },
                { "ur", "b610d07011064fcaa4210a9868d3dd3a53c93022ac0e7c72f9fd890d9c8e0d6fbea0747312b381a6e66b361b6ccd67c30997a44487f96124ca4e5c0c1426e1ea" },
                { "uz", "9ab9b634bd471dbd22e338033484352dfe9b3bab7a568a5a9bd17f1a6cde7b8e5ac895c62b9b761db56271cbba2ff340dfbae9697119d2a064bc4cedb5e6038c" },
                { "vi", "7c550838f3c3d6928d4104b0fd0702f93a227e5ec9b0d4dab34c906b501292c416abe0c55ace5c37f3a931c8983860026ac8932390cd3202bdcf2fcfbf902d74" },
                { "xh", "a414b7174335ba78dbf591e70ad9a63e56127d0d02e78a972c41b0950d246e8d3fb5ac13db9eec10bceef60b467bd1e7d182d5ad9a93663aaafb6a5b179a4e94" },
                { "zh-CN", "5da4db79fd9313dcf2fc9f138471e49d4fdedd8f43b74d77ec3d4a148061d3d5ad6ef924741351815a9704ae3c1df9afc9d57a85c10b5343eb2fbe76af83cdd7" },
                { "zh-TW", "e91a85c0070a5e71905f2923a1517fb5d4d0f585dbf170abe8d50149ddedcd81fa61e97eba992d2ffa6e182980682cc11d77f304540a3a8a2f2d89c0cce28f6a" }
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
            const string knownVersion = "111.0";
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
