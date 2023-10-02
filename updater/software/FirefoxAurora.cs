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
        private const string currentVersion = "119.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "93aa880de214ee33835892c81ed828b6cb6cd4ad81654c55e2316971a92a0baaa3b83d66ffc489135c26307dd06ecb90368cb633e1cb5264b2618f655ea964a1" },
                { "af", "6e76d749b4b8f950b5cc3d5b5168c758cc9d9875a2a5cc8cafa13487d318d33facec623f9f88fd53b8c6f5f8249eac18c5ea9b3c504f8c8c0adb7343290ec0b2" },
                { "an", "4cb363555da58b6669d00ed9518e03add18d984b4c858b197036e68dd66da4028ec982756f76eebd93523397f7bacdc496690dd3f7eea4b9805ccec22a923b70" },
                { "ar", "f52d31e6d4e2859b3b091b315696f4b10098aa8a4119fd850020a1500c66656458d8104142e626e96b9f474a0f300f682383c534d0328496ee79eedc1f827016" },
                { "ast", "19b572c5bc8c9af82a53d8632be5b295bd543e2748835601bec3f41487b658d6be9a69ffaf8bfce58cfe1ca9ad31fe6fd4b20946024bef96aab2db6fb8ddae55" },
                { "az", "f98e68409a5c76be885056cb5d23287c8ab94e0b7ff27f2c8d7a38240d41c202515d1ad056e84be94b30326c30ed393fe1d100f603432710661754392b29a0d5" },
                { "be", "5eb48ca38f8ccadbca1b313e40a531b4d935a2703ebb31e3b3c27f10bd854a3bf55f711761eda4ce5c20bf3f7d58adab30b6c4965769ce9cfb0737f374bc853d" },
                { "bg", "92199f93e4f5be5701496e12ec04750c9b688d9e15d106ed5409cc0f19bce89022460b09ec8624131426910dbc755b7d60fb635af84218125502b553c171cd06" },
                { "bn", "3fe3f7e1aae8571f065ad3cf31ab3941b22ee1f173c8f8dad0158cf88ddfd4687b95a00fe9b798086ab6c5f76585024bf1eee7f9387bc544938117ca0007c46e" },
                { "br", "f61e6d87f19fd31aeb73a17b7d083bc476571c402ca01be36d73074233e358df9a5310e56cb9bbae23af0082a3167462f00a280be2233b6979fe37c902628a4a" },
                { "bs", "ed6a4af43cecff723dae317fefc8bf8800726acb4146e1a6c773756a36e1181796ece27047395b75789d9bb3de90eb079766463db35a23bd157b631978750df1" },
                { "ca", "28a12b6730585c1009b73d31bf2b5ffabb59c3cc58fbc8cab5880f0cc1738c239e89cf4c03fc0faf7054a7085d43bcec20b1d9a7ee2539f896afaf8483d54a07" },
                { "cak", "576bfda53a2df0940c44401faffad677355ac11996f0ff7c0503472a76f661e1b0b3f9da3a470dbaa5f60f13ee74dfcf65fd6cfbd135f0d7cd095c6e64ee742a" },
                { "cs", "10148f6e523cdaf4de93260283ba2eee8d68b77c6073f0148b7fa31df6ee49bc4d70e8c2080267c7f5ecb8d9c290483b2f8aad0afce5453a3ba9a2d32589e8a1" },
                { "cy", "5bedac28734da19f304528e25c9405ec1e1ff4c30c75e953b67b745f22375efced0784dca16d72f1e7f5d0126a222abf316d7e801c09ef9cf18ec9e98ec3ea6c" },
                { "da", "ffb5328f1ac71251ec3a4d33054fa76c8592b1e9aef5db1bb0df042c7e0fb8a17b200d9651366d0faa4b8d6cc921388966063ff9ce8b6b70e5c83150ebbb9670" },
                { "de", "da2981f9e959d575e4d46343f8410ab4d4b03a2bedf4f74183c03ac6a4bae01c434d31e31d1a4c2090b6991ea3b36c976ae3852a7ed0050d15578394ac36900f" },
                { "dsb", "8d468b31f5698a7d928290cc5a8bdc3f371ccde7b9ea1ad2ad96eaf08217c903407fc563461bb41d1535c2bd26518a71db694eb0046f2885400c8467a8892da9" },
                { "el", "8d8b263842afdc0fcb6656e4b2e5c6aab69d294faffa37f4d48b3ba1ee29f07d7886a94a8b67e6c46fc4339e64cde2e289e890a17ecffc0fbfdab5027778a5d1" },
                { "en-CA", "328af6b88294381f658b00c7f0012713ff9b07db9e9d61ade055e83f6d30b15a14133a1dd4b3218a179264bedee8fc7da4ac120e3b93bc0348f77b0a4a47ee40" },
                { "en-GB", "677176d73ad46f8f491b9523e3ac050278f4b3e2d884b9c5915c3adec7ac5b0737cab8116e00aee91af88fd506ed992ae0898a6839efe0f421d8959d75570154" },
                { "en-US", "ea7f37b917ba7716ea581f1d477708ea60c9976852291bba7a774c3724b3ac52acaee8b3c82e850085a03a9d209884399165cf855d6627eba6f0827355ef64b1" },
                { "eo", "c0a3b98cb2e7c84a0679e546fb722cdc408091f609b87d17d7488af1dd713a0cd27f49135d497a5466d78ec123d6b227a2f556fbb38338b7e0960a7fe9d828f0" },
                { "es-AR", "d03ef27d6054b21c78d86d28ad0dea5264685b1c939fc4ad4c0b5c920e0cf58b20bab6385bb01a9a2f9ee8db722a571fd614c38ed6a4baa68a806814983441ba" },
                { "es-CL", "c91b5a1ecc866d9053e8d40b5a242caeecedef768a3e8f6b8ef3ee70a6e95e9e35efa9b1f7200308a27cb346ab5eb02a114555c2e0d59ea9ab2034c177c8ae65" },
                { "es-ES", "7d6854bd032447727cf174d7bf6ebac8d13aed6a48240bc3ecb03b08ea1cfec0d27541b7ee563b45ef08f51820e9a9f40bd40d15901c6f59deecd33ce19caffc" },
                { "es-MX", "23132e60848b3fc5656e3f2c97033651e86cd8f108a2b91df6c43e06b9435ad7d2f6776216d5d214958601a407f7b273396cd6703735293857dbe60d3f20b9a9" },
                { "et", "21a93b1056bdca8f7a5558370e2d906761968c27158c81f540c1c4b6d3972759ab60b260a779b60940a8bae4f65d926af24de0add5c58375ee010c8874b8b979" },
                { "eu", "f010bc9fd9ca6a0e8024c06b3759ee0da1af77346176f0dc484b745262427fec7bfd6f5408cc418a5497d6fbc69a53ebf1f83cc207f25e1d86fabc5e09b7ca35" },
                { "fa", "48656e8a75b36eabca31f55c61500c26ca02fbee47eb6d5a557688a196e6f5f776dea2f3c5016fa8e03d9aaa12a3a63af3cbc3cc1b726a7d3bab388813ed6613" },
                { "ff", "37d43dd92e27e5cce60558b3366ea63554f26c05387aea2bfcced267f4376288b49b4803ad0f03699c397a9211c188b11a32234c6e1fafa192a50fb0e4af192c" },
                { "fi", "45a1494de7ebcbdc9e8f741f72acc96a2b791e4a173cc1d04293876a503e17eadb7651cc71b5548fe53ba4c2417e2791854d073c8744fc3218c142c197d9216d" },
                { "fr", "f8e92ecd4a93349dd9a7a1060e92d200f5aedc61e61663bf3ee9a4feab94115e97ab560ca0a50424520849e6d49089437acd8dda4f92ab426654be088842ff95" },
                { "fur", "6d069bdbd4c7b2f707fcdf7a922e5633800f566b56f366fed51928b8f3d2f77e904c5a6dd872969d98271cd73b4a1f5e21c8372d3876476ad23984c38ed5c79a" },
                { "fy-NL", "ff60a1f797fa9ce26cd832cd0c61e9a145cc9f2e92485dc803b3e5887a2175247f78ea14fc2fcf2c782a8c3a2622051838f1012d86c379fd7caaed2bf3072744" },
                { "ga-IE", "848a3d60474cf8325f900676e819edcb4a07c09565c34c43c917ef9d7003f6120f470296542451734a2bfc194ab74f77e28d8dd2887c157d00399c81ae685ba0" },
                { "gd", "a6c5b6d4aed8bc71ab4532f587f1e9e5927564a1a053eb32060f6ea7b0a913600835e89573eb74833f2254e7cb12332b4710bbe52fbe835897dbb608edc4d1f8" },
                { "gl", "9cb090c9c112ea18d672325c2caf24027412911dc5886a578e9b0d24ae224549cf0e04bbf31dcd5d6a6c7ab2861223147317007c4b34741c350e0ae2100cbe2e" },
                { "gn", "b761cbdcd9bc11f0b1c57fc664c20804cf14289f254c8f6b0eb4dfd8b2a5220dafa749e98617bb97e7bddf446c375b1a0687682ea1b9cce7fd514749814016d9" },
                { "gu-IN", "e7da204a824e384f1bb035ad291953ce18080342a520ad699dac4f26d7232cec14f3b9c09d4274ab22bc08cf1f71906a378c7283ccf447aed5a25f2d3446b1b1" },
                { "he", "66c9e249e2828a769d986e03fe6bde44964e78ae5847dd314dbc98c71a83fd1920b11c7d2eeab7ee61b5d86369f8b6f57da24070e9863cc29cf8578df86d80f1" },
                { "hi-IN", "5241f120f0c3f71451108fa02d7af10ac0b17a129027d4aff5c2231680d11300ffee9563525348f05f4eef3f17fa21360c3d225451c0c2c50730139268cd24b6" },
                { "hr", "36e0816f9b892c6cff585485118f33d84648aeda7722cd196a2aff628c6d457f26d3e1540cbb331161cdd0b35bd6990013b920167b65c65a3bc7c075de8241c0" },
                { "hsb", "2a50a5e7c8471d885767023a5b70fba9b53e44900f3238c7ad226742ba17136114e63290351d1fc1f49546038abb9e83f9db47563ebd08d77e0a4e66eb81e5f2" },
                { "hu", "2294973e26fdf8a088d717c2843e16f61bf66bfd50c662786519fd67b336657ff50066bc61cf0aff56441ffa01fa572769766a46f96602822c2039aabc6a66f2" },
                { "hy-AM", "fcf07930223c8d0bdf507453ae0b7228d7a0ee5607146430571f199badbdbd35b571fdd42f2acf492f88777e3dce0ddde648872d101e73508a9811f4ac9e9df0" },
                { "ia", "88dfdcb81377a533b863cae354273c60853af389f3ece87315ecfd6a592767046487124bf0644bf0b646789f1927bd73e7782265fcbf4767fb5941a0b8f12628" },
                { "id", "ac3bfc8fffe97019b548c54485f10b987e71eed67ed76dc44a2b9122af4b1644b8f4737ac8997515057663349a0f2ddcd3148e0478f0c3ecdff21ff0ec1e2168" },
                { "is", "6ee1132f1bc5ce2d84b53d41493179104300c5fa3e1739a39115b0bffbb93e65b0ab65bf345c49b478b238bc2bf2ca840459476ff8900f7cf880319c05abf5d1" },
                { "it", "5954ea6e1f92cad3637be3345441d7dfd8d008b0227feb3e501b225a2f3779f2688365301506cee287f7cafde0cf47c3dae1203ef883bb6d19e499796101a7c6" },
                { "ja", "54ca9b5628f8bcecc3c794ecc5386db5ab474f30ffaebc83ac9d35ebc914969b2b2447c1bfc9424b5e4db0d24ed7fe612332ceef668bc2072bb4d294df552f74" },
                { "ka", "9a4f4fe66160b2264545483386b4d0de03dfc4561392b8e0c8e7d88a8d25939b6772d3c9ffa6d608b7facde663c8fe386d283419e7949f999143d12cbd6d52d2" },
                { "kab", "24b6e278a70f23f27e8e79870a2a5a69fe766cd5bc63142d1d4d02d4574df94ade1910b390cf55b848f5610c506bebef4278c79cc93ebc070358334f7b5083af" },
                { "kk", "54d36dfa64c105458f4b658347082ee3a8a1846f171a5e79f19dac5277a028dfad6c49a0e366570efd3b630060f46f1a04a2ab17a038fb7bab8409117053c4de" },
                { "km", "0d5cb02eb5b968b2496c3329f7334150180d0f5aaea30de77b81286666e35cb2ff33bd1903bee5a0d3ec1856c55ee0b23f5a6c90d22a82886a68d6054443e88f" },
                { "kn", "5fd8818c8613539fb31892b608d09162ec42bf294a0d71f9d7a292e1012fa5c517ebf8c99e997c160ba96e19a22eb0b279d727376ff29e690c91abdddb07825d" },
                { "ko", "eb7868ed1009b31b3eeba225fe0b6f070ae1e00d31960bf7d94148efaeef2acb9166cb942ba35289dc6f5bd3212d8c7f40298e3535eb38964bd92c53a6282e28" },
                { "lij", "93ed8ebc6aabc46c25ef91e567243a8b9c320000736121e10713c00285c1945d52bf0e54e55babf20369b9b38453aa3b811c10b216507b33401ac4444c162658" },
                { "lt", "257dd243a7edf0b0ecf3c2d2f1a5eb91f38b95e4adc6a94e7498f2e4c0cdefea2d39295c7320a4fb083679c2c09e5b7de224fa9d376fc6ee62131a67317f5e43" },
                { "lv", "02b08bce426df1a462233bbe5edcaf07863d6a60e70bec18e5903a09a66a69afe9274e309a1acb532e8cbce2ab9eddc8234ca038610dc553119578a07379b820" },
                { "mk", "905d2842a7702f6862e2f1a1aaa6e41c1023ea922d2426278c3ed41c0e9f906493715af7a3ea5ff64d48396ccbb0762eb25fb5444ed8cbb3fed9d25ca8da3b23" },
                { "mr", "56e5c7fdd22a71ef8b4c83c94abe04851432328dd9e52fc025449bdbaaf51bacf04e54c25b7e5c6a9a494283ce33afa2366ceb2e11519078d0b4c3cd44745ea8" },
                { "ms", "94ffdcc1da6c0ed3a642c5294783eea56ba7619b28345c25b235c4ddf2e69d4acc52a3cca4ac49f0e7867d13bee6a6076cf3f05851e17479b3667786e72c117f" },
                { "my", "3778f22ad19ee154ab7bd712b155e71245fbb8f06dc018b963ed4c47075cc9d1aaeb71e7b5cca094529aa004edc9107600536cec7540efde0397554d1773c40b" },
                { "nb-NO", "c6110de35323d0726bea43a72de2f99ef360bcb202fad10cf5f3219138021eb23e9280f0afbd6220e43739e04a3088b5fd1e57c154db1cf6308d89396cfeb0e2" },
                { "ne-NP", "9fa65e1c1e3a36b158a65363ce2ee85334ba1f5548d8921bffc789a1a253f05a7bffb7e2136edcea9d58bab8933bcc1a8bd8f2e2498549e233c09760b1617ebb" },
                { "nl", "51b1e4debc34adc48c1301f3f3e89669591cd3062962699fb6eef4104a9be186cbd45ee723aa7b22f3b45fb271b496f1f2bf77b9b01107fe87b00b9ebc4843c0" },
                { "nn-NO", "977a46a433c029474857ecbc7efb9f2d4427706cf51f6af692fd417c9627795c52a6e0e2140a8a21e3b83870dc02d2735a56909237fc99bf42b067522cac3ed8" },
                { "oc", "e69ff2412e9e9d583fdfec8f9fac5fedd2ebd8314c7759771429e70b92fa41744761efed79753cef0e832ab6a4cc69e49345041020e833e815fa87a384966af5" },
                { "pa-IN", "1c23720937cf20b3cbc6064bcf47a016f34d96094221e65a147da4ea12dc5e225407c60d11d54003708b6c85fb5c8862cdb5764208208032c9f6395798e13e8a" },
                { "pl", "a17402586eca25b0af786ab5baff984432483d447816100f6ef51589d8893aa7d3046f2de3dfd1e337f1f69b32db832d478f288420f3162d4359670ba332cdc9" },
                { "pt-BR", "983c0b2f8488c317035edd04f02ced7b50336f835c0b312192a7a3fc1b7fe212985b7a468075a1200d0bdf2d55712ae1a9e947670be4b9aa01a11e8fe41a89b9" },
                { "pt-PT", "bc07f09342eef24c726a7b459dd427c1fe8c60b32f93dd039b07666f7f26395f49959bb132fb5dc2a7e1c2c4f7bf6ab41ff083d4a20eb54d8c5888666bd9db0e" },
                { "rm", "aaeeb5e5bfd01397f038d652b65351dd38c0e074f96c9cb57f61211bcf9a4f721e2bb3b49b986888f748357172421b47f29f5249a34715cd3d5656a940e0177d" },
                { "ro", "2a4f6e994c0a4929a2eceadba2af60c77abcaa9b161c15b1a1d719145a04a1bc30d06ad4946e35cbcb3080b4c31286160ddb8d96b0fdc534957c53ad55a698b0" },
                { "ru", "6592465216c672167d327a72adcfb84725ddf8868beea6493fb4c2060083d3230f405c116f1afda72db81ddad2cc3f98d76180911b7a88515951cc5c45d55b1b" },
                { "sat", "1f42b221dfbc6cb86aae7b6a69c90d01fc50dcfd84f203fba5dfb66562cb168135f6006a01f3993565c4056bef651b78933cd8b53e633db90838dc6456893aef" },
                { "sc", "df68e8d56be7bb903f643d1c08277c476784fc4c86f442709f510892f085c8651afd12a1aee032bc0868ff1b473643cfd62ce7e15b78e7beb143bf7bc3e68978" },
                { "sco", "e9fe779d94de0c7ed587fc3e6674ba71fbb2a250b8d2243f5be3231cddfe2fb3e75e3dcf8f662b2e6329dbc030a88ddf7b13fcb8dc2e27814b0194e5ed05b625" },
                { "si", "02aefb40b3ac24bd932729a23651a0671a63c7bd528fd40f40aedfc72c7be2cc8572b05b7fdcd664a647a824cb17aa5b4c9de4ebf9bedc0bfa3ffa61aced9381" },
                { "sk", "468d58e34cd0cac103d398e7235973227277efd3e52b178f633a3ca5dfe2bdbf18ae0a608701ac0f9834992d69e97a640476e15d0661c5813f70b41dd11b81d1" },
                { "sl", "6639ee0b6b3863e039a37559611e9dada46977310ce48460a9d75b35b7b5c4367c3fd87ceadfa2922b9a664422ab8b9b5487a04bddcdd7cd31174d58ee5c7704" },
                { "son", "237e6e22a168def2f19e5fb5658ed67ffa8e86ae73decd2a828db5d95f0e75fd905415ee41e3ecdd269811e7b812e0c409e1bde16a221c957aa9a7344ca343a9" },
                { "sq", "970c0edce3aa486898c410b55c6270ffa679cf9b6ed75b5dec5809ad972c59ea9950d5a6e4e1c9dbba9bb1a7fdf46c07020ca9db62df5d226bbe42add35b335b" },
                { "sr", "18a8ed0bbe959ddb387c6824f1217d9718accd3b21185b7b022bb7d82d01fd4c31cc3abac52d343d1c8c6050b77eaef3c52a7fdb2c5757f9f50dab471ca41841" },
                { "sv-SE", "9ccbd6a7450e699400e2e440c901f4366f5e042da1c4b0d3ad9311510918d62b1b6e76a43d58b6011f9cad60f8604a5f79c145ea49d2a3ce63670ed13a2b2725" },
                { "szl", "afbca9539d8fe04ec4dfba5d41b8afe69a4cbbb4c6042a3f4da4aea99283c55b7e72cc2bdf44f3b11a648d05b85d1e891e13419bc7b61399b2a6c1640846199e" },
                { "ta", "bd564bf916535047607c5ad8216d62f6ea09c456fb915add5515da1c57fdfdf23c2e80c9a3a3c153f85c7bb80bcd6890888910949fe490ed9de568fb11384974" },
                { "te", "f65996df06ed751c0a23089089c2531bd335a3ec2256e637ee116439807679a35f874503cf3a5af01e5668a41a08c5da28f6948ea32f5c26620c0ac8b9bbd0c7" },
                { "tg", "661a4fd768d227d09450bbc7e3989b37892e0c5a05a0985394a20679e63920da9e3f170d4bd3ff4c1fa1b2ea4e933f2db1a87147bf6add2baf8e28a25af4ca23" },
                { "th", "c4e8fc50b6f329d5c050eb605e518b20cc847e6629fa7e92c4bab71941877bcf696237a72f847d871b4525cfbd414ccf4a06a3fee9be12df0d5f9accfaefa7aa" },
                { "tl", "15b7c4d0771f34c1c58aa5c34bcdceb9d14637d833bf97293866defe62d8f0865181046625b46a667bef07d7ede3e05cf104c037f8cd195b79298110131f1ec2" },
                { "tr", "04d9dd1c97a82c313162c817592fe1ce5a23c595256b4585e9d3f95822102b484c59b14f1d89d211f39226b00f5c55bebe5c9b908a93f2403821e25a85858b64" },
                { "trs", "baba80fa3ab9b799c34ec50c5385bd1a9a17c22a7a04d96bcb7255300e2e81c2aa964e94a211019f71b8f8b0264cde4df31c2ae44d30a397e09379f8c0fcfe3d" },
                { "uk", "24f8e7dd760bed8d7cc409302731c9f7c6dc87385fddf0f65f130cfed3c4e80a2511374bd0f5771bf97e8b42b3c2b40f25a85c9493fd1689158bd31306a64f71" },
                { "ur", "b6ef1fcbfdcff43dc93830bd5e70b0122e047e0729e21ffad0abce66f064e3ba4ae49a1d8a36557acf547fcb35fc159f8f88f084731665c8787751a968399692" },
                { "uz", "3769a66b5881f5dcd8e745e76abc0e1bce4bd1463a7ec9ced3404993fe3062a33e0bcdcc4fb2e835296d57eea0735357d6bf06055c196419845c8f4ef67a9936" },
                { "vi", "d9e7d9ebaeb15086cf5f505aff8c26fb5b4fd08d63e8997fa990652f7704f2be54ac725181ca6a6674f2bb33e08d6fbee0c5acbc14325cb8f61f07690c4ca2ef" },
                { "xh", "57e749d802a11249915743ff1d84aac29dda64fa83634c16c769389e04ed291532979dd93ff9e4c43776f8072dc9ff3a1c810c26fe27ccd8a10e0e14530d8768" },
                { "zh-CN", "be5b62a06a3cc3d9a5518ba7c904efee9697823d62baa7cd3a867bbedaee30da1b17d9e4d61b53852833b7dd200cf6f901cb53577f608a5e8157c85f590a6a87" },
                { "zh-TW", "acfc9578d30d020bc3492c8398113ca030c798488663478c04deee4934e41731e002886cb2e0a19db450dea963f5a4825b159a3c7947e64734e2ed30ccfa395c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f907ccb70da4ceafcca956e0a95c0d79801dd24ae816648adc066a8514e1edd29fdd8104931bc1c0748214baf58c86019eb1bb8873ba041474dafda45e10688e" },
                { "af", "30e703646be46f9da0d93ad0bac7ec75198a8ef832e8509af86ca5871579e53211933004a23f2f704ddb82e2f8f10da1fb966cec2a87a9f4e9b21222238d0368" },
                { "an", "17ce69be95dfecf625d71ac9f71a10408cee8b7756d3499f8a7ac6a69b45acbfdc188d9258a7280d6d9cbc7a1dee3661326454ecce49795ad2c7ededd3ef6e0d" },
                { "ar", "4e22016790941ec3baf29523720a52faa5288baa053c855ae57bc8f35c67a255b34b0a43131603f1ddcb8e82333b6079e742670832d09998cfbb5e1500b3b05d" },
                { "ast", "e692431f7514c13a6e2f6731049d489589ba5f284a9dfc6b933585d3d2b6ad15a45001d968cd41010e1bf7096788c84c0d6dc37cb1b1e317e65d3ec894390bd4" },
                { "az", "3b299c540ae8eca2a2f8155e0b6a5eacdc7d68343d7a740a6555412e540d4e450eb89185a934f7db0b447925ee5edb1a2a523072aa9c0351349ee515b010353e" },
                { "be", "52e6954ca86ba43e61852474841f0fac50a588318b74b15ce985871a7086896d5b49d0587cf6e49703e39e0766bfd87fa3a4c93fa6ee718884961db50ed04310" },
                { "bg", "8aef3bcc580c3f79d1ca15dc45bdcc54e0265b34ae6c3dab84e3c90c2aaa5f2e0df87537e76456f9eac0c83f9e1ffaac4b8ce9c8c8f66847412a20209dec6cee" },
                { "bn", "9b4b553d9ec366cf2035f4d257d18574a1b19c040e9a4914f5bca7819e39b6bad96707107d47eff9bf23ef57c0be5d744885ab41ad589943ba8b357ce7068b3a" },
                { "br", "1825f5a3de904fb027eb5d760dc140c2b68038f10de1399908c4c001f6aa89070f75e107b39be705bd72bc66e2c35a5dc85f8b2db96f2e8af32b8a21c564524f" },
                { "bs", "2410b0cfbdde44a54f5d84fa8963563d9d6b3bc3f3f1e7b8a58a0bd1391384c2b000f9f5c31788f64cfacf44a3f965cdba0d5457b595363d06bce9fb8a56728c" },
                { "ca", "de35dc1d9b27235c4728b3c83b5a7ecd5fa88e87ebaf5ef972a5ee438391b9f749ec8adcf49ae61e3a005c3120d35451034abae98275f1c20e852c5873031e74" },
                { "cak", "cd08b999b5a29e301b93731b1153978173b2051856d82a97bb63290bb9e757429bfd1e636f46a01cf54854d20ff16ddaed75732829d9742817ecc36c60ad157c" },
                { "cs", "2af86cc9b3e8bf0d027bf07b9399953c7b0e7d01b6618a1201d4ac2d18b7019bc84df586c655cb5107a36fabb19bd67bc060bf4d153bd224a9a2faafa264641a" },
                { "cy", "9e53002b80c88bc99337aa0398562e1f52085df02167ae412a1393dc8cf79c8f4815eae883b9f1375bb32e196a731ea1b6d9b6a6b667151b0c415084ef82bd20" },
                { "da", "0d6aeb6234d551cb17783c1b13535f4945e88ece01a30b2fb0f2c3d51efa2be00b973066e8147a6bc1a5bb13febf22bb41c919f0fb849fd16b283999f3806e14" },
                { "de", "eb8d6609717e87c313a62f50b52e728f18ecbb02d4f9479242aebb56fa089384a5163aeec07fdb160c66c6bec8e25907ef4395faed9aa496b7de77e1bdfb1346" },
                { "dsb", "7af9646845da086b80ea421591163d94007abdf29d8b921d81cef99c0d2a63479ee591e8e67555c0c7de6ad59a8147f35e32c0aa0d157d58a3a0c7f8551aebe3" },
                { "el", "34a5cce9f344607b4d1df4b7f458531405876fc8a720ffc325a1346d86b967e00e3bf599f94f52e8c4dff25a799c9146f548fe77abd618b8c674e4343745baf4" },
                { "en-CA", "3026516d639ae7caec777972e1dd4b35679b0a4a4848f75fbced70ff35676bdf867b3a6d86ef3b325781349608cfdc3331469fd2b4b75501360636fc1446441b" },
                { "en-GB", "0b12befbcd012ca9eefa6d7940f2f3a1f19df14714cb02975ef5e24daabbc5d4fd3a0dbbdfc9770bb422ee76443558f7843c3cf4e3f0e36358ad04364801e4bc" },
                { "en-US", "c736f7b5a3f6070c38ec2905a5784a21843b4a4fb74efa9f99594c916bcb594595a186d0721a925a3eb79974d31bcc72e91e077af546168631cdb879b28b1c66" },
                { "eo", "a958c670f0170b9784813a81f477643a8c48e848e240950353ff939dc8aca54dc024a139c7da8feee906be5c54ade2549d2d199d586f6dbf48d76c2922d0ce96" },
                { "es-AR", "8b1621a9119f246859d6fe2e002ecb0587cf9524860c45e9cec6c9e79633d7de507039d11777bd405d1a5229148fe0c91d08d716c816a0abc306a4a0e5210f74" },
                { "es-CL", "8bd2bc1926a72a2a2003dee51bfe9595211b7a8c8eca2892224473ba639975774c2ed6d397e0340eb4d8f2d15cc33ecf9adb236c2938d2dd9909357e25cee13f" },
                { "es-ES", "a6774f2ef09f590ed0a27810e1d21ca48916fd6ea103cf10574b39a886de7f3ba96f2d84d2c17194266ce543c2620bddbbb686d665bc3ac19555b1f8f642cbc9" },
                { "es-MX", "a05b89ed7c051dc0eae4448d6683921d0bc2c7a2edc633c8e00ebc68715ba0c66ba52e4e8ae657c3c342fa6b132ef746c06c676e10a3d09585c6d3bdad6fa3ba" },
                { "et", "ef06e5c8dea8bda83b67f73b255867008a21bb7021a31bf6d9f6d50ed458c7d8d311a18b14c59fffde9a804d735d66975f4a2e373c6f7b3c6dc8742a28a0fc88" },
                { "eu", "4e390ca95aab65f1c09137a31e55c0b4df02dca9fe3e3d39ff5e45fe88eeddb9430a3e0b35c5ee42b477808ae66826b5a5b9f396fe54a1e5082330f391b4c0eb" },
                { "fa", "fe5021172b22ef3764f809f49af3fa8e999a892a04fd2cc623259a8f8b2374afb95d6ed19c1291cdf910a37f7882890a91f789267c09a746ffcfd349e422f616" },
                { "ff", "a211cb7aea47fe32905390a0fc53c751500590ba58c26a63b6931784d5701084f0a7b8c560e3d325e83eeb47de5a2e7610ae1c07a409c81d8ed6a8eb27f0d048" },
                { "fi", "5007c4f895f6bcb05473d641b3b87107154852f8c4c9861b965e810b46ba73ed83634a3fc29fa63bdde7824d0699b807243d1865425f35afbb69919836a278d4" },
                { "fr", "128c54cf7e87d6b897512e329981ec8d73fa600d12325f5fe4cd240b60b952a244a5722ea28c0e9ff718c13665e168107b12c7282018bc0ffba07e8b253d8995" },
                { "fur", "18d2fc191206c9919959f791a935be6be62caeef443e9a6aeb5dbbf081bd68475d2d6d1752a4ff1b05b374b39bfed77bd46d1ed023dc64b6e6919d9775bf0711" },
                { "fy-NL", "784e8299cf9601116181bbb00659812dd3c36fb55f5af82b58aa8836e22dd402dd27fe8a8dd7c302d65e025c761a6cf2cdbea095dd56b550760fe7901c9bc36f" },
                { "ga-IE", "cb417749b5262b9664ac157aa984daab563b5504a84958a7f3ca77e0ba05c39a061a01deea24e0d2a436c9b6faa97b02196c4126a78e39870de96814bd7949fb" },
                { "gd", "4d4bc538f69b846d3fd62f73d2d37f95b350e2ad8fe8cc85801776c3edbaa6a131ea1adf9d866420d34a2e056d79f534770e67e7839d951727db132079801a23" },
                { "gl", "c2528b8b35429912ac3d3cb6b94468163295fe223d49004ef6ad0d004ea1fedc60e903d5f9f4636e648396accbeb24b2262094ff2f99eda56b9964978dae36fc" },
                { "gn", "e97845fc75fb2625523a9715a6808402a5ae7fa9fcafc748949fd89574c43714d639978290865eaafcef6f703973de2554baee34dc4a4bba2382cde17a99f0be" },
                { "gu-IN", "59f4db17934c85deb4dbaf32cfebcc6cc7b85389e35f5ba41f598aecfd7dfe3bf0af91643abb04743f3214318b06d0550f0e5caf319e4c52a6d0914858f232ca" },
                { "he", "5ecdbc8f6a4c4469026cb0af03f45c15e905f93d2714b8c47db3752df0ce73da693b78afc6975d723c9e0c974ac216dbfc1e27de0e6b8b46a70c7430f24891d4" },
                { "hi-IN", "7d66d18c91a8551c93dbc01eba3deeed5769fe47fae837aa94b3ac38f8c3169fc7512fa9cba8597c87e89af40dd193b7804c0f25e40ce96c30ec5c34caebc060" },
                { "hr", "6f11cc2e213698431caab79c84ad5f3b9541e7726270cb71851f589829445280ea315e02d2418f31000863de1fba111496b1aec9042fc785c2e1cb0b66a0af4e" },
                { "hsb", "b3d9013fce1efcc5afb4b0ed654c53770a766ec041fad5ad9a3fe225635754ae9f6acfbb7d109f6b6ece18dc99da3dc22f78010e95da068786a47f7f175be93a" },
                { "hu", "e02a7ad0856ef632302f948768fc9f21937d5a40ab41daf2496ffbe2f4ef04c54043b094e097db720e1978f56b4b17dfad065425786eb6dfba841bc9a71aa242" },
                { "hy-AM", "8e21de42b2b620383961df0ea310d4e64ba10a7dee99a89fe1cb19a3f0aa2a9cae139755317ccb94de9f97a85947b97ea5aa6237b8254cf8efbae934dcad7f4b" },
                { "ia", "22ea0bd9960f227398ed874df5b2d1c914d80015bd2a787a281fe90ffa001253d8513fd4348048b561dbfd385aa714be78e057856b06b82e4680da56b0ba2700" },
                { "id", "11af09c4d57bf188faa40255dde35026664c439f9ae9524740404443bdbad62f0ad2e4332fe974dbdaec1a5a343c3170b4b9c72a3a4bb40667239a8e508bb9f9" },
                { "is", "23c9fa86c17e99202a713f87a4f1aacf9a26dccf9a186b87723da84d426b2bb87f63adb7144b74ce4028182b878107c936bedbff7139f66c3bc46087658847cb" },
                { "it", "b30c403dce98164d4a5b42f09b552bfd81f4e27b55552881f56adf345c1e0e44dd7e84251fed6fb26e51d006df9bc0fa7b5f0308e229142fa0598374bf514b62" },
                { "ja", "f4c911641b70647ae2e561fe73711b3af5b5bf9dc6863926e1a1bb17e5e0bef5a80c08e93bce4007c17e9eb64714a4634142e5813c832f8163548204895d581d" },
                { "ka", "6c22dfb93c6864ee14b8b9ba5800668d038c9d41e94b872afd398fa9c4070ba488500d9316c4e4cfe52acb3680024ed4748eab077235614042b53c83fc350078" },
                { "kab", "68372ca172b7b3f14fc454ad7ff81a8fecfa5a6e82528eef9552a6cd9d7092496bbf6e28925874228a0818c8754ebe053485f6da11014c8dc1c5e88cff4b9b91" },
                { "kk", "1f0ff940b89e0763dbcac466c323d86eb695747e3d2a3746cef4003e92ed0cbb1dc83b38ea54bf7c6e6292b44dc6572301a1cb2ed7d9525527360a8386e9c06e" },
                { "km", "6bea0c1af73926241eb2d1ad15f758e97acd95e83eacfff80d6678508862b5deeaadfceed0b164b29cad7044337f0c9c37e13e0c9429c23af61c88394bed5d72" },
                { "kn", "ac4e7aaba0810fc989e07391e767812bd6330e12969406015b6040f17abf8a4cf196f1073ea47c678c70e522324d2ab55a5790c2eb75687ccac36047f6e05f49" },
                { "ko", "47d6c165a6fb4b7445b8bca784eb04b69a177fcfd47c693c0589ed21138d058afddb9c27c157a3a7f8144a8104b4deeb159b03cf9ea7ac44374788e5648c9d32" },
                { "lij", "0ab3dc4da2d1e2dfd591a082c5d1a202190fdb5c84265217616138069cc40e423f3122753402c420e6d9642e3c9f5589edf630ed80c471e1fa998e4661c72c2d" },
                { "lt", "1c61cd10e41878e33804769967d9c6e78b6b46e14a62898ce76c606a98d67221f0f71ddf53922cc6669556c37102c094dda5bd8ab2cee6f51390a381697b7d13" },
                { "lv", "a51f1f4b1d0fa640fed11e1ed1dfc9cb058e03fc5614a15e35564f1437212b4a4d043433666bb7873cdcb36f9db56b5e37fe64a7aab0debca11f8ef38f59f7b9" },
                { "mk", "f363a7048c59a10f114fc257157b2f785dd3efe24f4fc1576d38f1f5908581deba6d110f76dcf1376c142f9618417182cb668e86a5f81cfdb16f3d2df7f0cc73" },
                { "mr", "898edce18649c980e9dc90877a075437802b11ff557b320f1d6ea8006e17d089f5783cb8c3732b4f0b7a7285454d0d6dfc811de08b37867c797a1ade7fdc3b3e" },
                { "ms", "12583e61b4d5bba020d3d9d5c490d9da5aab947059e8c00b837590587e05191b1c45e502bf22272220e2f06c1aad7c698fb73c072378ed4b797cee66acdde857" },
                { "my", "c423224dba902fd47a2ffa58f7f089365a445cf17cc80445bb54b3c87ea7826d8dcc05f18a0120e8dfba698f0f0820d5538224c40c5ba0f7b016413ea085d5e8" },
                { "nb-NO", "f6d81c2d38b76d3fcdfefeb1025c3de24fc57d1f2ca935e9600a899091e2a98d0551fd11100e68496148ee02f5fc744733590d9cd0b6c582d0130964c2ad9055" },
                { "ne-NP", "1326e747542bdf1c2ab67647008d1ec139a547d7aede5cd58cb415b5ea7d7ffa68a0327747154e8611601fc31dde2b6fabcb9d08ef6cdb3337a60327b7c4691a" },
                { "nl", "30bcc61b0fde10def31a54479714f21a22572439ddf3ef1f2aab79c6da2f8dfce6c2c85bd99fd5cdd5b90b5db0cc32e17e5e9e80d74c5a9c1afdab78af962044" },
                { "nn-NO", "709b6fe4097bdbefa1cbb89727bdbd84c2114b3d7555a4fc341534c3ed25592448e2f9f724a2ec63b7a0be7a252241773754a276af740183aa3a27c6d3fe66a4" },
                { "oc", "fca3c1f5d9575c8e61be1d8ba986972c67481190db54e08eba1afb7594780174cd47e1bf4c307a6345b4cb48fc17ac881f5308c1e835253c93b6d68b79076c03" },
                { "pa-IN", "0f1bb33873d993558a7c38842717ac4e1bee4083184b13fef94f803cbd023987586daa0e0c8b230c55a983aa6fc7c088525f90458ae3600110fefc242a0001d2" },
                { "pl", "9c96e6325e536d30be8094ba5a7db06fc3922ce33228f1a8419cf3f00d6eee44660104b93698a83c4ebfb947f3724228e6c9b981386edace1c318afb59c0d8c1" },
                { "pt-BR", "930de2aaea76b1d54734491257c090c3963fc80689898da022c8663aba08aa618ef68d58f2b4c070a56b517012075410c8d6d6346f9d7923ca49608940f42d02" },
                { "pt-PT", "7f3190bcd0707f3e91d42042681b04f45d015c28f86a2c1cb607161629714d2dd2335c7afbe680d1597302b43b5db4088c9daa5072cec99378912d28beafa2f1" },
                { "rm", "9d6970a5ff9c228415260b39de4be524a236a0f36cd60680d39002c739207a009396304473c3a818338c81e035e91d0db54e961e35e38291355ab65919ca7d5e" },
                { "ro", "40b54f89d356728396d1bb9076df5d35a15c497fe64a789cb99a82f2ff0ea48854e9acb9bb9285bb49b8e4c14fbf5ad0b50cd6a3005591befc8a7f320b663950" },
                { "ru", "0fe62da9ad7296a1ad77c1c10d4307b9da4bf2613f62ae382e159bf60930b5a3ec38d2b02d80725ec215a2fe02ad2094df3677c2508382ca83a8aee24a155360" },
                { "sat", "cd9dc94943eca855a43f8212db753185405e7f07b923503c8ecf5782b864e7fe9d37d34332be212a4fc8e4393a7c800386c457a88afbc44887fcaac525e4610b" },
                { "sc", "83716ac3e4a4922d9101399874a79e13cd966cf9fc34afde45a113a290346ff6fffe202dccb2b891d3d540fca38a65b3d347f51ade588bb89259c749c7d08b87" },
                { "sco", "7032e91dde99353ef49358180748d571e7f746f8518d7eeecde7551a4c7484d6edeb4ebbc32e9f9a840b51ae58e89b12221640e28dc83b8f29af5925ea61b212" },
                { "si", "42bc677bd0f3626514a4970495acc0dcb16ec66d0fb24c49f975ee366685e9a3982a89ad01569cc16b512aeed271b13e5d81751ae293f151ad5a336495246b57" },
                { "sk", "894e792c4d1f15f113e2f3b4334a8ed47d71308d782c784b1ae5bfcf867eb9593da9ba189d864262ae2f3dea537ebb2e8331f4a089dbba84d0f766bacf755361" },
                { "sl", "b5a9ec2d605fd62c219b6631e6d676891ac5c963a1dc543608c31dfff726153f5d2d834a98e613be7692dff2a8c7fde000710679e8d0acca69dd00b5387b95ea" },
                { "son", "87ad9d9d2c139ef223738b61898f5df774937992aa6211b6339e184286aa21d7de1394e3d8619afe29193735e85db3d3c522a80f1628b11bb7ea86a3a391ec71" },
                { "sq", "28dd5d84c0f7ff7918475574539482a11a7634e05c574936bb512fe1f44c73f4bb9be442b328cba85a1727890113717f6b8c29870446de1180f6749696dd7802" },
                { "sr", "0b77acbdbce7465552cd477aae13bd818416b13964d6cb500daca4c15e3e26794e36cb9468e981ebb75ffd8d0292e402189e35e3997fce7d90a524821adddac6" },
                { "sv-SE", "2a763bb4cfa5a375179d556e827227577e1ea871918e2c952936a5137b39741571a0592f7c226fc12d8c7dea2e410e689f1ec258cddead5d07796acad19986fa" },
                { "szl", "9151475508142cefcea619d87b322060e6cfa2d60cfe55032ad35999f06b5acf9fc092697171ff72b3bf9f847d16da9d5f0bdf670e16e123d7439b1da5793bb5" },
                { "ta", "f0d0e7ff083b710af53151d396be790b624f1ddfbb55a64a9f373c37b3ac631d718d7106bd08ed3c7b0d63af07c20fc02a967c6efb71ee66b319550564145d90" },
                { "te", "224c9bfd7ee20afa9e5516aff4b15629285e8039659cf21b20c776d56d56dd5fc84b5125b619f095bbb5d9b16b1600bbdb0b3e75bae42c6995d60d6ac435dc67" },
                { "tg", "f049ab8ff87f459d625c0661b8bff972dc69395f5d6b0cbf87906ce3d531cd9cb55c1b05bf09604a241a99512be4ad971add8d926329ad8b89d7bda5d4bb6a06" },
                { "th", "fe5b6b9b9bff1dc604eaaa74eeff42efdea81c9e6f24bf3625f29caa1d82373bc3c9898f85283250f3edc0cd45b6d0d48d8a2245e17557c9ad0cbb7be6d99099" },
                { "tl", "c4ab59a11c3c7b4903287194084e66d2fcaaaba66698a3d74d505a93d1e77c49775d63fff02713f9737fd3f49881f68fbe9010ccc78d5dbdc089b66f3bbc8b4e" },
                { "tr", "46fa6b2f92c1fa58b3483c807471a6719d0af48614a14df31b913b134e58b0a6585c706e463a040487916ce2566a2fbe78643c3ccf7cd0b8cf394809b9d1a795" },
                { "trs", "4682566cb598c441bf9feeff80038db7c827067c221251a3c0768470afbc84456eb6866623d49cb3419ce51a4d504734a5abd847e18433bd5963b9f64ca43cc3" },
                { "uk", "146c36856178b3633f507f35ae3fd0b757d5fc21bfc4b328e802f724b6362f02f52989ef37c7332c0a602722d6de86b8d15903b2f8acc02c60e22cfc13d8866e" },
                { "ur", "f8bc0f68283b07fe1291036f1fbec4f6012f885b03b644732a66c5f909a07efb0a382b48a466578f166ba612225b46a8a6b225b523e9efceeafe8ccc44fc4f47" },
                { "uz", "b990590c581c8364ff6b124f9ddad5a6903b61393381e4d65ff5eeab2a4feb6483bfad3fd3b2543e6a4269e92cf96abeee9597bb156d389a51239326e04b70f7" },
                { "vi", "a71d4888a9bde2a2ee43beac230eb7968cb10ef5a223b33001af3a3e4c2421ba690cc15c1ebf49aac949cd56c62c52f5a80c0bcc4b8c3994b427d1e15895105d" },
                { "xh", "b402a51beb0a3edce091ddac75bb842e0f6069b6831c3d27b3583ce99b9c9909c00f5ad340d235fa690e8472aedd4a03555d20b7c210f215193923201092d78d" },
                { "zh-CN", "326b09379d4d6807095b8294b2c7fec32f8405557100566f82c01edd3ff80a2a615e954f3b2c4bbca89b4c81b732db0ca4a2c0595d2098fe9aec81a38cc19ea2" },
                { "zh-TW", "6731837a961b869c115d82b395d4c89276ac095a447b53e6e842989c2b1e2977cba4179ddc432da505313df2361203a9033a1a6cc0dda19ffc85d30025e31f3f" }
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
