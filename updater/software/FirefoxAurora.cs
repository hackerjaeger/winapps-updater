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
        private const string currentVersion = "110.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "13825240947db2ecf4a1281491e2e06a65d684dfb6d86cc71a1cd94132300872c754b68313766bb5992ac897083e7063a88122b7cfeb3b613f20f591c4654eef" },
                { "af", "ae08c09ceafd4ae56ca288814bbd036d49812b9d15285c7cb88e90ffc2bbea7218e09df6de81f202d131b8be1c44400f873364a24ec416129be74821c6cf546e" },
                { "an", "d81e05ff01edb8dc30e3ebeec2ecde61283e65bcccb49a5ffb5e196418a0872eed7ec603585acfc14fd84e01e16c09a3111924e7a85ec481746bed4ae62d3544" },
                { "ar", "9c83bdd1ae83390d97783c113b1b2ab792449c474e073c35781b4ffd4e287b761745fdd6cb5a4ce25b194c9acae1207a06bb1685ad9aaa23047c7593f17bea7e" },
                { "ast", "1f2bd41c505d99f19a81485656053a3a1117934b8b7770ef90dbdb96780af49ec086d6813322dd9de324593acb654631c55e468c42678f03938f4e7c60445439" },
                { "az", "440dc9f1ec1d2254a9ccd2548d053785a9149d725722e70084ff347106a57e999d6942698f9473614b09d63954fc1162d76698672b7f2a2e425d4a6c4addcea7" },
                { "be", "a417bbe291bdb4a3919fff8ed003eee7a160cb45c72e0bef9e2d28ccd9aad1d71fe5878b597df756915e14ab669825ad2023f922e0eee2f780221b14087926c0" },
                { "bg", "d2eca8dcb8634d3038234d9d2de9a6febdeef2b80125ab663e4cae0aa26c22a6f9f59868c3b248d4a82bf059345db20680b067bacbe0e5d0f17f8453f199ed30" },
                { "bn", "dd3616432eee24551d53971e5f73276cd79c71cfc51078a7adcb5eff1f8b653dc49131bddcb7d2b03b7df045d88bfb8798f3c8e4301edf791bc8f10d0513c977" },
                { "br", "af63fcd99ab61f823ca61505616884dd1ebcde95cc6e8e158ec7752536ed28c144423a9cd2bb7d59039f125c1d64490f6b35e604c849d535eb2fdcd593591fd1" },
                { "bs", "545edc1df93468a788eaf9d35ea7a9eaf62defdd5aab1f40c0cba8e4f4da3c4be41f7d0679c19d4f75363dae264600b621cc2a8e0f6cc70e680455d8c1f5c58b" },
                { "ca", "4d43b5c7c6b219a663400f8fa8e2d3afabaea0c48226593d12107a8fedbd227d1c3fae7e19297a4429aa98541c0958ce552d2ae38a7cc3e1052b1ec2f71def8c" },
                { "cak", "ef67943c655a19a9f4b89e683fb3c0a5e44ed5a6dc34883600247768de6a5944d692b067a5503c82811778222be6ef7ea8c2c1074b672877993fe9bb2ba42fcd" },
                { "cs", "703dae79e900fb41a94eaac32a3659f3fc8d6f57e949d4bf0face99be9eaa25da2b763eb45e357c496ed7abce4a93d9096cf4658e04a6cefbe92397b20d21479" },
                { "cy", "68b6e26ae1d17294e4bf3d765e03ad21ebe1a140bd8f29b28989575a2fa2dcc373fde9cc1adabc1f387c9412b20859e4d5d22bd80ddac00b2ad7ba728f3e63c1" },
                { "da", "07346a67cd13ac1ed7d8fcfed6ee510d2117f8b65cb6500401e5fca7e7dc49a0ecbda8d51717a49b69cf63a022947c2b65db4bf4d46bab7f99ba7a6f15fac967" },
                { "de", "6d52fc15caf4bdd7c41c01ebf69db1f6856f73d58ecbf53d8b6f1b3e6d971c0968f731efc7e8dd4fcf81e49fff6c1a00d29a03b319b3c9cba464a6089ea1606d" },
                { "dsb", "1af773f434865fb249969dc322b9f02299f2ce67e9fd8a9e712c1e81d9e375dc98f70cfa047754e5c29b91812f1dfbd2c5ba4940b7f26d70c1315c5e8126cc6a" },
                { "el", "8b66e2d435b3b3a74d0316ccb3e152dfe6c8d40f71ecbf35396138ad7a1cafbc00cabf7200c12d66ee26f99d7f65f1833b8c1db14ce85f341b45a4209ce640c4" },
                { "en-CA", "cf8bd8ba5daccac749041f57fed2ba7ff53fed2df1dba1d327387a9dcb1f65c109210f1bf9c75afaf63292cb99ff284ea429d7aac4c24509be4e178e3ca85b6b" },
                { "en-GB", "9bdac103aac1630a7c153cf1cf4445cefaddb8981d633f6d14a93f2d28d0b258ff944e1450fd3425967c88434ae89d11a46c431c17ca3aab9119c9a33652c786" },
                { "en-US", "3d1faa85c087b453d923c3ba7d1821b32f26993807a13921873070b0453ebedcf0b65c31ec149a1800efb070d1490ceddd8367428e575de7dd1bc58c5460139e" },
                { "eo", "8b45c950a5fd50cd7d6ddde42489a4aa0b087a6dd095117de62fda0ab8f0e9c00be7e7ab2c21430512a835c6ec11cb18a51a2a6a5d204f73b10f65a2eadbc949" },
                { "es-AR", "c2b7598b070dcbc72a406707d13ec0be45688597531896851b9e527615f9c710416930e03f8ff6c78bca0eda92745013d5cd85e0226d00058e455eca83865409" },
                { "es-CL", "d10b3f345b7325d3610d55abf16980cae9ba43075cb1eed7a75a05987f9ae95d24057577472e96c3d180f4d8bd55b7ce97612cd8bee7433ce7978bbffe2e4a62" },
                { "es-ES", "23f677c98aca0279e519522a8980cec1cd88b11423defba0e6eaea8e792fdd8545cc9a5685b35d0b2c954dca4f24c37f304c964803a8775fbea2a9bf6ac2ad4f" },
                { "es-MX", "62dda07636ebfe26c1cf2b8667c6cd5e194672b830534c40090f5ab93f488fc889cdd08d5f6dbdd5c50fa3ad6bd8d343fa6658c1d7ec40e25feea38f75707c72" },
                { "et", "0d74b194d2d289d8d1d1b1fd8f610f1f6f337acacd009dfd98001104da8eb0cf37a6f54d55dcb30c2ff4626ff925176010a63dcec72d89efdca3cc0aefd3514b" },
                { "eu", "fd058bedae2cfde306f85ef200a7ea2c75d3792542c7e382262e4e2124542b445e349295ac35dfebb2467e15a1ec7828d990418d57a9702b6a5614d29cfbd1c5" },
                { "fa", "5a84386ab1b7ec21d1b0cbcca10e2d9554c4b50745e137af25ca53514de7f97b1a264c4fa5f7462893d45e328c637192e449c41c416d1336ca4efd5084f849cd" },
                { "ff", "caf9966231ff3793c233ebe305ff4660fc8ac229b01d68cff4a8cb29abd3ded0c94ab356ba00ebb1f7fe3f7c14afb30b44ab7739f963c301ca8095631f962ab4" },
                { "fi", "cf734a7307f391bc1a4edccacf47b4b9deec6b17b827aba6f20d2032d3cbe51634100c614a1e0ebf578092af75c8cdd426736953c031e31c5a95bd8feda8783d" },
                { "fr", "54ad4c3f2fd1608d4eceb90aace4edd518fc8b6e3de23e18d445a4737ec987e31024a53c629b656c3b974b4ffa621050cc52f2c2aeb78f4e05842e5b6fd38242" },
                { "fy-NL", "d5deae05b972ef9fe5f1dfb1a4bb69e650910bf01f231ed1682578a0fd57f79232228ef9e00f2a304ddff85f0bba5ddb990ca1d1659c71ab907ecf1bcb7fdae0" },
                { "ga-IE", "2f52fa189f74553b198f48d64d93564c8814f903cd5eb2705ca3dfa8e167c15950201462b08ebff4ffb91815a8cb6189b1e43bc3131a15743fa0f6a10db4344d" },
                { "gd", "03ce4466af4debf186d67ac18b294464bd1a2e1d793db5fdde70a9ee531d91779151b7bb7212fbfeb9037764db0d1ca236b3cb69cd8339d537ee9531bc0e4453" },
                { "gl", "5c84bf91e34f7d4d2237d306b0774ecc790a61dab52018142204dbf3f9fc42abc81e1a716ab7c0269ab1f8eebd11cc7657bf0739e030a9412755fec02fabc0df" },
                { "gn", "adce741fa1b874a2faf9f7f90858c4306905ebe03a6d2391e5cea5bceed3a07e6d2037a2bd28b52fe7aa1db15c1067e66a8d5a4b212bbe9b62617dcae87c54fc" },
                { "gu-IN", "7e8c48a25bdcc9cea3c699df76ebf2d35b01cd0108988f95f02d542c5cd9e900f9be7f3d749c29ea47489698576070c4f4d017141badb3ee536b675bd73fa3f4" },
                { "he", "2d7100a22ee99f5323957549fc878cecbe6e6c7c015df62fe277a04457fbcf8ac65a1b7ed00a91162edf10c1e865573e5c161a48fa87b5023e18b50da1edd3fb" },
                { "hi-IN", "ce566c53c6c9e3d97a15da0443b036a88630a4a351cfb15b630b366f39ef7b1f88428deb93782146c202d5d6a9f141f7c83302464c08e59c76b05f432a331e98" },
                { "hr", "c007d510d43c3a819a1f7f97afab84883c08b18f48ad165868bc40e07824749a33eeca1db1fabb6b43e6add0b21e67f8cf5d1cbeb2ab345a3dfe1e3e3da67614" },
                { "hsb", "d83dbb4f14d68200be5e0ae7ea1f4de4c08b45e784105172d332849515f1f0a40638a2419622364d755daf0b0727d80c4a743466612a0baaa8e0579be4f9503e" },
                { "hu", "133f12b5ab4a77dc091ade0e224aa88e8dfaf70c50a16b2a9a6fbbfcc88c6e5551f527130f656fa7ca7b1eddcbc7b0b0043e426d503cd4745d6e8dfd402fa75a" },
                { "hy-AM", "7e11da5f4184c4195cd1408efe163abee517b3193da843fc9109087d63a92c0414d755d231ed414495a4f714fd56d7963b4ffda223362ec20c4c1e9292e10d4e" },
                { "ia", "5f7176a480e1e7ad3c328b92dcb0c81a938befbf7076f3a01fc029d16a9409c050060e43fe226c818293076ca8ab65d674a7843672d56ef14dfdf4bcb782e410" },
                { "id", "9fb13c031676f6e46c03d0de32846a9a4fd0ee14d58ae8213c5328e6459e8b4606451c4d8e46fe6c16c384d7e82aa26f784f2173258d326aad2fcd317c550676" },
                { "is", "98defd45de8eccc3250713417d5a77d31346696c2f5174d1135a8c3e76740357349d28afdfc0833d9cef37e5269ade4fad43d0b228a3e7a5d7794ed9d4cc5df5" },
                { "it", "827500e9c7bc2791ad6dde5a2ad4c1b4b7f1a47dcbf51f671b118622e9be29c2a11fd5777d9f1b0403d31953d1728f022fcee030d39b867d365e5e8bc2a7dec5" },
                { "ja", "04c3bbc60182a1e15c7a7e4f7390823d44c7b2ffdb9eb372a0cfbdeb42cc697502145945718c7eb35fb093d1a6fee052e16918b6100cd5d6a2c0237ce11ac737" },
                { "ka", "a0ddeb73e5931a88168b45a1c70430f3945d870a6a603c245ac1a90f1a809dbb3863440627c2f2433059c4aed5d8abd937fe12e88d35de2a0e88b43b75d633a1" },
                { "kab", "759f1bad6076a9603a50afbe7f174e08b577f8c62f6ae3d87ca0cf35aea1dc186af87bdd0739056d9ebe8605c010a52519c2c18ecca2f17dfc5ba52cafba64ff" },
                { "kk", "745ea1004c393acff9d69cd41aa33419be24e7e7c8e1619efc3053b3dd29445014bd8bfabdb8b0c7221ccf8d6d5e1777d3e747db2f327e44bb2bb3b7cdce4203" },
                { "km", "b1b3529e65dfc3df8848fe221557d4c142a794ed9dc8d8d53a62afc6451b7c778a0d9959a9aad7fdf23e6ff08a96ca2805e808be08aadc301a44c5b3c8931268" },
                { "kn", "92dd4b7c4e06575bbbe9b8355eaa5f7283fbe485b93a86fcba188357e1b8bfba19450d77dd361fa91bc85dee43412afca9ae2f244e567c1f356242cc4c98f4bb" },
                { "ko", "89ef9eefcd8eaa4a8e838d910446573e1178d30c512446056ba9934f04f737c8220b68d69e395c418a6ba8dbe4b8c686eda3623076f5071130f246ab0c93f3b3" },
                { "lij", "8709061298fa0b6194b8d8e4130c5ccc47d80815737cad060113aef12201befe6f2cf56244898819aff7bf95c9a8b0701455b2f2c4cf3b872c698af39627de94" },
                { "lt", "97b9b46125d264d0fc86bb00554d92e98f2f522cccf52adeb435541837747d6f490fb3d4da50723bcc6db7556c353dc49a5865195cf9d42cd2ed0fe7fa49131e" },
                { "lv", "5633a4d42404c19f12cd73bea9dffb6ea78e7fda59b92078f209f1918d6ca7a8f03a4f04d3c5d041ef8d4994b5e9e2359cfd9a367c42f2922b3c84724914a914" },
                { "mk", "fdf2e7e4d492829007622ff84b5cece9092ad547ff9d04f770c788003069178584c7a77572e6f5eabee4e5c4af7d6662e2a24ed4c4299ccc60dde638acddb017" },
                { "mr", "d1234476435d38b84134482eeb5308f51f91b5d48322106372fcc319fa3078db0e2d4695fb99303347cf47aac3504aeb38d6aaf8fa1e8cea282079271df8c045" },
                { "ms", "aa51782ae7cd2d74c50e1012ed82cfb0401ea2853e68d66dae55db3a1a632527dd37b572f2cd3c078437a0ebdebb741bb59039cf63813a9e86e8ea27e50f03a8" },
                { "my", "9b052941fb8e3165f7be0846dd94d2d558f026a66e6fd75c72d4d57d84ce17e6f2938a6ee591c6b253afb06b25e53b10085fb63772ddadd912976558bd436715" },
                { "nb-NO", "51eb1eb13846e21c306c580903cd4dec6a30c60ffcdc01b75f1e524acf180137e276ab9365662ad90edc03079c9ffdc5ff987ea2b545227e4d96079650c7e1de" },
                { "ne-NP", "26d1d5155a8ad4b59d2f0bfe71bf84f7d22653637c02cef0fc898372cf723fbc8430d9ec3db24f4273edaf040837c84a9dab1ea8489963c21cc96acf42e34c29" },
                { "nl", "41b8c529c9bf3e421c4e06f5c0eeae757676ce81fe1728a8c249b52d7f19e2b20f0a3e08cf00432c683e5238dd124bf9f802f85aee53a3d663d8016947e3ec64" },
                { "nn-NO", "7c03f21e02c4a7144c6d9b1b795a0ee6aefed92bef571e33a5e434f4808e45dc33b431c650215b37ba2f16941acb0cc54ece7fc1e9d291c0efe4484a80593e66" },
                { "oc", "c657131aa90a23727a14ed12fccb45e1133685cc6149fcb0201e24c099d8552abd1a530fe3829ceb9c84780bf315d79db8909ed7312020d6d6c617fecdd4844d" },
                { "pa-IN", "6e1a44847767d51791e204a3ec97024325cb6fac724d6d64d5b7e941bf288ce186f99c09c25b20b223ee08f415c574dca233fdaa51bd31c436c57a3b4957047d" },
                { "pl", "528cea5ef3990950344634fed8bc37e941675de80399fe44f32cf0defe76bcbd5962c374a9487d6d3b39433ed3b724a9e6b4d83fb731cb4b589e61cb1a46eee4" },
                { "pt-BR", "b1b106d344894284f0b62c12a6b23f62667ba060f2022fd2fa380c1061eeb8982b7f491395fc84cda8de2e8727588b75029727f193b34d5dde75cb2ccf773a11" },
                { "pt-PT", "cccbb508e8308fbdf7373bcf2381f24391869b65aaba5504834592ff0bc8f5feb00a0bf98366327ac75a4ebd03a461d7fff4c34a25b30bab1520fc07ffc7b3c1" },
                { "rm", "6c39cd4b2a7b539e81eee2b8f6bae9dc976cdd84f8d14a7af9fdbc21840673958e2b6423350a6d9c1bc5afea1a3be3ae688fe9eb2f3283736d49bd6b9ccc6508" },
                { "ro", "0550a49edd179f52011234c1dbf05aadee93e80ab5235af5fad9c73fbb5e3178159fff9833ede169eba25328bf789a550ba3b0e77134e6878a21da187468cccf" },
                { "ru", "b234e64748c2c665ae351a5bef52af760956444ef618b5b939e0aa7f67420a1b061b62dc934fe5424f6b29c3b3c99a5a5eb3571b3adb8e9dd45c0e6add71b7d6" },
                { "sco", "a4edd22a4a7d39b2b9270ff09123cf11412a77c49deb728c53424a275d12c22aacfb8ddbd89d31c3be7e568a692c0b1c980b04a754c26bdd02449c5bfde16449" },
                { "si", "0cf26d843d004d17e17e9c196b013833b4aa7ed768699a87a0e14c0ea98bb167bd778d30426a498dd1002997d8e6a94894b73d63594361a9c25983f6b4d650e3" },
                { "sk", "207f595b3a132b86dfd4c9b76f9b3e25d900400f915ca767bbff6f6142213437635f04d33e92bf234e998eb16a210f2cb273322dee8c2cfcf62fb9f6a2b3dfc5" },
                { "sl", "e04207550b1f4906ad144d2bbc8d344c7c3940fe487b796124c9d4cd148c837ed96e95049747e38416e46a2b79bd15fe95426716abb1c48bcdad79d360c1be6a" },
                { "son", "7e6cdc218cfb5b9a7e582615e33baa59cbeeec4d19464bd4b4fd5d22d850c6d7e92610b3850a5a1d1598ab74994765496e75dbc1bec86adcb7121ff5f9fd6acb" },
                { "sq", "1c7df85168155144ee73ce096352dffd4baa9d00f152c863b061e3dbc8b398f393050d64a4f04791927cdec7c2adbf627576de32b367b2217db4f4c984c5266a" },
                { "sr", "a4e9fb731c719cd40cac57f08a85fb002ef0209f50a9ebb9f2d5fe22d5eb93f6bfa8aa7c70d6d8e327f174bb541aa143a3798f8260f8eb10d257e404d74dc451" },
                { "sv-SE", "492b9060e69bd31bc732c8583f681f702790ec8e5978876de913dec99912144cb071ad4da3432f3cba5740dc68d1e2c6a14a0ae8446ef603cc75f667628d4170" },
                { "szl", "bff5f4f3eb8b8d53bcdeee0a69c7daac90679b702f988379da9c7f7b8d8ccd29024c971a5c4e3a7e940cec5dc8175506d86696ed3ea164b5de50254e6067da4f" },
                { "ta", "5a0d716eef9cdecfe8e28e625cd60baa1d8b23dbb15765ca0c20a549d82735deb716b790ec50aa9c409148277365a14405431963c0d8838ef92cf42a6a3a5f82" },
                { "te", "df7cd7d20c8211ecc5c19f7a4ae381eef9a124eff6dafb984cbb6f2812164fc91070d735bd399aadd63826e02329d6cf14f2ac326474bcbf88de829e03e2df80" },
                { "th", "204fcd877b50959dfe155872a005805e32be1a5d82e8bab0f289a4d763e5046da56d1b8fddcfd17ceadec20adf4741752ee5c2355ac1d24b3413a9a2d8af75d2" },
                { "tl", "d0afa8ec83c7b1bcd7f02ea73c15d23cee5a16fad0acc27c96976c2b4db37039ec0a8a5df41719523ea1a35e8b32d5fde8e6936c4ed5e4cfd7ed83f8cf1e3061" },
                { "tr", "315713b6df1657e893667681a47aa1a373484cbe94bca7cff86e6b78664ae3564bd7b135e0d4ba0a644f6a3389270ad9a040a3cd5510b3f8c0d24ee5e686ef07" },
                { "trs", "361d7ff551ea5a62bff8fceb1f98769f975657ad9a7e52cb06e669f74c02e6128bcf6cd41f6474fb7890959db4924ca683fea5afb0a80f5141cec0b610980961" },
                { "uk", "66c08c05592ad747914e1c5c5064c677cb3eed3d8d3fba23810baac6654a61d582b2f4a0652aae31174f7c93bfe6d2496b5857ebd47a8545b7c12c480d9439c7" },
                { "ur", "3509e9e9b8357a97378e7f75d73acee997640139771160cb0ad5c4fd1fc03b86e2f5ed771e69da594fc339ad513220ef0700b0faf3a780c9bbfcd7c20bd42257" },
                { "uz", "edd6fbde600ae5d08bf535417c0257457b31de47419e5a15d4b3064f0f22c3fc57176600abf3db2e8f506ec19abbb653e5a2ef9fcbfcb2c1bf75a03c0450eecb" },
                { "vi", "806ddb5446f015f54365550ee99e51ce4311365d81e42fd2a935568020737a2dfd8f006293d1ffbaf0b70087634312de9955baea079dc6d5020469e90bf7206b" },
                { "xh", "7b16a53a73e60d2df5110be46495b751dded69219c39011f65d16c6be88dfce694210b0b0bbab287fe58edebfb8484ecd11b3f730b8b78a0bbc378d3a24507eb" },
                { "zh-CN", "1080676e71eb407f50ee4c21c81273460d447721479215718268a71cac88d66fc42a9bce7a3603fb90612a272857f6effdb27e8998512aebd0883cec0e6a1212" },
                { "zh-TW", "6eba555abe09d2308665105d245717c4578455f8d4e2db4941a6512c15126c92205ac01f5e501b51357a187a9f7548f0f3da7cd85c33d562b15ec1566ac63ef8" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e3633c3a151531f8282726ddf2d444dc9c480c1f235bd6b80cb585dde361a997f8b9705bf4708320f0405c951ec4c91db1c8c51d09a85a146fb79f17159adc84" },
                { "af", "5d695aa1c4577e802d3f8aa16125c9e9196833b07299292f2460818798bda0596c195a3dcd5944b8ed08d4e4983bee060e99d9c8a8bcf0a0f58c525a87c494ea" },
                { "an", "de1fe5ad63da5202bf5d52a814733be2712f57a8dc9e0319a493ab3320792f577e807266256e30da1a186d416bf6a33c949f7db2e5ca9c9b7986172986af7db4" },
                { "ar", "8e9bc60d8512be12d3a024b4967c3582c4305dc16ded270ed948a16f5c89930eb742e96962c3fbb9438af3e36a2331506178cef6cb68c4ae16bc39b3bff031f5" },
                { "ast", "fc2ba2927fd5361df8117048e406b163632bbfaa84bdbc549cf69682afe0fe3b0636908018dd579e3b4c222c62efb00ef44331ed7678074eff4f9e3af86eac9a" },
                { "az", "c57262aadadfdb1a5711175b2864c74125e3590feb6dd9fa363011cfc4b31252fff0c9a60f79aa541f5d8e2d885e846165b0871fe893648a369738219b931da3" },
                { "be", "213d861ec131080c1ae1c537e189cf295fc055176c6f33dbb5fd73e674d0868254b8ed91123e17f460443300c57ebd0ca62b9c6cf4f84a4ff595f3dbe84e4652" },
                { "bg", "4955a8a06a40d26d2d3702f131b24274b273791cfaf3b4abb6d4dff1f20ee607627db3db00b241d48b29ee87c4bb4224db2763d05790f65d389910878197d1e4" },
                { "bn", "f87925431c1a8a4afd32ad58e1b565ae37f2f6e7155d364f4b31bada24b770e74bcdf748bd2347f3fc2618e8cf237b173eeb15e638bc02843e239d92fd522cc6" },
                { "br", "a76c8522237340e4d09abc147a961569e60629ba8ac34e22ab3c1ba1b5ca9f77cc1a0be7cef77ff2c9fa5ad864453529cd47f859a97726a36e350547d9e21005" },
                { "bs", "18a6157e7bf169bb3396d3fab6008697b6a840f7b7babebc1fe6a03cea9a80c9ef4c1cae582d9efdb36f765784f642e2a8dfc872ef44b1198bbf5f95ab425edf" },
                { "ca", "584acc27501ac766614b7846795aae545ba6c68cd278e1799cf77bd654fa451955a31fbce9c74d2e472539621d5c220a8bf42c077d69088caab877b7ed04e15f" },
                { "cak", "be7a6376bfb038f3208af3c8fe1d44d913eaff435d1b370829ab3cefef0a84d30a0b9f800c6a6e10f154cfbc53276961f86a4344cf308f3805ba99c16e852680" },
                { "cs", "d6a5d43a456830203bc1935eff1e54e09c9f6c50a07b3c98946ec0e2dc2006fcb586c76c53ecc09803417c9e370d811c6fcf2217e332d8b36313830c8535889c" },
                { "cy", "7effa0704c430e767bfe4943754b702aa116c4580edc136ac953f51dbe73487be20bc231161961eb2e6a885ddaf54856dfd2159b00dac4229c514dc88d6fb824" },
                { "da", "7364c7e30c98eec283922ee078dc3d12203e20bb6870183e2c7b36b3fa809778573427e4f81d34a8496fcf94210809f4ac8a8767d9b9915b39fd3741092043dc" },
                { "de", "5b7526438e2951054647316b0b5f3fdeed719a23184549774ce83db7683a09427c94d475b99731ddc36e4ad1da6e864944501227f7434bfe81349d32a85dc691" },
                { "dsb", "fda6bc26d041315d93f72cc3b0a70fad1389cc2a835b4673ea93a06447651307356f70ca76dec3ab6545256c1eaa1465f835ac8e17c28a1faa02e379b7ca4035" },
                { "el", "87d0ef7327828f27d2f041503215ba86c0b659f63ee5271ef8f4650cf186f188fc84fd14d7bab83ff1289c9a9f7a73819e1f0ddbef27da74dd399de2c0fb165d" },
                { "en-CA", "7650e8f8076307ca73247e69a0d5f5402cae48bcf541e8460d38bfbc272ecdf63a7703360828006efe9caffa0343adca09a1f46fca576b55ba3370acbec58129" },
                { "en-GB", "3b474a7afb2ae143ca537dbd85f68a08b2d389af17caff73ed64920bb95435a55845c60be0f694655786d5d150f333a574fac347394730464baffaf8e1350d49" },
                { "en-US", "51de981c7a413881152797a7e809d27be2b3bda390bf435d48d816dd3b8bcd24065c66f98a35b868f10a5769291fbbd4980a22a1eb9a9fcdb261977432524f6c" },
                { "eo", "f6a0c2b0982c1b2cabc6818021fcc2996a4a6877d2b6df6d354931830177f5db4ba712d23dd1ff8e9ec411712ff6a687150a8456357986fa51552d0a23f6538e" },
                { "es-AR", "b594722b64d76cdff7994a33fb0f21a4a74633bd6e9fa8decffcb7d95e57c24c293e5c634cec512b47b9bf69b7cb94f10983465b9c66232599f126e04871b028" },
                { "es-CL", "aeef5be7a236720400ecf49d23276be853bddee719faf690d5d806a147294297742e27e7601dffcd065541070d81e06a62582fcad3bcc491c39af89e00a4a834" },
                { "es-ES", "1e23da312f1e488b85a17ad0a0a104f09813f0990591477f9229a4951afe57767e67aa46988d8931d45ddd9a1df5c671fc796982fd46f088605a0dec225ff58f" },
                { "es-MX", "2d9a95644d73eef3953f32cb981546baddb59dd77ba705935e7c27c2a687f5d116bd0f2914e1c2300510d2f3ea6fe5af65825629bf8992ac2ea92febd9d398cd" },
                { "et", "056744c38b57189f26d3e4d18ecd149e968617353d8b975c4de9e3a4842566a95ab1fe0d3dcf5f5e7c71666390f9d59a1514ce06bc5e9071e3d3b33bb4889a39" },
                { "eu", "6757d1ff66e69c5ccf4031923cafbee4e25c05aaf6e74118444334fb04350fabd021a9c0f3f88ccea5594567c684ca39ce9ae1e3e25069fe989d6ce112a6e33e" },
                { "fa", "d6b4ad45c8d781d934d6c074b26a39ba9266b4f1d7cbbfa7e0513273831234bf7741059048a5c467d528b2604ef43d2b228911a3bc184d08a8e334e5ddb6de58" },
                { "ff", "18014da8f696abfa7f87533565ecaec783434dd2a2df6105f4629be70d360161ea20e3615f2a9e30a90bdc6105c75c8480dc6658ce878a80e9bad79a5490a923" },
                { "fi", "e5bf6fe4f86b333e5bdfb17e01e3aaea29ddcd95f4ce3aa224ad8c0dcd58c349e02cb03700287111e685ac2442a4f2d550f455d62cd2b27817e354868d2731db" },
                { "fr", "d04db98f3c4f962c7aecfef4a76e43474974cd5cf52b399f1f923549745dde5aebd9b87451824b1198637ee73cfdc2a7973f3de8d624a65c0f8682736f6809ee" },
                { "fy-NL", "a0925cd94897516420a8d1ea6d8b693e8e65c070161cf6928d73d19c9b6d0ffcb479e01893a67e10c77a248041cdcfeac8d18c1eb16abc587c5d2a6a563569a4" },
                { "ga-IE", "7c058907daeb05f2071492e0f472df2cdf9c26c2645492104c83f3cb850064021d192eb212b577201c7eb141b85a7e30650842b252cdfc1c787df4f31bbe2eec" },
                { "gd", "9951078894aeadcbd0209f8e706f361d7361a1513e9efa0d0b816fb0f49385f231d47ba45dee943567266fc8f574ad5bc0471f269173eef69f3b9009ec3bb884" },
                { "gl", "853b546c760ab1e07d34b8a7025b9b5da5d60763ca45c65b4b4a7f209b1c83b687abd513b620b119c7a95a0b82aeb6263e8af6a4ff7779b3328ff1f7a5c244dd" },
                { "gn", "178d5ea68661a08f3ce29bca993140b992be8f96142309e5a6eca320efdbfb949a87b111c31a4912a23c2b74607bea4ffe2bc4a79168ea2a286932a1f8f347ed" },
                { "gu-IN", "301a6a67536ef4e4cfe643f5166aa832bb7be0d0d182b3c966dd3a52ecb5155ebcff747dd55141c4ca8dae4cc20581919d1073f80940af841830aebc1ab7e430" },
                { "he", "96e00890b6c94e8533957f5c6cf51238d7a794277530668be147c8600fe6a2bf030e9c6a3e1155170c6db5ca4ab057dd2aaddf7c39a61fdbcf4c931a7707c8c2" },
                { "hi-IN", "ef05c12cb929f4694a5b9387e1bc4cf2c3cc3dd118b1dc4160cf982fc0bfb4cf85c1971e23867f137faae4878cb04b11ab55fb5a98c3d2c74d285f2e6e898ae9" },
                { "hr", "b29d58cffda8958ad8e24ea377dc10990391a77bbd7e930316eb296544c67d7af131e9cde39dc7f6e4b22cf096f7f78e9c0630f378c57e772d5688e08886c9c1" },
                { "hsb", "192a63a73e185cb79a01a027c3f4f2c47844f400d43b1f86c90fa682458df332d88ddb9d3ef2adaee81ff0538525205fc646caf1db708c122aacb2cec801b0fd" },
                { "hu", "a6d1f244e6da632dcf7ce1190769ec807c0dd7474520f2f6971e994dcdd7dffa6ff2ba0e5878453a148c5c26d3a8aba1998487e475443e6f63ce7dcd0773e78e" },
                { "hy-AM", "5bef5eba1b555ff9900571c197fb86a76f60117fc0090991dd2ff2220a7a310b629697b2ddd34115cd28b285d66a3f9d99b748cc5e79fd88705e2799ca51666a" },
                { "ia", "15d08bb5fa36a9aac199871efa7deedfe28c072082c32fa9626f548f27102e54e1c722d13079265776b2a32d78e3d046db43c24576d6008f79ca801c09a019e2" },
                { "id", "9a2afd55db32876e83e60c9a33baaaf194c2ab8831c7d97d5488cb5b58b88b98bfffa661b095846527fdb42564c1111e138b73188a3b62bfadfd1e65a222efe9" },
                { "is", "024a5519a95d173e71f9e791da92c4610046c28b5f178e330083db24ff40bd89ea714278533109723d62237015453a5b0f26c5b41b23a3c08c0b42aaab6a6d39" },
                { "it", "5f096d1dd86ca8fed4b606e15bf6b0dd32591f391ffba4c51f87509b220681cc98036cd742db6218766eccc143fa461926ecec70fc847ef6d167df56f23de951" },
                { "ja", "cd9d60fd21f3b8b2339bcc3927d6bbbe775a3bea5c8fa898932bf597594243fe507d2da5c9657614e66142acd38be49877aee3d2097f20953d6195a781bcf303" },
                { "ka", "c283a44eb39cf24edc41138d51cb12a804a3b0468676cd04880de19eee5457a4b82367a52e8a5749f7a6b9e2f643cf891f78c063157af45cec04e4ca82ad028e" },
                { "kab", "fc51165a02297929a35620f1ec7d6fdbd6b1edd797e2b151843c9c6a5217f3438b386eda662226652b1a18890a5feed322e86842a43cc6bc48a84060197100b5" },
                { "kk", "cbe13d352a9089538cd5e4b2049df9f74ace1e4323f8aa616106f120a2a70d735b5cd8aba9ebc887873d311839d08b60d038ab6eb7adf21c3090ae5a01111789" },
                { "km", "d621274cc3f6860eeda1ba91989e802faf420ead8cb2d8283b0df9977bedb62d4994d96b197b443b9a03e453b6b880c1357b08be8a5b7c0601bb06f088e0d2ce" },
                { "kn", "57dc9b190066b75343f8a0872803fb6fb5bb343ffaad083a5b1b7a5b43d608391fe1d7d9f13202f3b508befe677613a0b7ed9bb63ba16219219f6145cc02659e" },
                { "ko", "ca2e4e104fa04e0d2fe182d073fd3c1b1f02bf3a49be23119f86b583930498ddcb4d76bf41c1e64a5a7d72628857cc56131b2bafbae1af9bbf66982ef9639c4c" },
                { "lij", "8d2b007168f33ee07b56a01669950e948f54feaaec8e83a1e63e51ea41e2fe57b18c4c1d7aa90ba2aac77923f4217995bc21171e0e4a3b54c90f562f0e15323f" },
                { "lt", "9e6681191db4b44d6970854b6b92ffe3066f9d6afc5a740e33ca3f6a84ec363b2004e848f48ab550d936aec1e3418d2684cdeda1ac8445043da72c64c8f00824" },
                { "lv", "2a4a5d63ad4a6ce428acf14be76bf4a02d8ccdad494fc47ff78e90adc0817df4fc2f596b433f7cb7c00f8013e994034d39eab97a151bbc2abccdcef1673e7cc5" },
                { "mk", "4d7aa99ce2805c4b09901decd8f9d9bdf66e084cece5b44c70b200d8ba6d995ed2ae6f7173156ad437991e4a523644214024b5823b737b77f80fce7ed28ac9f8" },
                { "mr", "5b05d2d1061dfc1c93464b09d9e50cada47edc40a4923f315e2ce4befc704c40d9ead057e9858eb1c659f52dd1bbe0e48f9f79e9004f478b62c09d204bdcdb0f" },
                { "ms", "667e7fe3347ea0e919de3a807edd7d742af8d4aee0585cb9cd6b4e1d9345cfff959a8f248af0a36323c82131c4ff0c7de51bd6b8e04c7ac8e5378453956b16f5" },
                { "my", "3bc237d5b3035256fa1f9574fdf017d38fb26547f2b8652893fcfa3bc00afa232b15af3b866f4e139180e94cd582186a1c039d90506247c90733b76e51c95c44" },
                { "nb-NO", "cfbfb8598ac5b2a1042c66d0c7b147c73bf193fd51a053d4db33eb2ea5c40772fbf4242ef373ddeebefa83aa353bce544a2e309bda4b3e8cd8ba99953cf1a85b" },
                { "ne-NP", "73cb4bdd12d6db4b8e40395014baac9f95289efeac5e682ef4b3b86433e45c6090a13535549d9665b9027184e531961ae7da81559a7754617abe85805e6207fb" },
                { "nl", "17bdfb2a54cd100a6b00182a4402f4ce9fedd51dfb58dc69501f6dfd556f902a8ccb56e4b36ce3e14f5cbfdc432ad086bc4db64681af9a52c28c360150da7f95" },
                { "nn-NO", "c137bbc78c5d05f51d551c9da69b2adefdf83f83fc37236a5732c659a55b8cc4a4664b3746d1ec7b0aa7858df84a2063a5c60454733e1d56ebabea2f47eab350" },
                { "oc", "fce33a42bbf827ac37f16be8dcce8871aedf3ba80d1c31ecd67869b160c11e09752474462f0c14f7a5cf51e53a34f7469ea2448e3897e9a6bf23604c152dbee7" },
                { "pa-IN", "6fbdb54079775f086a65dc15c9e23b7e9dfeaffb3b884ad8020efa0e0d9f39579880daabc22e23eb18a6ab99e2189e3825f08a8df65bbcfa54a0394513ebbffd" },
                { "pl", "e5eb4b1485b4cc499fdd7980b7352fa3f3fde5d15579152624bae81ef63092077e581c4fdabc607542eb8805d7fc82d7cf22b054838775835b447d4f1c1ca436" },
                { "pt-BR", "4a84c0d3c4a2c0aee1f3c59d348d482355f5f268361d7f0ed4a22203859f8b518f4845947e3a4ff7cd3c5a2ff5dacdb22e5bc6f67d4820641adbd6026a709823" },
                { "pt-PT", "cb648d68e1b3762f9815578d1e09745ee6e2dc87be5e2b8cf61f23721016929fdece262dbb295039cfe2e59d568c5881ccb956e8e46da42ee7747904957d3822" },
                { "rm", "4a8106287b34c462a12e16f287e68fc0fdf750e36b407e414b0bfd1aa09cf45f7abfdcee30610166aa8cc601922884887fbca785a3e308959ecb47043b86db22" },
                { "ro", "61236c8682fad9e6a81a7e4c48febc68379fe51e05578cdb640c1715b710d734b4a0dfb863cbd1c43e885026ea9c6181e2312ae633e82f61a9096364e85639d1" },
                { "ru", "560c21c68f7d944193541c103711218c347f8dd2e100dd2ee52874862ab2bfd64fc51be7f75caf8fa3998b4b77dce1d0bdddf1ef2eed5513d0643c4f9d95adf7" },
                { "sco", "df9fbea498154cf106e51eb7eda934298605b7ac95e3281ed7e915f634ceca8543f179e0bb4c8483376c8162725a9340ff8269e440bea66a7f70cc76095ca7e0" },
                { "si", "b86d080e48b350a413f7fd2c3bae1426921ad68f8afa5cd75405fc39149d114bd7fd15421f4abe1d41316701dee5691b29ad748ed099d8908154e25e567e16c1" },
                { "sk", "37a137726cece6480a4139302f5c99896b5dae3c578f7d7ae8f869bfa3618d035abce1e3f87d6c66cf4d4e12ad6fa50dbc54ca205e7b6ef9f7ea470649bb173e" },
                { "sl", "33f9e88691f3cef3deffc1656ba1c308fd2dfb65b60e494326d381d208786260fee2017072e58a08793a95e3f3416ca16d9ac52c5a1ad6661b380bf750600fc2" },
                { "son", "17d3cd369cc8559476d0932fc0127449b00dfb3117a302ebe1650d3cd5bf39fb45e1014a57f25e400f292e24b5f42c445e5db5f7f68299a2802d3bf39937df8e" },
                { "sq", "26809d2a7b67ee9490f89259ef48cb3b4e22f2c9522a322eef0120013c79809b27ccd9f89166e69bf2036eee91d8f1b675874a3541a72627602b45cab37ade5a" },
                { "sr", "c38816b547f7185d10d7e3cc321c8e286b04cd971074d483dc7a2e8e4243c4cb935357ec267c8f80ad474e4d4d917e80bbe005db42fcd84ad1b98a134049ea2a" },
                { "sv-SE", "d1d5d081c8db68b5784bd100814a5f8aa12f93491a963037020bd3b79c52d9a00a0f78e3d62a573df943177733a99b5a5c57ab15c914b8c9170d482f8718dbf9" },
                { "szl", "bd884cf21b2ebe54ac83ab489aa1259514920f51808ac22cbb655e4daa41b0505f171a829143edf9b2299a3e002b6dd88ac6421f952e761bf32e18a34af0f344" },
                { "ta", "c58a7bdf99cba1cd12f8e002e8e6b1913940332dfdc358b8d84804357cdc110763a393eb5b685eb054d33c23f8dd37ad63a4309132ddf5ab6d435f4b265f6418" },
                { "te", "60fed32d82764109945e3ba2bd6541c24a87014eb3e6902f57a168c6b78595bf69b0d08cb792377c4d1fba0112e34f786b186f9e0898b4f0539a317b503b1024" },
                { "th", "3d1ca2b677559746f46c1d82e6e8bd714def65b59e07c1d0a86a8379356547fcca39028306e79f016a6987474d43750b6c059dd2fc05ef22ea3f261789036057" },
                { "tl", "437c601e4db2388127c6ee46c1b93d3ebb412c2154bb7c7a09181b6ec985c0b178848cbe2cfe8f3ea35633afb3aba4df2d0a66e9f356719b5dd50296717e0676" },
                { "tr", "b181be50a01b3a855ad9a71cc60e48431d16d0107149f74d29db6a332653698c1c9cad781f83770ee3e7441679fc3df96b51f110fb12ef5ab3e2d96012589a1b" },
                { "trs", "e7be7e1f1d04cd705c1da0c1bbc5f15b4a0ce7aa7209ac8eb2db13bfb8ba4e4b41b450749a0cffde342ed4374acce584f5f02ae817e3f69984eb44aee83e8c82" },
                { "uk", "36a65a2d9b00da32b75e9f76242b485f38c69ed91c752bdb315a6ddb8139e719aae235526f73c75bffccab4bf7a004e3e65292f742135d231f2eb80d29192381" },
                { "ur", "89a851690c2e8170cdbc8c09db3306a577ce8a94a072dd3e96e142935892088534a44e252e9678d1e79d043926ff43d5aeec95036c399ad8874e2c09363af7e3" },
                { "uz", "004347a7e4a475a2ba65d0d7e0e2c2f9c3a750c2469f3b278d22b476c23f54c84764a0faf06df868bee8859dda48b356017f22b251c5c6d8796c133cf43860f7" },
                { "vi", "2cf9bf355236dd38ddb2e1164a5dc079b4a1b8d183e2d012fd339632d9c0b3c96e1c3bad13c901be9e037fd8610e3ffb5e159fee7b647dcc3ab45fb058c01cd1" },
                { "xh", "2c2588537d70b80e963c3676c783b80970198c386cba4d29bd2e47677b2dff5e68e2e7e418a05229390e1f3c7a01ef5d2e8663132bc42828c0e6e78e7918706d" },
                { "zh-CN", "5a44c1361c78f8090baedbeb81b335bf2bc31adeb40d2c33fc38e9a19bc35d5abc6f29cb1e66b894c50a065b625db3b9b0aa977e1c84a116dc53488837188ef4" },
                { "zh-TW", "cadac8f9bf85dd656a7a31d43cebd2d70737feb06ce0246ccaa67992e1b8da4ddac0aed981f4f92ce5f081745bb4d209040b69e4a253cc701f9998247f772315" }
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
        public string determineNewestVersion()
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
