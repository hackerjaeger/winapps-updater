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
            // https://ftp.mozilla.org/pub/firefox/releases/125.0.3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6af35d2b8bcbd324e6d56e83050baf146fb3972bf9378adce22edd39915710352b75b4cc63b703bcbb5c7fe5f848feebfa2003d36a94ed48555d4aa5daedb0d3" },
                { "af", "5765bf77b84940d7b5c00b6d72a826285f93bffec58fdecca4119f7b88981f5a517059352c4ab3b9199956ed4498f0effc953a9e9e82c4fa115ceb935aabce0c" },
                { "an", "ce92ebe6cd4fe80b9732ae1051375d141f17fa057ab043df04b93541fb76bb4d2fa6978152b1343d67523f4f2ee3c876f18c8f78dcf95ae43eb03f0f04da59e6" },
                { "ar", "770b1822e997b0d2aa14687f9855fad172ff004e87920164f43d561e401326b1eca9b4b20de8406e3857805dee12c7cbde0409b9eca75fbf04ce5b280fd2bf15" },
                { "ast", "daf3df90efe1667c8c5f552f80ccde144ffac36b7e9e68815335d53fb6ba3f32f9f9c46ffb5589d26f3506c760f24d1e7bc04bcfb53ae3e82c91b27505acf44f" },
                { "az", "73a03d05bbd87bbd26d8b4d421062c0202c0d8d73ae867a6b6134f6211a8e926c8dd15bc3159c53792d6689882c86d4d6ed7f0ed77ade26eb5978f4c61552fd3" },
                { "be", "0f2fc7ca67eb1dca299935c2303cd60ffb16aa6670945310bddce617d9cbe8f5fc2facf3ab4750358b81d233b762ca428a3b80165b6215a5bbc2fb8dddf65b16" },
                { "bg", "a26308bb20dee813c5ea6d7a28429e56eed4f469a681a1b6adb0c829f7fd3ca3301442386af27cf48e31475de4f6dc5efbc719f866a429a077c979fea00aa1bb" },
                { "bn", "cfd8b0bdb4ccb29476a780191fae56f918fbe3f3e1a6a340ea8877f975e97895610dfc1fd8abbfdef5dbc010138cd293008508a3890fa9904d119576fce00344" },
                { "br", "bf3f8058810cb1c1be5bb20d47e68981b21c5e5042bc24fe96e0ff48a80684066f2b8e2edc3fbb46f0260a1dde01330d2074774466e3423221836a6624fa1818" },
                { "bs", "8a42c3108504a6a6040a49a68b97558c666fdad063cff70c6806c3a7bde4447af1b4e7546af706447b0178923a242bddf1176eb27dd6e673ad759f4090ecf3dc" },
                { "ca", "a659685a6c48cf25a43809718290d6d4b38f12831715eeaecd8cc719e77d0d26ed13c4d8e68da04d3055831495572596f82e0c2d7d7021b29c69eee46189602d" },
                { "cak", "672ab34224eef7c4b24e9c4ed09b714ebf5d1745cd403461151eb98c640d0a07c3880251b5f0d074e3cd4f4ec84ac97000917781e997d36faf587f7e64c58747" },
                { "cs", "11aac2e33af673e890eeeda1f05b75d90c23b1a92cf1b49253aed6085eab65f33928c07ef39ba4dca66c7ce8ea61b9101f41a2e75f711c934db7d53c706d4841" },
                { "cy", "42e402456fa35eedfaf369298c9b057812254eecc07209bd2a29f5020765030e2305527cec1b9c565a2f4e57aec33b70b0f206a126b20e1acd8e361ab118b3a0" },
                { "da", "0deadf7b9c2c4c76115e7187ed0a3fdc5e985dc04eecac61f6cc9f3cd385e9241f773465813cd1ea9506df0a6017d9752b5c845161de4d845e652868d67a9ac5" },
                { "de", "79cd57d2cbdb7b6b79c0cb3cedaff3a4fd95a941b94546894b2dcd5559160f7c79f08ac1322c0fd6a4b0f5d544a7e68f9586026699ac8a8c68c4326313f70f5c" },
                { "dsb", "9ab4224a9209e6ba7746a2d6c1b4d8fc000eecd683df5e025bcc904a5a3a5bb636608e483aa9500b3ce537003b46112206ae7a0d093e801c66ada199e1933513" },
                { "el", "eb7ae17eee8eccfa8a261042bcf70b0d11bd153ee351649570f37205395b25f6dbff7cf801691819abf4a72cabd603762af6a4068b60d98df53093baacbadf58" },
                { "en-CA", "df83587188c0a5def6063f09f56edce12692a3e7f265d6835989911ab22ef4057abb8d4358bf851b44d070ca44158170b49c9befe2fefddad40cb5cd144f89b2" },
                { "en-GB", "aff3e9aa5a7c9d838d116e7befc7cbaceae308af57ed217678e96b960638688cff6044c8441d6251defa28c78e8b970e7016b89505e5bf0259007eb8fdf17958" },
                { "en-US", "4e83e084d59e23747030bc9587072ac7aaa11774b286c85b4c56522d649780b8c0f2b256c4176bee6ddafa596f28ad37fcefee5f2bfce228d9bf1535f06cd9af" },
                { "eo", "3724e2ea7d0d71a283461e9162a737c3b06c5d420ef6f0c9cb03e8c010ef74880dbca933e9dca90fc3e6aa8293638f4be6606e710801baa83f30126ee7826139" },
                { "es-AR", "36680b9f3a69b75b8267078bfca6f103f5a6d3b916e99c55f98a0941b4e6d6df66863a4388bfe933421b5567f0c33bbd57a9bef1c5d9a8a83af9dea9b4040e71" },
                { "es-CL", "b34cfba0eee047a32f39117cdb0c6eaa4949c2a9164483f4d492382ebc4571a230da62ba0aa9e70396e5d23cfdbcb0d4e8ed61bc976755a5a512d0d301c44cf4" },
                { "es-ES", "168cdab9a844441baac68bed14ac370f282efd243803b1a99b0a7ff91672a7f7c00307cb05ee37e86c8c6f0686c6b1f65cdcc410787b0851407c12670f4d899a" },
                { "es-MX", "43cf0e833606b52af16683c06cdcef1c3f4de83cc459d7dbed5b8b31527e3e21c3c4bda217de646beefc73e28899ac6aba772b9f559ac3acc955b51e3947020f" },
                { "et", "38ee7a42fa3f56be3d709cecb35db88f12c3b1ddd95fe3dde871976d9322ac72d56ea4261e416d459554ff32f8f1f86c26e2b98905f791e893be5ebba3565e7c" },
                { "eu", "5f8c51b1b2033c3d73bf4cd4e090c6240cbfb3e8ea669bdeee4ecb28537d8ab53fdcb911faa5e60f7431161bd445c4da7049c9b296086d60e043e1adf483ce1e" },
                { "fa", "3c56d054bc732f794af5533ef56e7c73728b5804878808bc668cbbe565eac049e72de9fbd9e0b6ee05ed17a53c6a23af11a471f45a65b933b2949c9f9dc5e0e0" },
                { "ff", "9d028d5c435b1701ebc8edd5875e1fe71579b3b5fe2921a6e42aa61db434b87260945c70d0385a19804b7ad63e97fc2fa9f6d496da60f92f3f78bdab69357601" },
                { "fi", "73cb7adf3adaa7a3cdf4a05955398639a317fc76b9aa2b5741646db2b4c1c0681f1650f14085bbc1ff60936ca29d8f73e49c087d572c2eb50de74d6d9ae32a58" },
                { "fr", "ed87f8d11a9750e464921efd5d08e59949d1902eb1f056a515274244916ffd77d21d4f97e7a6d5f61a72d57cb18720acb2c1a90304fd9cdf23c71e3d729df44f" },
                { "fur", "44bcfa05aed6ea9e4906a4e753afebdc6d19c66156248c2db896677b85c5f1359f5217eae8f2945a637ba7a9e915f8ff95d294fa7cb69557f21e88de3e386f53" },
                { "fy-NL", "a138fad5ca29c85d97d178476b18fafa02e5d24aeeeda2452ca6f746c76cb240ab9675524eee5add86bab9404ca9a39deb405faa2966f43be478f0e71617d7c2" },
                { "ga-IE", "0c2937057795646bc51dcf56310c33c9724b84bd1953c65e4834b59e3f4894fe3a0cf2fa8b76dbd7b96d0305c040d7a1a5b1e7afec3202c6ad0080f486be2c3e" },
                { "gd", "65c5c00d88992329c222ba5cc6a90b78549867c624247da2b09143ce74dd7a6d17898a500811491df7b84ecf65ea1fb7ba2de106bf7c5ae588f2d37339d8ab42" },
                { "gl", "8214b6e79e986e089011b41a6aed0baed19666b7dfa57a2b6a89cb4519b833caf956be6e4e3bb6d53d50de4b2d57da57804e996f7d8021321ca12934a1cc1a89" },
                { "gn", "86c7e8e4950408d5d1a014b5c05114a474380b668502fed492f501b46bf7099a083fd5e1ba3c05812086bf5d32315c448db0bd5ca21d36f92c561c499997a1d9" },
                { "gu-IN", "68d4636cd05ee2e5df2ad491b019b74625d90694d13c5710bf57e3075156b1c5d4671f9f09d1ff43392b19c7feb137db3cc0a4cce31794d05ab24c78da35eb80" },
                { "he", "544f39e6638e477b06840c4b0a020842626a4088c9737bd4a8a4e21a8a2449ad91034c1c337509869887b17bb3e41c4d6be1718b0496ca5929127b4a7720c53e" },
                { "hi-IN", "cfc2a1d18d9416efb2f9633859146301e86fa074714be45d37b50d8834e427dce28da265060b79c5bf935d18b312f5b9366550b9251c74d0b15cb51d693380f2" },
                { "hr", "ce260576339c9419e6652d6bb0c20900288913ebfe06fdbf4c0f39e4d30e1a94192a99d2e44074db6e9f7f27de9a3c9cb86982610e2708d42e7ad715c80c4440" },
                { "hsb", "e37d72a7b91a5655919b6faa3815bc303a9a2e9f6d8483495dcf33a553e38e21d7caa17eb67cef28afd5bb6991906e9bd861cd0e22a099844e9c1a1cbc3942c4" },
                { "hu", "ce4a87fcec33e9cf1d0d48f09cf724a1322943309b3237f384e01a29ac9d444670e4afa16ab15b1be1037b2b09b385726a9462d522ad7785481b2a10f2ba7cdb" },
                { "hy-AM", "81eb0e267bc2c221e496c078937ca10c9d5723fd106f2403fcf6925c440ac38d91b508c1306c33343f64d42425210d3f786988afd712af77a982f35c43d188c4" },
                { "ia", "af82cfd6cb6e87ddda24ba39d932dfa808f656570f9e6f2a63c963e0f434a13995d991071e38f6b24484c68a2798a8406749acf75e7e8f0434ba8f9083999f7c" },
                { "id", "cbcf64ff923675a7c17215367e8314462ee7fbe7b887bb7ee6cf0446d9f2b08e9aba0d06abe148073ea59ce2aae944f9fd01009884846fce919f8b5723610378" },
                { "is", "c80e2374d7ae3c4b5aade0d469870bc4b8906251201f6d3c5d568c3953a20ea30ef89b367db01244da48573eb5bb7dedc49acecf3f036ac810305fcda3f34fbe" },
                { "it", "1cdedca02ce9a8658dba673de365c0d6cb0e764a0c598a293c0e2c1851b911340802112e7a99a637a1c59dab915369fee76336fa8dccb12cf50b6a7e0741fb62" },
                { "ja", "521922f53de23a6765d425bc52f0e8ce04847e9e901b0d0e864bb4a76ec2a8c323815e2853d47cfe076abc3171e972a6471c32bccc3812c382613c11ecdda6a7" },
                { "ka", "8938b1855ed71a0ba4fb33eb78fe0893a6b4dca0f9acd66bc85ce359e323d5d9e53a0ebc21350ec241f0e81d819908ea6330fb422d5cd5b058fd18d95290e811" },
                { "kab", "3a3979cff306186615e8fa704a512d02d8fe23366b7aea29691891e7b061ebc2d0947122547acbd3d60add6501bc0d2be3ef24d8aa56c42b01fe2ce2fc9f508b" },
                { "kk", "5bbdbb785b9e252900a5fa0a47ffa32e7841cc2e551e2b01f65f7de36a29472de523581b0acec90b6bf1583820f1462ef2dba77c4eea5e200e6d8f1fa642a1bc" },
                { "km", "5b9d15f83146ac9a3de89d71df97f4b8fa72012c98ae899921e18455613719c8fe1e570dff60416a65fd76512900050ca830474fbdb2a45a5fe5411c9207e4ef" },
                { "kn", "4899c90e8340e8ce96b7c1230f52dd6684e2601401136831d70c8c9bdd2d39567b1114c1b140c43769b60eb2db5e219f395c2f02c302296c2381738608f60996" },
                { "ko", "fa6060c48a95787037442b3123b74ef7fd88e4b74b631b5af00ca7b3154b0bc1f7465a9a1a91be161649ecafcc5bf509865380f697f442239f1c8bd54c35b6a8" },
                { "lij", "a62a5f4c029459f3473d0ee144e3a4bb3f172d7d5fd54c1cb0f287461a4c4a594db7596a262797c36d1d07ec19c98e140c27475da67ee74c393d8bf553585d50" },
                { "lt", "0d616fe968740a3c6c2d0e7462869e0a79c7c7c95dbc976faafc97034607f4b9b26157c2b9a20722e9aa5caf4e74adeb7a0609f5e7d1b65217dff90cf8c544bf" },
                { "lv", "539f2749c9bcb12488a074671dedd59bb550a5d72bd2f79895e702028b1a662f40b5b45b5992cbd71f7ddf2297defd567c1139593587a848b64ef2f20651a762" },
                { "mk", "a7611cf25a380fbe3e9481058b139a29620b034c98a42f3f426c82ddeeb958f7905b63a910cb1e802cc10df3db58012ecf4675ba29605e5ef7d4a167de0cc3c7" },
                { "mr", "50bd2247e726e3836fe27e0698f1fbfd36fa78782e641058e853e0a89c09d582944cf5b31e1414ff09eed97e47ae79526b1ac24aceedbc6a44522f74b4a38498" },
                { "ms", "1892bf1eb746f873866a2ab85e0db2e450cd611cca5709afe358633d33eb0323fe007b25534356a4bc949cef85f921088daadea9d59abd56893a58c355b18dcc" },
                { "my", "a29d3660cca853a131fb4f62837bc5d4b36a015d8be7889b130903579e8c514f28aab53440e5cb9176b72871f702deed74afdced70f3905c3c1b81172de83fe1" },
                { "nb-NO", "69d76083b3a53ad76615145a4b72e5128a7f68ca67c509382da480f9f67a7a809d32a052e1c9306dc6dc0ba2f94e1ffd7ed8683e2c7c79e8d636fb1fd6436502" },
                { "ne-NP", "40904ab1af2f5b5988e2437cff4ef7edee49dc884c3446485396c1013005def10ca1fdb382477149d1f49485263f0487355442d4c64ac474011005af03217b9c" },
                { "nl", "b386a70fdf8d73846f61ad56554b9289cb08567293ba97481174335df506592f9abc33caa626cf450ba58954867e91923c39c837b67698adbff1fafde56e28ce" },
                { "nn-NO", "b839423cff40e0615cc408511320cd72cc9320503eaa3121b5f144a4d5d523beab39231924132d0672cc0e5ff0dd6a411ad1a7274ec60cca727899809a18ef0c" },
                { "oc", "63a9ccd5256b7edad6a800648e5b14ae4060ee34867f94da9270e3ebb9c373e92c200718a0790690e1422aa9051877adac207fd9c19709b3f5e00d81584f9109" },
                { "pa-IN", "e6b2b7eb874ac4a218b7ddb84d3dd8cf46d4864b6c41f99087b9d30e0dc13deadd0cffcd96b9edcc460fdd8170b8c077abd189c035e32e3a33e5bf773cb8aad2" },
                { "pl", "723667a9d4adc5503066272b072820572c38e51936f8c18fee56aacd4e0e7d59423df0f191b0e2c1cb800012eb27aa57a288f0780ca2fa66e6b7749d368e785b" },
                { "pt-BR", "f19903c350f6369b6454aa644f1426581203613c1778afc6052234d08a0f6d61cbf1281a349df317437e63decb157b24da685ab15a54d0c501a4e93923c748f5" },
                { "pt-PT", "1ebbeaf2b3a37947274fb4da6193ef3cac1ea68374b347c8249d3718644c04d592713cc3cc286d6b301478ff8fe0a315767254117dc298a1560b7e61e640f936" },
                { "rm", "f1371105ca67629bf5309a67604150dd8af5f69fd46a11a87e442265de3f7c0e3fbe3b0cb284f161d8add2b71343e87e4d8e0f317b034703275269f7465e7637" },
                { "ro", "e7cc5bd71b1470f7a5455076284412b19fe6fffde9328949c9156a01503483bd0092a40a977887f9f7b2461e6f6968b8df781c80096e64a39a42df737ffc56e6" },
                { "ru", "9ead4cb9cdc7b8e50565fa87e4fe3215af1943b8c7b849e4f272624d00592ca2e9a7bdfaf40274012092bc9736ed41b36e507b3e15435a988431c224a0a19dc7" },
                { "sat", "7ca5ea1f197da1a44f7ba9435b6ac07694ffaff4f4cb3bed77895e6c0be35a3d4cc8eacdc4fa4f2a4a088c8fcf6317c2fc9ca546b23b263f8b82ef3bc1b9e355" },
                { "sc", "2d9d1005e35c22707983f7461e832e9357083aeca0e6d2a80f7251b6e30395edbcc7045f3b9babbcfd0326aba7bb3e9ca0c23cbb77982d307b3e19d665407745" },
                { "sco", "046223dd915e5dfb719aa5630d1b1293cb210e259f1925521372f9984114619da6661b80b5bec07fd83fe1498f78b6bc3b3b886935c47b5c48aa73eaf70509a1" },
                { "si", "5ddc56275d4596627458b77c1c375f360642f876b12723a7a0af068c7c186f920d42530a891eb6bd74cded9bbd5244dbe09a11bfddd8382e85663f3ebb159225" },
                { "sk", "bd3344c6b021e08aa9da2ec7bee509072012852ade596ae93061fdfd6fd9a6396f051902fde2cafff96c60a26aaf9aa5cc696ea11623cb59bb1a96e71493a11d" },
                { "sl", "3cfd39c23fbcf07615953bfef95be69d1b669f618c5168040e95672138c01f05dba87974e58673ba46fe20a6e0862036548619ca372accde77650cf6dc537a5f" },
                { "son", "6df2737af22d340a58ca4edf8d4d8134c40b987843d714f2555366a70d4996d2b6054eef4916625bb05805ca42ccf289b4ebc6bb72226eff1852e9db0748d967" },
                { "sq", "915f4ebbcb9eef9098225843f52f1dfb4c3513164e6a471c036e8dbf48409ee43d9796a9941d80d20227e1c8ac4c22e07676aaf95d0c787ede23a848c6006fc3" },
                { "sr", "a9d7303c1d204463efc1426ad56d8b6cb2b0268469b60c53e170f2216266538fb4e94f6fb5a5d7563175238568a02a3421e70c5f078be5cab2bb2242cd1bd543" },
                { "sv-SE", "678bc43ff1974bf048b1f73a0a53553e4d98af8a12327dce2688c20693580b898ec0a5940b324d25194b93d706f9bff78a69399d2af9025aef93aebaabe34c7a" },
                { "szl", "2f9e1a90170826555235d6b8ff1edb5ea0125aad669e1705dca0aed189d132eabdbfa2ce5c37b4b65e98470c9ce66f642e0480bdcdc04d590ff4aa5c7f71896c" },
                { "ta", "a5d86d149ded32de31b954aae2c30b0832c27a6a86aab09110380545803e129742b88c98ddf6ea30205af543fed16fbecd86168d08ead8f1c3eec70b39b1f2f2" },
                { "te", "feab1be8131072a0bcfdc9990793fdab7f21f88eac17b88ca46f7aad74ff0d4e3cd2c5a7281d55207126fe8bf6718158a611331189eb871a479c80619d4ee294" },
                { "tg", "5aa7ee4044515181f80a30cb81eacb2b14b7e82327267cd35b3a6d7bc3ad9785e9ee4e7e2ef149bb69d4b3d093b01450fa81c1d62cd1ce517e13ad9d04175fb3" },
                { "th", "d8d519431b880ba48ec8985223713235444aa5a9e3e9d07760fdc12173331172b069eac8ab49022cbb8bac84a17013cf2d381a132ed51ef61f0750c179f6c550" },
                { "tl", "b67211e14480a3b7a5daebdf3d55225d076eb10c650fee4a934f333c669d4fb487051397664953a0ab96c05c768ae99c9930fc53f82808831d7b70c68a735b48" },
                { "tr", "0aab63487f9bbf6d8193f6863bd4eaf4d71ef691def2852ca51d96d3930b0ea28e38b78ecaac6c0d2e85c72a268ca9f9ad31b67211362c784c69a0ce8771398b" },
                { "trs", "aea2441781954f9523ac02fe3c3c09dc52889ef61aacfaa252d337854e07dfa0dac9c69b561dd41c9e3a216856997deb20e67c132bb4936cae554c8bd29ba993" },
                { "uk", "760e80a86df33a85a86128d5cf73e48032e6079a508fb50ac057c305ee726154da38a9864cb78144c281a986ae4af3302dbfc265fd604c077696a7b012e75fa0" },
                { "ur", "35288b4a8f8f5d6ea988e5ebe0b44468741e313d27568fd32b6da95e0b8aa818f61608c3e8934a672bd7df599507b9e4376b370e31b76285837b42c6c2860993" },
                { "uz", "72428b0ed16304d93d3cd06c45d23f516aae0bf0da53a76d02e36eea9de484a438a2f267a252378f1710d19018059fdabe5dfbaacc25f4085ad53f489b62915e" },
                { "vi", "37053698b89c989e65f851ac177b522eeb834307593c25d0584a374c8a14b0eab73f257aa64cf61c3cb68b55e71bddde3fed77ec4ce7214e388c9d66c325f429" },
                { "xh", "70c5f1d857df3a4ef9ec904df894c42863fd0c1b675e06fb4f42342626e2865c4aa56964c0a7ee5e4d34868ebcf2cf901c706886bfe20328d205587acc1fe8e4" },
                { "zh-CN", "4fc5ce287387110773e2fd71be41669d1b6c8e27018369dcc3649a2e10d7dedf3e6e4ea4a2bbd8c2910099832fe6319a104041f0678e8b2b5a005e119e639469" },
                { "zh-TW", "ffeaa47f20aa2f0dd36bab998178962e4b60cd5e6092ac010338f3b4f0426e6018dc9b1331303a782e09a476be0c8f76c5f92eea2e5af4a8315f7509a64ef634" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/125.0.3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "a8a5732ce1b6ea6f1c9c79c9305331dc0fb43fdf2b84ae25f3bf92b099b56f4acf55599960ba5e7c000fa96e5496afae80355a8d506bca7276949c3362ada38c" },
                { "af", "4ce0aa2e58059c46135cf03705746c6bc05acd56e6b602f9dedbdcfe5e02919d644156c551ac7dd543376bfe76ca6be35c6e57d06cb5a5d819c1b459c3381003" },
                { "an", "9517e5ca8f0dc2fba598095d1c4dc78e82436eb7d69199d6a9540f4d750bedc4e8b55c1cc80fa507aced04137fe7a4776dc9cbcb70755ebe8f60e9e300dd9de7" },
                { "ar", "048847e5ddcc0b125d70bb408f9d7648ec9e1240979f44bd087944f737595181708d654281a0bb11ae0df0f694da299c422b91e805c6d61cbe98023699c8ed95" },
                { "ast", "374205c5cb510540c3109928b66d3e8827f602f5b2e94994cb216e0c85301b3d136026c16b360d72c335123e8d4a059966c6ce8f117cc3429a580f892d0959e8" },
                { "az", "d2ab53ead140488765aa139dce3b091bd0718ad5eea7dd4dacef29a8da0d0d34ca40d535b3e146a44138d7b2d7eae5660bacc81e6eaa1d3026065d9c301e9dc0" },
                { "be", "568e5cae4d29513eb7ca5692fbddecce1be8701ed6733482c10ca4cd0ccaaa6d5f3d27f00179cc69bcfd5c8b7b00d1b3cf6f08907eb9c4237744979e9cf08071" },
                { "bg", "88bebbefad7ea0862a4841bd9cc8f4215910bd2dac522d5fc7276bfd9dee2ecd1e963e2dd849c3eeb18e5ae143ffdf1997623fdf1f00bab57cbe72ba59ff7249" },
                { "bn", "c92c6a25171c952d953813e699cee22821c80ff42bd2a1fe6f50aa04329286b6d6a4e7f430896e9d76c3c43475de7bf4a67fc7322fd1b001b4b04d16344655f1" },
                { "br", "441ff447db0f7740588ba62c59c9d25b3af2b81c644545b9fa2ca01b39dae488566a3e1859454749b0a9527e53172ec5b4ab4a9c5f858db4fec8e97d5d6e65ef" },
                { "bs", "fa92153c5c45b473d7b1376068166f9d0f52d8b738d093889db02c1322bf697d602e3da5c2ddddc1a265b743fdaaf7c10951c14b20471f6765dd59270fb94600" },
                { "ca", "488c6881a7ac8d140f4a97957a036643706c127506be14fd4bf569be1ffaadc24fd435af2906f7e1214d3fe57a27de37c176d80e2231608506d785ce53349e0e" },
                { "cak", "b2ed842dcf44c26b1c5c5e935e73eca71e815ee0d444538f77889ed1582b61b1f547325dafd0450cdadff8809203510c206dc92776e0331178d50cb67df1d230" },
                { "cs", "a1e3e5ffa30ddc4ab51588163c1ead4ef2662aa4b9569ee03951553534c11ba571f7e9d494deae5b22c76bca2b2755a102da22f2b1fc792b21c0c5619fc7d263" },
                { "cy", "2eaa179054f6237c135ea4224bcc036966f64307f29e2bad560e6df3429a64fa54e89750b2709186f283ed634f3ec804a8b3e4e1abb7fd6a0606ff0ffc4430c1" },
                { "da", "8ce0454d1724f48c81ccad8e758d803eb0848eadf4d46082e7b6c29f63f37faae452823f682ea6e52a039caeeef0e4a47a77363e85a7328c57cb3b2b1d071840" },
                { "de", "62db6e1ac1d7853caa90008759a59c74fce4e8b01a134747917ca999ed189d40f5da5f6bf930707c22baa6e7f244e3d373ab0de80629ade52e6b7f8151dfe2fc" },
                { "dsb", "6b42787bd70d19211237eef1302db9b92f73554b25cf4029243147b2e7faf47912d2f3f95022abd1290f22806282da00dec67af02feaa3ee60a2a4a9a4f0530f" },
                { "el", "f60025816c0e56111c14de4f87d25b67314e32396caa1f21218b56c66861200c90dd7a030f876668cdddf83ac5e7c8a737329ede28bf78305128789bb55f43d9" },
                { "en-CA", "22702f2b122d69c2f75c5f59bc14010079a54691a7e5ef0940529bc81527d55a9e1b800708257b4131bc7c8fadb1511485d7be148bb1afd054b8e8463e9ef04f" },
                { "en-GB", "f8b60f6d709dd32206b6b71eac385e88d55127c873539dc062118c588ad9f7e87bf5eb1d78f8afad424300c65a631788aa19b786789a7d7f1c67b13352fc10ad" },
                { "en-US", "fb81d46155069e4a2d58e14ad8165a2b75a2fb8cafbd7d7d014081b6a6085194f3d8821fff09b0b053f48a9adae65a02b6feba5d83d1eb44ba5a1981cff3487f" },
                { "eo", "e53e79d402f0c7176aed2231f06f346b44822a074b5efe8aa539fd5283f35fc1dd5e265e338b04809be9fd3aa6c0f4b7aa9ec1147b736e4333c94158d8da7dea" },
                { "es-AR", "01e565b460f184cee8707382f01ccf9c1daa39a64c477233f3d6b9a74cb1cf3b16d8da5bfed70fc8f747bd557436c5402731af4b1fc7ccc66aa47b149914e695" },
                { "es-CL", "77a14a58fc85eaff44505035b9711714ddf00389a48f2bdb97572e2810de707c7a07c8e81f3df1ed44ee0d6996037b996c3f8c217f5372b9eb75d2a61a72a848" },
                { "es-ES", "a1a6bc52bf0e8ab5c9a69e5fe7f30d65155a57fe481f5be68f15c863223407e134c5037f09930a863be9d3392f95097aa5478db8b1ded234431b003b06949574" },
                { "es-MX", "bcd0fd09b6a8bd67db514f7591d49c92a09dce03ab5ce075773c81776f2795a7b96c231f1e8497413e0694f732367adeed8dba78c90709e5dbf2c0776ca8979f" },
                { "et", "ecb0946990c427fb3fb131032d277f74d1434a2ccbd12a09f5d196e01d1950fccd4cf20a6d72dd0192d7c2396628af4b58410130cb535ac1e5aec33221236170" },
                { "eu", "1429315f8ea4220d5d1361722b85a41283dc28f8e0dc6943a235fb7fe9a8040fa400cbc87c048a96c3a9e912db4b2a917cbda72ab13f8fcbdbbdc82dae1a4e03" },
                { "fa", "b75caca6a2486bcd2c7a12ebd292ceb55dcafe5b57e622d17407d44b5e1b77a91250ea6811e5736d6ddf1af5fb026303de41d767d0cbfd516d1413bd271a0fec" },
                { "ff", "782cfaf974f0c064defccc73512a1ac283bf51395d6cc9216bc3b25bab76969408c56f024096d587532697f2f8e126f86f4211da9c9194ee106cb21c9cfeb180" },
                { "fi", "957fdb4134ca6d7e8e41ea038c6c2e4f93f60c3b749b419ba842613f098dd5ffcb6fbcbc9253d2bced450154160ba5e7c1f0d565724faeb8400042897f6d4299" },
                { "fr", "2c901bea43641cfeeb8fe47cd80dded431f77ce991443d23b0b076a3327d7c5f51202b582a34d273c3ee0e95ddfe91391eb743ee235f52bb54121b5744963bde" },
                { "fur", "20187b8aaac60ce11c4f9e031059bf912f9fd4959cf08756c963ebc00a19d3b0f1a4d01f5dc55c7cf6f95d8c4cfca4234902bbbac8fcdf1be410e3ae543a0237" },
                { "fy-NL", "0a9405cb835b66a273ca2d5810ad0fe6676420a8a04b435a7a209e24e4092c5171ef32b05d12d5b7f806ab20762b39d92e025ceae4217b54f404446ecebda9bc" },
                { "ga-IE", "79a79874c0786df18f2968ecc9404e889b9b28bb2f6b385fa0c8ba3404039c75d9595b5fdecc14ec3f44403bf81a13c3b1ec92d1190c4d2f581e5ec13fcba4db" },
                { "gd", "8828efff62747a4b2a97fd2bea9bdc7e8ae61c0bf0b9b9a534cf1b5d095bf272a43da80f424577e75c26ee415b1fc3ee059614aaf924d1b4e621523ec356d7ff" },
                { "gl", "36378e1ba9314eb4112ecbb2360e6698aa9b73b63a8ecf18c3ad713876aab04e2c8cb2defb859dadc165cbb7450944f7980e13605575748ef989862849de5d0a" },
                { "gn", "578dfc8d95f65994d3d390d2ca35a1cfeb3eb723ab2d2ec14a70130c95aede7298ec8ee5fa645b0b3f6ede9acca176d117da384156187841c36f468cb7ae667d" },
                { "gu-IN", "271994a2ece1e19f8b8bf2f258bfb5a4b3c7ff8aa4dbab5efa9ff80824938d3f855a7b763dac17a5df70286d40fa859bd98f5b929f52b4d2125518c818a23e17" },
                { "he", "fabdc921ec9c438c8722c79f1858b988cbdf382c593ad05c13cd737f2b0bb02eef94acb4b3bc9b71b1a0f31d84dec3970e5c482d5de9a2812f0992a95ea9ba81" },
                { "hi-IN", "475843b0090d8bc8751462f6b311da17651e3d52ef029f088d6742b9bacbcd20c28b12c2d6d950ed990189ff9a20a429b253939e14cd32dfe8e5540690212078" },
                { "hr", "55c395bf875eb718f845fd7083bb55105a163be77779c2693655508773b79a2f734cf73e7bf44947c695bc4b5045e5f6770c6ac3ae78abdf51ec87959d02abb1" },
                { "hsb", "124e7c557a2920b7006592ff4b63d8815dbfcc99b9e07a66bc750a098629ff692e6cedc758cb109daecbea6ffb9522450016b1da21489f431638c395156b8f88" },
                { "hu", "da0460e94a58fc0d25bbebc719e003e972e8963b676ccd0cd827512dd5b72256e3c7543a16fd4f3579c542883929f27c111ff81ba6a2f7ecea4bda707fa68d50" },
                { "hy-AM", "7faadb794b1a810f17216505d7ab107e15ea4108322dbf5bba291fa334da4665b6e845d6a3e770b1e7add8fc73acfe6ffa5822cf95bd4f19cf4e92a3d5fff457" },
                { "ia", "520c4864f97b6a2285aa9edfc4b1e4041de18e6c4a6d2644014eadb79c31f352a59f312c2583bef587e8010b617957d81daa01cd74d1f1abeec42986037f765e" },
                { "id", "2794ce508c6586e316f1fb2a81ecaceb8e3df2b4c5264ac8b9012d7b1c35b6ec6c993dc780fb3135581dbbcc829abf5a2eb4b6a2a53dd7438372851521462d12" },
                { "is", "4fb3fb671dc116598a5d258a23a110278ee3dc855b4c21ef9c8d0d3df0c3cefde57bdc0865d0f0eb4a85fd0553814e84a7c8adabd3a6405804c42ee3a42b903d" },
                { "it", "7bd70b92f588afb77610cf7ba6a8bff90e012696c7f738ef253a8d94f4f8e9a845dca4475a7fb9d325229320a3e21745a2453a21b7de4dd392810619e9eb4960" },
                { "ja", "b15c6259d80f78a4ec5e038cdbf02b64b4ac265d7a3df436ce12cd91feb9532d87155f3bd5634b07e3864a49e779c13d00527e532e5414caba81a85a7180eac6" },
                { "ka", "1b581b0f2ad3eca60bc940289919818e1892e0138d7e1d3f6ba3545908bb09926f37eb746129c0c9275bef9b5c433c37f8679a7c7e43a714ac19494affd4f455" },
                { "kab", "fdc9a6d163d281f55169aee84da9fb235f3ad8afd6908b2589d63047859e00455dd680613e28c3ddf535d5153bc2eca0ae015817192ccbc01b5c0673fcc869af" },
                { "kk", "1616e5043a630d6629bdd32e178d3b09cc76dde71cecbe996cf63a97ab659d8e772d7ce9abe7223d5283d101c354ab11c2ee1e48421d308ce7394ae26f514fa6" },
                { "km", "edfdbd298f7d9c3f1bc4c544eb612f240f5ee6b653e0882fd6a86f1edaa5f89c44d57b04f71dc2d03077e057b414ad918bf0303976f7f0bbb62ab1a9e2a833a0" },
                { "kn", "f9b5bc733d6ea94d51c10348b771950ddbcb317ca3462eab2adbb4e2a64664fbef36d3e2827415a48f2daa746b408f1a4879d8aa9846807c25313c60e14d20de" },
                { "ko", "6d017a4d1b48120eeb9d1032b8e725276dbaad13dc50ec3b2eaefd4a18fb53f54b5b08670dc46c013999f2b44b9962b25e67cabc1f4101f3114ccf0a2b8cffd8" },
                { "lij", "3ebf0126f79dfe40af9e67a02418eeaaeb73378be29969f0e231773ef841cb4d9cf7046c40a20e2e6ca49e9e33dc4adb6f0c5559e283b7fea4770ae207dce33b" },
                { "lt", "f03d6c84adcd40735cdf2301a353c3755e3358b4d7efcf05ebd7110a4b70c9db557298606c141879f112b4e8b6efc1b12448da19d7761ce9d5561b962b621044" },
                { "lv", "92468194e9d86aab51045ef20f124f3984a38e4f8ca517b811744876d8447b0a70e7804ccc04c38d412e7899e4b09dea4fb59694818c7534f380e99b6a6ec006" },
                { "mk", "066284d7b4e38fa32efed72e435205fc45d9424660c8c67443d1241f6e526193e3cd6ceb24c027751f09dbeb2c004322c3335efb102b9464ef128fc230e760f9" },
                { "mr", "c5799bf6ffffeb7879706f73637085a3627e6f3bfd5cc45a85d114313641dd7bedfb15ed46ff38358575542a11db2ae21526df80f0ec7af05cdddfccef2673ed" },
                { "ms", "49b1b8798752d3894c812c6c2ac115304787d0ac8f1c323936fa88b67c001b081975492f40c3fce75f6bd0e22206e564f8cf404fe590e7c620f1048c325cd724" },
                { "my", "26dfda2096d46b964ba8aa438c2eaf16595ae4ec8a58c1245cac155a8dca07296f129d07982f984f30080e3828d01f5c26326260c41bd1c0595ddfd74e760676" },
                { "nb-NO", "313e813572dc7a1b7ca601711945d19aec349fd08c6ac15e5ab4781acd9caf50673153400006585fc2b351487198d172658366def84092de47fb55c41c0f62c7" },
                { "ne-NP", "70dc6858e7238188aaa98abe45377c6879a77682b8c2ff02df5e8ed50ce31e66b8dd53201b2693bf66270dbe9fe814572de419bd17198cedb41678a68527eccb" },
                { "nl", "4d949d6b79bc07294fb4863190b7588fee8a9bb3bd8b3195ba751cdb6f0c08cec5d65fb6c79c422727f5596d23f47a4b5381f458b26b9826f1090d1073c57dfd" },
                { "nn-NO", "a6c2c3e734788a876e69ad79957982a321e7a6bcaf4941a7e9657c13c2cef0db11feec4b4a5a178f8f3dbe3a125ff42200ef752875a424b7167a7ac7fcee024c" },
                { "oc", "a75e5ec90374ee74a348a1b7875db57a94ea79bf8f63c6461926bcca38ec32409b631b3f406cd0cb054871b869b319f3dc30a21c05b786de4b4dd6e0ae711944" },
                { "pa-IN", "692599912c037e7251368158b9dd0fb08c21c1c81458782fa9f965550eee7ac934a78ef90871a631934c46e7f941b28418d8265271911637ffbc182751041614" },
                { "pl", "7301ff1c29f53e4426cf282e9c266b0799538b56a3ed15d88015216e4240710e88c5fff10940f97bdb6a2ab213f18e4601af3329e28ea6c9c17f556ac863a2dd" },
                { "pt-BR", "ba02ab9a61393d7b96763772f350192f111db6172afba0ef65e0a5778b8045862abdb3de4ba426eacb245cb6cbf594d69541e98f8bfc3fd637803dd2e3b90d8a" },
                { "pt-PT", "845a79aa25b8ba872f7328c22483c89a3f8bf646ec9154aed580334c6406ad50bdaec0babcdfd45f1124f052191a651aa58c3f4eed019a66bad1046a1fbeb6d7" },
                { "rm", "9976fa68ee5170bb5f6165d13cfccb9d97a328b07239518a4bacb476a3c92946e23f72a3c5b4d07e950cb71f34e2d0ed3f5e4a888948a2981814d1b1d429c26c" },
                { "ro", "d0976470d00a0a25ca233682cf367e416ec022afc0ebacc189da144cbd35e5c46a5c7a7d011cc74333d000fd66c7ef31970aa8b26ac7a6f9deaa00e36d1d1d98" },
                { "ru", "fb2859ed9807ab5352bc41f23fa879f695ce734b2ca44d1e025aa14f97cf053af0e7727c245b7c918cad6122bb6011cf7818802f5d2fc2f1290e3723f711d404" },
                { "sat", "fa690cb3aa334d63609534d7dcfc075e666d3506ed0062fa94e54f22e79058616749da19ac804cba94191e4370deaecc5548e4d62c032314783bb39d7710327b" },
                { "sc", "99a1b1e04a8c3ac4da89dbe6c0630387920e0eb3be077970837812b99ee24606ab9739ba8477c96e87e36b2ba740bcf46fdcba1a7ba46b87a5cb472b2814102f" },
                { "sco", "1b5f11b72542eafd71b15ee7ccbb445fd05bdfdfd57931f3f73a417d58e89f601821b2372717238dd5a0033b5ea74a0f91d1b0cebba1a12fa4fff913f5a10614" },
                { "si", "35acb14ba68b5c8ad1456c4e3967f3d86a3848cdd9b60fc7583fc6a8623fcfb70ddf21e0211cc74a33b935c7977e1e25587050063a944ad1633507a2703e08e5" },
                { "sk", "1175ed4273536a5d23352a5da5c10a14578b5fd2bc38c5d3b9cd56e6ba560f6c2c11516bf33eb5311f27b1a4c102dcf62d839f63ec4433986e2ea7b4ea10fe26" },
                { "sl", "d7496df1ce8dcd9e93089574df9a0cb75a0b45c629ffa4e96541efb4e3c10b650cf3a377a7c9e6283ecf3dab3e6b1ad9e7c66cb783524c438461312d269d198a" },
                { "son", "27bdd8d5518649f89acea3f2f6f32bf464801816e6ca518e99d3318b9c685ec586661bbc097fd3682377b5977518c6ec5a1931daa2d15d584df59016557afb30" },
                { "sq", "41b34c196ee2556efba93c6653d50f0fd9e144b3375c3ee326235aa6bbd3e8550083415c53669317a8096b341d594ad886189021df4726326bca3497a2beb696" },
                { "sr", "663f7219f958de07b768c82a91336c10e50682c3e83f8996a40b0609ef58e688469335b72ec943cdfd7647c02d30074177b6ca98d4168a1d1daa7c5f9ad3dacc" },
                { "sv-SE", "3b25e3f5b5200d52835e986eabd0ca35ba398ffc4b7e4dd67f2676d9e483166236195da1a440d1cb5fb517942e220d875b1a044419a9c433a301c2600162e5ac" },
                { "szl", "342be357167d6e1ce80d6ae705e5b007a06049fbd91221043ba4e1df044ad4510600d2d8ccb3810b30d47ec462ff8925b62fc842d511c64602dbd6646b711032" },
                { "ta", "7308a121a5185535b75bf7564fa9e3f409886d593d48e3af2c22f6e5769b6a47582b71793ad0b9c52d39dc640357b505c55c0fa4af0bd919413c4d4a42bce0ac" },
                { "te", "745ecea422383c22ac5fc8e782fa31310c46029533f9ffd7849042e5d83ccadcf400a5788ff4c8c6215eac3b32d90788e7ce4d42dd22f728b4ce69f3e7fa8ce5" },
                { "tg", "36edd0ab9a2cfeed3cb89c8ccf1a414504c398be72ff224c547332a1592cdfe93fbe5b90d465957c95996b9d93a54e329f74f1e0eaa4af46f2a4a68cf4dd58f9" },
                { "th", "7c43045a445c93e88ab7984ced04e62db753db495667251321ebfafc38137511e2c7bcf0e3a48da490ed1d1a181b85a7ddbc720a1740fe11c0b7dd07863dc7cf" },
                { "tl", "c8afb1279626d23a066155a3b8d6267202c71aa931ad229953fd503963ba12332f9f3f0051bc1cc1b217d845219df4d436caa816cd09c53559c1f7a222d71487" },
                { "tr", "b59e3c6b796414b10b0b11979f89b1ddf5e28fbf8707cb74c3fc244ed0c768e16d306b51c51045b488b195a640725165a207edf095e5a32d09262a749a04c2c9" },
                { "trs", "cc5b2725ba616300462f8a705834e12aa2b092122395a991ddac41e87ba2c54f9ced16b66b3a97e0d614280277d1fa4c2426e0768e35d1788e5df45e4d366ddd" },
                { "uk", "9b4427a35476793be931bcb6ebbcdcb419ff235a89f691118973097b5999d5a8d187fe456fcbeb7656afe345225db7efa8317aa89b38b128fc1976ce7390f30c" },
                { "ur", "31bfdf1f8ac663b60646b131558c616ceff7b24b8a2ba7266fe2a1b568ccb883a6e4dea3aba007e963312ece77030e6b7a107d0e1081b2ac68a856c2b9847c0e" },
                { "uz", "056aee1533976736ca4035665ad5b370814a3c15491ca37f89b105abfd5b98dd0c743c3c1513e23562340cbf8fb80c5a7300d37575794d2ce9893c9b216073f5" },
                { "vi", "52c04c3d779e1038752e6535f088125f7c96ee1b28ef6e2340e4541cb5a0ebd8dd6fd611b45fd402e01cc7392bac0feee8346999a650f75a73bf1468af2b7916" },
                { "xh", "668551528630c4a0ec02d48dc5e720f4674b6b29b74ab198ec6ff195b8cc17b1b3833384e5d7086fc1b08c328e39ff6308641dd5bedaa583f811dd703de62d29" },
                { "zh-CN", "1dee4eea299ab76184c933ef0080d8f5a65ea7de720ef8cc15bc98d83280998fee99c593e5e3d7872b32d543e7140f9f7cb1b944466b1e1cf032ae9e9e8381f8" },
                { "zh-TW", "0ca785759dffaef4d3f2e9b9003890853ad2a6994d477d26729666ce86f249e66c554e0a53062fee825934d31329b1ddc2093f5e526bb0350c5f54b2236a5d72" }
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
            const string knownVersion = "125.0.3";
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
