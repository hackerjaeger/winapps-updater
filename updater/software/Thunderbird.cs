﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.7.1";


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.7.1esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "83ea8267c57cb6de47abe110341caf2d82cdc5148ceb61dd4226a3ec29f7e03d662806252ef7be0760b3df6716fe25becb67528c7ff49da91143e296d54d0689" },
                { "ar", "1168d9cb6b7ef5698e67677a6c03c0fb4a03155afe5677e50e86f9f57b74898d965a43177158ccc481fb626f9fe7d75294175702d706296156e5695d6632f3fe" },
                { "ast", "26323f1db4da3cecfe47a7b22ee353e8df64810960bd0165141c783ef3eef029a7eeb5f754d8ab54f73efb5f97033af20ac2e6d2483b976d92330bf0cfc709ff" },
                { "be", "bfcec7fb596af3ab516481afa55f2773a7a54dd83e4a7bb6c0de703e815ccadfde4fd8e78d11cea3172515828c86e0d9b97c4dd0d8cf8bdfe433c9cdfeb2e213" },
                { "bg", "8dc2e3027572e015cb21bd31e97b0e7d2d81fd74f9b5d58640fd1eec4ce53ee4170a13f25a0963147cf0b3b59ef99e8a9c1e40d1da429c397bc4d30275ab32f5" },
                { "br", "e4b7f66a15286e1b8fe5686bca6c944deeeb5a24585aa41ce6fc933f91aac2308e3979ea2e364f9ca2b325040483d9364545cb4575867b546f34d5659df96901" },
                { "ca", "0bc02bab11668e8bdcdaf9a13807b4a914982eeeb95348fe74274be9db9f53874aa2cf74a8d115ec63639f989cc2d4dafa164d475bdfb8001ba1a8fb1dd0d12a" },
                { "cak", "64dd8d645066b40d7fea4d40c8cfac29a5bb6d018e823e55847979eb3713a2004340a6652871398887f76bdfc1ca474c8f67b2a73f5afd4462bf6eced0013662" },
                { "cs", "c61f2af0c0223d7996b2292a1e6c5149e919aa7518d608576b0fbd2014441fd04586ba6f3a24dcf5224eae74dcd5ff4b1776525e23d0ea0f9136727b1cc7167d" },
                { "cy", "cf0dd2da60c9cd216f468e279213ada5e0a58546039cb01ea9d3507a7a57b6bdd2215382f4a6559f3abb38c68d573dffbf8f5aaa22b83bda0682fd389a7b1ace" },
                { "da", "f845b7f489914b4b87bef3cc613724fa34a2b5db0598c1866bedb99055fde26c45b5d45eb1353cd82e1978cbe72913cc4b5010310fe0002dfa3388fee90f5005" },
                { "de", "683eb961333c9b4e98bca0f2c09dd61d1ec0f6b2b3d3637f032aa7c76a2a35fac05681b38ac64e631aa2632281cd41ad4a33e18047a20058ceb20a0187102ab1" },
                { "dsb", "c926c2b88ccf16a9e5d4796a75b15d96fc13d185c08e0215144b5ce78acef0329a701d47ec9d4a4f4caa98ce552e02d957431f68b3a9eef69627cd8a7898c288" },
                { "el", "33e444da3a55bc58440fb7e65a2307825427f3a0f47c67246f0ca4db831b85a94520556b074273aa60dbfcac4a627a4525e2f0404bc599e90eaf1fa22931429b" },
                { "en-CA", "d8e521c2b9b5c4c37b0f969b3afdc253c667d079521d33eb2bf155d6b35be271d49767163bf83d0668ad700d455f61b6dbbab5124535431b168fd49c3255a635" },
                { "en-GB", "ee6e178c2a000b6fa04228b33f6bcc0bc09a64deeacef3c85e21ac76cd184700c0a7450adb47dc711d46dad821a38bd9c05069534c7a5f57614ba350e0de4479" },
                { "en-US", "98157dd893ea3d70e0cc30bf0745723f7bb36be4bff6d76cf1f22fdd38d383c15bda87f3197f0dce5b868d0304c99f200ea850c1f459c90bba4116c982d619a2" },
                { "es-AR", "60829863c1de4bf5098d097a868299c97d23a8d71bae0894041f92762910925f7aca6808780e96b50dc79fa4cfac93204718d201c536028d2e11ee0b594ce9bf" },
                { "es-ES", "84979e3f12aee01a5afbcda7a9e45fe04db1949ab694b3cf0584d82c484bb38d9c25aa8d96c006b7430ab0e99068c0fe650d67e64910dde96bf499bc3dc6ae42" },
                { "es-MX", "93a09be5872780682361b715476b2b9fb56215c3f23b1dc8f9dc5b7c6bb3437dfba2eddd83708a446ec791fffd43a5ec0a51213efd9b31be51cc2b47d9bf9d66" },
                { "et", "8d851b512d9eaa4b97125e0f3249b4ebac4eec9264dc3b7d859ee43962ca66a7df8466d1a527943d93896ffbf2c14af94c3a4af4fa6df83fc8f1cd41078b84e3" },
                { "eu", "18ddab37e7b75a8dadcff5d9a400963940bd227f76c63274d195ccd4febabf3e406f6da6aef216d8acaff4f43a21b8217fb50b50991b08b92456504efccad0e0" },
                { "fi", "b377a303e04a4971e606c3642602d65b94cd3983ca0f65c9a30d68a7ad1a26d985afe5c9c474286cef0d25b59028bbd1adca6bf17a964d817a6409fa950eec46" },
                { "fr", "7f5c42902e9e3d16945d830ab3bee26753047a29cc86b670b5fef1acc7af0738db36d82544f4188dc343daad212aab642e5ad1d881b8a3997a754bbe1fae925c" },
                { "fy-NL", "15333a90e330187185c6067c7ff8fbe71aa563f28f3516e3b5db911c3469fd62c137bb76724db6ad66e2ef13ce47dc2b2823f43d4e2681abb088cf9bc58fae85" },
                { "ga-IE", "6bb3081a4a42e3574d54814e56fd48fc6477b55f00bb34e9536b13585621bb937a80749d3957824a96a880e58ade1d1b6118807f45cfef3bc7476bac3d8623d5" },
                { "gd", "a33011befbf3c98eb73a08b2c7ed67f36d3143cb47f734eed7db8c2ed0bff1952443fbceeb3ae28f0d34eff7bd37c96a5276052054c7218af0eab789451970a5" },
                { "gl", "93f23e9af73b99c335df795e13414a4e88c977a9865aebda4ee9b0c81546a15efdae4dc29c5d9a5248f89eade8bbc62afd17b4d323cad5a46bdc17a95ee26abf" },
                { "he", "ffaaa11e9462c43b68cd8de6f3abd294dfef5324f6e43f24c052fc0fa1b75121bf2922accf4c3afb599d75814304c20aabb9e2385433f03279e76be990ba0ff5" },
                { "hr", "69f55a8130bccae12a8e6821e67076945c2e9d590ee4bbf33e6f5174121202f87b42b7d2a1e2f0b289b2a8364ee366449313fa9ccc6f3d6c4b3e55c37f4131e5" },
                { "hsb", "2a2f8b452c352e29a68b2ff96e24e4d479c61fd8c3d29ab9f942b464ffc88089f31443369c7fa51f638637eaac5f8031a28c77bb0d8c9491f7008367b1c5f11b" },
                { "hu", "22746c6a3eb8695de75a88227e7bda41423f7b8f02b2483d61030618860cbacc1c09027cf6c239650d84da87b47a6b8071209811d5af204b34b74f2881910a37" },
                { "hy-AM", "85fd55e97fd02780f12a09634e2a019c97be6baf838e7fdb5aa825d500d8160d1b73d0d9fc076c84353ac7aaf6f11ab613e402b882fe4e49ef8796321e8db85c" },
                { "id", "c71926c07e1e66fb89dc555c8682fca6a954f0ce1d1ce07401a9283af045fb9e2468ce08070b957cbd7ed187e32301a3106496350db24a904f38ab3bb5c463f5" },
                { "is", "5138e44bd98efaf9948e10cae72be80dc49ff22a97abad116e82b32846b8223b076318dceea79f6de44e7cd9fbe8dac7e6ebfdba2eb9ef43c3d9503bca307fc8" },
                { "it", "2201e8636e4d8324df2719198037536101561816882e5c63f134743eaf74463f1a2094a3743adfd2455af45df65aac549987fdd0ffdeef1a3054653bda7ddc9e" },
                { "ja", "f06d76485a004c1b996672e41e8a3b9785f125fdc45d4794bba322681de380a8a5788b8d04678c5e93a6ccb3cf5a2799c94cb2e565dd0dd9256dea4e7fd6fd28" },
                { "ka", "4e14deee97d7712db53a2db1211b703a3a8b4866709fbad8666a1406658b15dadb5bded2ecc71d0210f67a70b2ea91ab394ab7ec61f8f5c2306ef7232a9b9ba8" },
                { "kab", "bb5e1b34d05b8a9aa5cbef541ca4d49d9cd8fd3ad4f0a146711abfc8f335383d046796471789f789fd6c554a724a98b911dddf7b694c7d5ad20d94286b192f53" },
                { "kk", "fc34c5953eaecd9fc867c5249d79bbacfaf9d0be8fe58dc2c013976346023869b6463b1cf8f1e7b53ea1f7de5762fcbd962e43e0c861bcd8d8e714b3f4df2104" },
                { "ko", "d24018374011032ab0a9aa65bd53581885fce640719b324fea59b8d07985243f0a34d5e09f347658fc68ce856f812b2333806022714552590570ce50f5233a0a" },
                { "lt", "2c504f350821ce042035eef19ffdd54df5ae1c430e47a7500477fbd19f01ced301be090835e32a9d037b75d9bfa09dfd2a31529398fa8542b48a758f5320dbb0" },
                { "lv", "60f6bd05d33f352e32f520acefe571e9f100400bc47f1b41761e44b70b3f3eca621b69c440ee3598607e90fcfe165ca954f798fa6d01c13e3cc9f1322d47d9be" },
                { "ms", "ddc47d58c12abab95dc32572c85a9ce8710c0ec17b01dc53306ceae60326e18944713d1ba8796959f69d90814ec7b1573c9b04e6116a2f87559d36fa4ddda589" },
                { "nb-NO", "b569b6435418ce002c690a5cbd3ff9b700af730b01197fc59221ad7f9daeb236cbfdd243273169759819f251180d56cf3185724d1587727e8059b6cfeb1ccbd9" },
                { "nl", "3981d3566bf84251c62d7e58366c937d8623a52f0831ee3525a0ceb685bd5dbc6985e0fc4774d4b322bf64538a635c2c047b9a8d90928180b59f414bb1fbbcd7" },
                { "nn-NO", "65ebf8a312c1fa3b1709680242fe862d6b98c39612836f1eca07a6384a5ee73c9c1a1d8cdf4c89f7afeb3015ffdca295a2b0e23c774393bf2025aea305000b73" },
                { "pa-IN", "e5ca4509a6ca4e99cb39606900f853002e61fb59ff36df38dc9f2208432840877e5e6ed3b612836a33193d7aef41443aad70c81e028d0f725347e715ca95e590" },
                { "pl", "c77f4d4b938a0bb1cad30802e9e2508a48fdd427b1bed3ce254834beb54a7e88e156375e96b9317fc514b3f98c5b4be9690aec1d7d0fc45acfdb02afe4add942" },
                { "pt-BR", "9a2463063b0114fa57ef245f27af1fb667140a8e70205f0027519d0c15cd2c2df74991ed8870723ab041c8542345d2db62c9da7135c009ef26bbc38790369379" },
                { "pt-PT", "de04042bcb94ec7f3cf7620c01a3928995a3f9692958cded7c38c62c2a9e68c0274d25033cf5101d28d3a6dbb5bbb9067d97052d70841885073e0a583dd4dce1" },
                { "rm", "eb5dbc44e51bd9a00ea6d54709bf899b09b8560a1089ba88d72e818951f59b723fc50d3edb8b36b3ee802811a73dc3fed6d065c667fc2457fb5570183d97f38e" },
                { "ro", "39fc78c462699ba5b3345d0a1f5a93853a403fa111816d3a376c50b78c25f06a1553fbe721126b0efaf2d6c65f4cd77bdc2cb928c117f23cfd5caa6d110ce64a" },
                { "ru", "a83df627b97cbcefedbe6b3eae149446def31af6c7f54f8c8744a713e0312ec8adb8944aae31a82c0ef639c5911a5e3e21fe0c3fb2e31c71ba135b5af635f43f" },
                { "sk", "6726ad90eb9f73d4ec1748bc32a0619e2f8420f29e8d7824578547240f15f887500616d905a6e61475ff1c7f2f821832f18293732531907afa1998613b54c4c1" },
                { "sl", "ebee8930269c5eea8bb94d1a8ca60b5e0e9de4f6766d158048e0b172aa54c42c3e90cd40fcb52278c130b6deb63c3789056e4240168a333c2ef43c9732367b88" },
                { "sq", "f38ad70d8fbf842aa1a96588b65d782ce50a930d70d935793b00a779a7a62b3f61715731459352d3d025b9bdbfc9a286b7eaa3900024b8ba3b7337bf6be28134" },
                { "sr", "a99b13681136a562f5909cb3a9188a3632cd79811f11862ca8028acd05edff9457342586c21113c851b90602116200c0664113f79775d9dbae33c0dc744d5e09" },
                { "sv-SE", "b4870ccbc27cb84156ac11cff72cf4a4703ae2950b180bad5a22bb2d9cd12a618c48e511e6f148f2c5bf6bf9cfde674fe5783a4da0755cc17bd2c68511d50281" },
                { "th", "be1569852a242e95d92bec68f021fd00dda87484ac5ddc9ed864dcb392e5d965661b5bdd53c096e0aa473c999651fe1bb0de25380b7339588027955537164fbf" },
                { "tr", "674ff9b2aaa40f282ba386a5a58c7c49315d5b09690c7f8950241b75c35de32a70f42efa8b50a301db9972d0a20eaf6c2dcf3f31edd1d9c204f502b83332e165" },
                { "uk", "56dfb3cdb8aca5065a1157756ba3489b4fa17ddf9ea71f124ca0241e301f237f537baf83c6d55b69f1b1507a9941c442f04702b6173e5380f516c28a30691c57" },
                { "uz", "e1e7e052906d3859f9ed00d567e8d815632f614c556edf302f0ab9ba3d38bac23ecd8f5ba559fcd11c4bccf82fcbc723bea035fcd0a9eb425803f5cc496027fe" },
                { "vi", "cdc6f5111a01f25580f30316cbebb849cc5bf93f770f212f6f47059a2f306c40164c4d4529781fe9e32fbbf198ad19ca13e2c7bd557f6dec71c8479c7b1f8ab2" },
                { "zh-CN", "79829f43bcfbc36e12de1243e437e0774a2292068696b265e18217f950303d2085e9a614ce18dea85f44ff3a1acee807e7851ef9f54093c4511e99a1f1468a3c" },
                { "zh-TW", "5cec47b80154b6130be8f2f4fcae9a7d3c54796ca02d6a69bb2d9fd7109abe42d290025727372774169b327dcfa8fbbedb96b49da43179cf984fc4f53d3421b6" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.7.1esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8f1d8d114411e81192a82591a5b664c86fb1cee2efe57e39592b471cea328a660045c96c581797251b3a774f3a31788a473501bf55abe97b690609c362d318ea" },
                { "ar", "1bedabf1fac90b35d516eca0e986836b3b351cb93f4c699bb0c519d2cc019972a2e54538811967df6068fdc26a138d53235bfc0f835b174d6e48060c89823630" },
                { "ast", "d262808c6e4100be8a9366d855e5308379332a093da3f55dce929ebc1eab6e8a9563f20c1fcecc11cce40a248609ba5c2c12bdb4d0cb5c3ff0bcc658a00ed871" },
                { "be", "38557c30b9f9beae6f9a4aac060e1c28f68412f41a965b549e6a7f1f4a885df755fb9b035e1b0b91c434afe704a18cec8de59de95576d1f2df598d90de0aceb7" },
                { "bg", "99975825e5a85a63c98c1ba53bc2bc91286d1dfc1ea3c2e4f3cf55117dab8eb60da7fa7607929491825187e03ee3d84bde145e8751cdb695c415f848633935e4" },
                { "br", "c17bcc3024fda9e0741a5e7b10ff19562e64a104d7aa8f99eb8e2fa822ba95ac28850cc9a077b63c46a95d90bc73be98de21ab4ae6fb0015f92ac25e95361253" },
                { "ca", "f9397dd1784fe86041c6221c6c3be294fb9bf10dd39ed10ad37d67e00eeeef0b0dc541fc0c77dd14d3afbeef5b47960215036a261d518933be77fe43ab55d605" },
                { "cak", "b956f967cc8d6843f323651deb9bdfef4a33f9a88bdac6e4a14086499de83e4d5e6dae39a31bdf3a0b52e7cab3bf06f7bc87c491c7b26bd3be092dd7a5c4b906" },
                { "cs", "23635380f174e4f1ac829098c7ca63ba56ee3b739ca3f2093034e95644241d4788b2998b8f323d597ec8ae60cd009002fe738e7962c71b09c14b0ff0b704fcd6" },
                { "cy", "e89cd24602c3a740076d930fa71837a8031bce46bc4758a2b37469dbe0e5f346bd0d0c1f54bc1e4a864bfd38493adbef5591908cdea3f31c300ad892feb62e21" },
                { "da", "841f16e2fa7d53ab0c7a5023474f2e9b857b8eb38f9e86b86682ae6b79255d4d02df9063c8fe0eca93d94712065cdd1f61c9047814144d4f46671a3179ffe090" },
                { "de", "bf58eb0d0bf6ff2d5857630b127523830b6175915c8e29edf78e19d24dbcda43cf37136d47e1e4cd18d1b15ac118243a058da3c57de78fc2e475dce32ba6f0fc" },
                { "dsb", "c328f518d35a4192b3e5c6839754e96080a27f567eb9c9259281673d25206c08225e0a94fe298e0f64b235501bbf5e8d27b99b4e6858cde325034712bb902165" },
                { "el", "7e2b387f5f11e28b2efdf6a0782a641daeeb50d36cfd60eb7c6e4c0c5ca192db27ea30b1a38115154c6bd30f6401c714cba2aa9c1587ad5e7bd712d2c0e87168" },
                { "en-CA", "0f32f15757fd62b81efacea0345b49b9771be0c8a1f93b1198219e528c88c27a76a3cf65baf8edff34a042b350b1f994641c01ec174d374fbb53d19eb34e9399" },
                { "en-GB", "e7bbdb7140e05d3ec03c2e656d9f1f8e1f061d051072326a01c996ae9180c01c28d2be77f7a3375071730863a3f812c4f9453c01fc82860992a13abb35224fe2" },
                { "en-US", "020cbb872c56b5ae6ab90b08257e014934236567ea9e75d2974d1ca0a3c72e395a9f30137f0219ce4b45b00fa288a8d7269a1a012d7651b0b0ececa7a7faee96" },
                { "es-AR", "e2fa29706e3b23ded756dc51706a3b2103353959a2c062167721b6a38d7466f5db4a643d0fdbba20811ff5d4b040e2cdc763dc7c82811eb284a63547eb5192cb" },
                { "es-ES", "79f6c4e69cfe3cb930294da6262472946d1de254ac766622d2e7145380f5c9d4ac831e3a590f740b74efe487cba5a69d1329756b2c0549898c4b455cc550dfac" },
                { "es-MX", "b8c3afa06351c7ca708e12652cfd49803f7d2946432d36f5b7d705b6ea9ca9a68c1f99f8904e79cbc891829fa9232accb0f65e948624db451b4017206a86bdb2" },
                { "et", "61c96608edf734ee3f5d74602bc91f4ca137f072d62a85682c87284bd02121090d1b003c0dc02ba869780823a11a5fd7516e24733799a5b0de70b0c5b15a1371" },
                { "eu", "fa5cf079f888086e43e855471cfc6d2a96474b31d40af5a02c4c8ce4e24c623cc02ee84c95a5a8c314ee9aea467bef012b8bc01534815f9dfeb7cba423204324" },
                { "fi", "d592125866eca467c9a34295ede7064c2d2d119210e7d997130d9090d04a9945d3e7697e5dfd9607b61e80d63a4ea31e60fef8237d40abb5a122b934ab55eb13" },
                { "fr", "85a675132b6c4a938def12e606f4576d514009086f37245f98a49af8fc4b4ab681b66a708ca11842b7c591b5dbb7aa19126518cc42e260c21aa5394ebe7e2f78" },
                { "fy-NL", "8a5d6718c59698e3bfbb4fe7056466d8795450fd5cffa32604ebb217dc7e346cc7ad481fb579e2ff0f72c599fc97c9dcfa6009148078119f6b05cc6c019398e2" },
                { "ga-IE", "f930c9df85310fdf86e089690081bc8851815b414268de5b9963ac73e1172cc8849c7658caa9dff621ffe5bff3c30df7da4fe485dee5c55a6ca2b876879b356b" },
                { "gd", "4710890e9f6cb00ce65280aad4ead9643be8d3c8c284baf12c7755d7b61c7175542b94e3d578bc34ac494b26c39e13111b218ad2cd02fba01fe203e29b15b8c4" },
                { "gl", "8dc759a917ee23c8edf3252f769c472cdfdb1b9c57d156b96b941f94d4c2c45250b54879f1a7ba806d3b487c47c90aa504b20a92ae5ba947295e71b2bfe5c1ff" },
                { "he", "c9368c802f19d5f8278b0bcde3e24e81dce29df5de4d469ddce77e1e29067e67e62f42f9a6cc1abf26494be17754a85c6cc87cffe49185064d1e9baa13f1436f" },
                { "hr", "75e7b10c3bc7564eb16ef41bbfb3c46020ea975466d7e14fa560b9104a3710b91cecc9abf390fe0ff4df763d2744c456320b7ddd4704e299cd8d5e82d939ef33" },
                { "hsb", "3d3d90c49644c3894f8cdf691c87dcabca3e153d08c2635657e578f349001308f0c6f1814ef938f6d26b6f8d46302a989d20cbbf8266950de8ff077442553756" },
                { "hu", "c6f90906009b66492ef8fb1b492e5ff88f4a52ebbfd656ad11cf2ba92eef8e1cc8e71ea1da32766fa4843919fd8b625cea696e855a1f1c80e97879b607b68f4c" },
                { "hy-AM", "c7e1232c9a060b2ece05df1e6bbd33187dca9c0558de4895abf0eb05ebe8555ce9e75f30be4f4ae7b0dd066ad8926502937d5a5c574621516203c7bfb04be596" },
                { "id", "c20f0dfee49ab292c93a9cc6d89389ce4ee6aaec2324f03113a8b16a0b969f2dcc66b9de38527e3bb18aab7c43fedf9fbe66af8852ccb5e2b73fbed39e6e9449" },
                { "is", "06c2946d2ed530bf0d16b4753504de00b754f1671c3f65f5e88fd369ea3b1b7d2c7369baa614f9c3d7ec59e6c1fb792d7e95f4264383d764835355e329dfabc2" },
                { "it", "86391709e900bf089a13dae955ed07d35e895d633194776c1633b954d834c1617fa85993f3a751ca210389af67a49db3a4b4669f774440d770181331cbbc7772" },
                { "ja", "ee94958eed2722bffcacb239cd3e2d54a9e8c9ca0f8c9017db073a97acaf864f837f7df4a4614dfc1060330f7feff930b33d5f27e39d05b5294d95f83c1bb238" },
                { "ka", "13c69a41ebf0266d174b9281f4d36f8567907b7122e30d2c1d96a4ff1bf4bc720fff367ff80b7bad3baa01910bf0995ab69b6b8d80db8bd35e2d9a6f79ecb365" },
                { "kab", "a52232405da611fc59b2b2be068d0c3b3edbc93b923def2289ab70421987cadc1e21a00d60d8a2cc6b2337a8c924e40a856ce593e8a270b5450c6d0d7ccd75fa" },
                { "kk", "6ac28e9473f6c80d9f9122009e1ae3a123f809f40df045af6947d7ccc6b998d5ef9571d81f238ac50b4d6622f45b2e581614d64c46914702805d64976f15633f" },
                { "ko", "5f4c3ead3c547e5f07fa7058c5770dbb4df821e57cd0dabce6ba46cf3a3c95e0ed04e866e82b05e372d2f68e03abdadd66def24a694e895052499bb11f056f16" },
                { "lt", "92f76a34f9efc518a2da00e68da3f0058edef11cec3704e0d75206360a6cd2ddb3bedb0fee348001b91994a2138cbe92da90a9e45c279031ee5e762a8134d9b7" },
                { "lv", "ca8b7451e1458ea5479ba6643ae6eb71fcda26638dbc2fbf743ed996e4b9232f8b1b2595797ee732220561e009603cdaf5f85425dcd494aeab75d34a495380b1" },
                { "ms", "33ff270bd3057e5f97c1cfc625d738f93b81717fc2bbed43fe53296aa0e6c15c704c4ecddee500a323c37eff1994c936c5442360e2f763be2ca21a2fe20f3114" },
                { "nb-NO", "03783a507ad046804e69601e765bd2c605bed7493bd7eeec2dcb48fea150bec473c18a10d301f32784d4aae7b6f6b5edf055ce38ad3a109ec45d30a26c79dc20" },
                { "nl", "6237e188baad4655fb73b0055c15f0320ef38219071bbf81b277dd1d6aaac63fb023e949fbdc309cd3dab43ffc81376289b4addbdd4451c1c8c8e8c6df9da380" },
                { "nn-NO", "4ccbb25afe023244bcbc472f42b8e367ac6f10eb11a3a580474f7a2e552ff5be9e7a6298ebcee49e4dbf1bd5833c7e1b7377c5ab4528f7584794405dcecad954" },
                { "pa-IN", "b3872c1e2a0cab96c0b1e0baae93c11446182b2514b4aaecab5dd0e308a956aa1c3e96ad94ac51b537f37532f494f81bc3d6cef6e0d9905958b60397a671b5fa" },
                { "pl", "cfe6ff315bd32218289ee7d73dd4ef9697422afb2a29bf2a4366065b8416066969dd590d7376232e37763d0e9da3540ce84ecd696ce3969f77a15373479cd4ae" },
                { "pt-BR", "3830e786279faa6a8d375d7b772f99000a1fefe24aa62eae47f23c188f8907e7676cd828d5f3889589918a968d5d25abae7184dee803ab294c33169a8c0093b3" },
                { "pt-PT", "85fb15e6005fdf24a9af973d4a4e9fa8febbf1a6d98bdae21ffb1cfbdb91c76102092dd3515a2063dd761cc95c9184f1786bd137a29bcfdc8baa71e12f207854" },
                { "rm", "1bbfaa8dd61b99c3e7b1c655c30823decfbf9da10f52fb137828c99c8b4596ae15af39e59af834dee3b383eaf06b259ec9bf08070247172a9235c34c2cc29d70" },
                { "ro", "a621381f886d3c05cafba7ec277e418b638f9359d4ad2d4331224fb2c39849380348d9cb665646343fe545b0d37ee801cdca561ac26c75c55a62e2a3c0caea11" },
                { "ru", "c1388aade881e0cbba169efa73d27334a7a6a5e0a18ac5fdb8a71ff1d39f12b30c6a0c7dc17adfa204fe95c4fa91ea72db534767c96126342fe9a9c08b26276b" },
                { "sk", "e910c0bfa9a49ead1ff105713e5b4fb23f1a2b7fa020aedbae3534995c43287e240834aba820e0936493469370a4551c7cec786e0a2c0c6e59e976d0227f2b81" },
                { "sl", "9236fc48dd07ef96d465c8cd1e7852f2a43df2b7440cea0cd25df27212e87b6f6d0d79c4f4a592656e93ff0b6d5ae37cf0f2ba76c83593d34620d7df5d7b2175" },
                { "sq", "6efd948ed7db2bbdf2e8a1d6f2d7b0012cc8359f536a2df35cccaab6ed91979269d888492bfdaea161c7582e667ada57344aa6285bd6cee5822643baa2aa7e4b" },
                { "sr", "3a609d9cd83c596998d7e8262ec148b2d640403e75796595bcdd6fedf00c33513a164a2cff317ae0ab06067189f8c1d6bd09e5ea14351c453dd235c3aec16416" },
                { "sv-SE", "40e3b355f074104220096047eee5f68b294aaa5ea7c5a87a63a0568746a6e703cb8ba1bd1d8dfdbfb071d2cf4c6ede3f39151f2a09cfb7dbd436bda8eb198296" },
                { "th", "24f76258eba63ec13c5c491769ae7c57206b578448e2b3b4690437b7b16537ece7798bc890169baff647247813ac1f8a26371494c0225952e34cec9bb78650f0" },
                { "tr", "1e6f6db1af693ea82f6fc253d34d563783c3551c63bbdc9d54e6cbea8ab603e3c2173e28fcd42dc387b6cfbb68ec6c335797c4fdd52b399b5b2b31cad5c148a4" },
                { "uk", "bee9e4739a1100c54bc63c32adc4f8e4320a216e03c44796ff82bafb311b92b38c9a6467ab2c2b971292bdd09a928bddaf7d7b2b143533983dc1df0c1561ef1f" },
                { "uz", "446af8d997a74f2b3c5708d2d0aed3e248ba957421c32a7a420e2e62dae68dffb09aa0e0700c3dc49e05adb6f61cef355fe667174eff5a9843c52a1e27d1e6e7" },
                { "vi", "96a99f3d4b7f1fbe823c99acecbab5e20821dcfa39b5d03aa448ac2f050a509221a9f9f04243a5f61fd8d798862ff3950c62a29ed02f9ebc47d8d16b97f092e8" },
                { "zh-CN", "66cc84512a167ae5f2a5e416dc29d70353302b6e44639193373ee7ab68664d5cad9a082d95c5459f86ffd645e62123d6311b3d3a04d22a72295d1353f4c36715" },
                { "zh-TW", "8a8088c53372012aec7c704e3cf412f9fc902a9f9aefe4cdf2849c108a51ac08b044c554c8453481323134d57aa441dab766e4542467bc6b2d4b28772a9c6af4" }
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
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                knownVersion,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win32/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win64/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
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
            return ["thunderbird-" + languageCode.ToLower(), "thunderbird"];
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-esr-latest&os=win&lang=" + languageCode;
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                Triple current = new(currentVersion);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }

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
             * https://ftp.mozilla.org/pub/thunderbird/releases/128.1.0esr/SHA512SUMS
             * Common lines look like
             * "3881bf28...e2ab  win32/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 32-bit installer, and like
             * "20fd118b...f4a2  win64/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 64-bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "esr/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return [
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            ];
        }


        /// <summary>
        /// Indicates whether the method searchForNewer() is implemented.
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
            return ["thunderbird"];
        }


        /// <summary>
        /// Determines whether a separate process must be run before the update.
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
