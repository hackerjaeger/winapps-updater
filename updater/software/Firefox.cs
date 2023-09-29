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
            // https://ftp.mozilla.org/pub/firefox/releases/118.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "4fefadc5d8d2fd04a25a3571c131ea6d4a96572498a812a1685cf79dc3d4227f92d6273555bdd386c7e7abdc35a8c375e2f19046708c32f540b0e4a826bcbfa5" },
                { "af", "18716300fd80ebc558b260c9184c37d8a9b1a254d77f7786e2f49b8c7e78f669665d0bcf64a171a473318fa890d50f6aa194d6bd7607251c50f910e3be7d86a7" },
                { "an", "bd6da5a0752330f7ef86fbaa1b66e27aa4d4bce97cc3cb7b1d87b9d6659e66563b60cd8551c0808a696b837115084c9fd1a2db5243b26892f9c902f6472f3ea6" },
                { "ar", "1c2e3fbea227ba8a825dcf0749d63278b228c8688d090743422a63e43c84ff7a02c0549251a65c382370f25e07f55e75b40bbc42e3d3984745e27c430377c64b" },
                { "ast", "652a77d1096950ecb98dbd2dcdd7cb451a47c6a17aa27b1c152054f4fd20332e162ab2da1d4b54d12b08f905a3651a714a887c5602ef53ffa7c8b94e0f66273e" },
                { "az", "dab00c8955d5dd8f7c299f7c623f612176f5d41a6423ef5772befff0066bb5d7ed8ffb2db5eca3856d771301149cec62489f9cebbf6042474a77bdab6362ac13" },
                { "be", "126d00987b906bc863aac2da4ce040e7433ebb28f24aac5884b303d5bc12c3830a0b6f6b3e087e30452356883149a69d2746fff9f85f4fab183c60f6548cf886" },
                { "bg", "4fc09f959e8e9c929d5063332a2dfd9750e36fc26befea3987b27616968fa2243c1ea888fd106335db9213a45ff83250afd6a3716dea8fe139c73fccd25d0581" },
                { "bn", "a6a058f2aa3d1e88ec37b8d3725164bbe93fee9d691149ae64bd378532bd5c7d613090f2a5f457c6753df971dacbe5c300b08f035fc91135b59e722ca24e7b02" },
                { "br", "0590f92075e13733803a8e7f6f71f6c976466be0480903706881ae622aca608b119bb36ed70a14c75c2b8d0f5dc5bf4de3959c56e09904d6dc04e54758fd5201" },
                { "bs", "f570732a7920a14973ca48d711b66dc6150bf4e9758bf6cf4f5286fe89674378e04e681c0e6841459896a6b9df24004ab8ae80cf547774371d1f037b89e698dc" },
                { "ca", "794a78df89a86aba210800678acca2a64cf05b82c14d1b735772d2a2482ad7f70c61c8d141527782f16c6daa637f0cca82546f1c9061091ce3bd689e008d71a4" },
                { "cak", "98d3ead9dea31c6cf907dcef82dec7ff4d619dc79f4112b53ba6d9ff593a67b9c548bdb7290e9967d063180611cb76138403bf66a503e1076240c2f7ec3925bd" },
                { "cs", "d3a034f5086f585beb9ee0cb459f5b264eb9c7d8f1c5a6cf99c0f47706cfd76fc08254732ba0ffa9114c5de37d09aefd0fff0d9b5bbd5453df1d49f183cdc93f" },
                { "cy", "69594aa8fa25f5037ff6f920c2eca57be5be3e4737f9fa46b9d29c34060458537dcf539b532374015d2f8bf19f925419d60aa363b00c00147163741f2fecc61d" },
                { "da", "7b445f9f4cae62c0f49507e6ee761439f21c5c156d97b4bd7bbd6b3d7f13ed7088011ed07fe91dc99900585faeb9ee2d891b2784cbd96f1a44c53cecacb511a8" },
                { "de", "62383bcbc7b5404d525afa1d9996e9cd649a98dbe14edf8b928acef7e84d7d863c9e69422f9c3ad07e5d089c20a955cfaa15765f16f4d05d47c2838d45538dba" },
                { "dsb", "e71ad186f19b28738ff1af88e77c19617f7dd835596d9b9654e52a320003a3edd9563d3e38fd7395b6d0416d06d3d22ed2c2e8e6d28853e010582ffa95cfc8ae" },
                { "el", "c850667daed12a9e39a0f35652d35e115d734f560e0624d3dc0dc082a5ae7f1e437502c6815c39bdd872458683b84c40d5cd8281c203d34c84753096787a3ed7" },
                { "en-CA", "4056791d9c6ffeebd67d79a1f3822bebf1ea808e2128f362ed9f0575f26debff25e567e609b8428a9949551ced89f36edfd44adbecaf91c058f3639934b71872" },
                { "en-GB", "ae668a732c4c9046c19a64ffd4a252ca65d3adf0d5c206de9b2860d1760a7655203fc9cf451e1e58bd4a027125833df942e26f648583b3513380fc706d1580ba" },
                { "en-US", "6386e76b57b177229d7fd33bd056493038d3464c4af20eb1f789bfef6a12a6dbd9443fab9a6d227fd2519802fb02c5cd2117d2db83de73ec74f845632af6d7bc" },
                { "eo", "d82c553c4c6d98d7f366d65411bb23ece17d7b00b523d640443ace9a37207de26b817d423f93db1c3bd87865eb7ef08e7586eceef13c43516a990600aeaefb86" },
                { "es-AR", "756905fcc558486279c3edf14a49b079c36064b6ee42adfe4771c1ac9d4db9e73bc245c14e5151d1d74c5b0e6fb4ba4f4e6938c92284d9e9d316f7a25aa1d34a" },
                { "es-CL", "8eb3f0058bfbc25f76622a73feb0964df30e3e8dbc0a36f96d3f40b1850921042707c4665fbb25a71eeb3580bdcd7f1c0200725ed9038969de1e1ab99548b35d" },
                { "es-ES", "2c609e10288e95e9f8f04e3d960776979c5e63d32210f780490db19ccad41993fe4de37b38fbe2e43c0d8b7343633e82dc85fa473b3a95fdb4d57e7bd09c867c" },
                { "es-MX", "7e0bcc76a093dd27129ce6304f1d5fbb53bab62cbcbdbef2461a51cb3116fdd17c2c66e865708139b16658f41c9ffa258ae80f608f1854bc7ba98c6a74203485" },
                { "et", "d8446a59d3c5e43bbef40798828e1aa363efca75a09e117eb93cad4a4dd74f38aee37eb042f0649d0500b66953930b3cfb5648f827eed63f18b02a37a6604298" },
                { "eu", "c0dc36fae057422ee98ae0549a2551b2e7d1f8fbfe592acce7497e93c2a8bd08583d56a964dfb3f006bd0acfc212bf29d684fc10a65851f338fbfc16d19d509e" },
                { "fa", "c5a4ef10f5acc068989e8127c092a7eda366248fb4d293686e4bc78cea7c81b7e18396f4e95866077a577042eacd5b165f0daed894d5bb523215cc1929ef4d6a" },
                { "ff", "3932e782b1d69dd7e0403d8c0a04c1cbb79e6d0e44b5cda915001c14e901e941e0d627d41d923a961dbcc4b0689c827d60dc66d017fd01e4aa469a0e0cd90b1e" },
                { "fi", "e138437e8f39ecd31bfc8dce118be453ba7b27f3f344acbd8f1e416b223c3b1e9c69256645b9592fc33a0f4b798245ab2272507493fa9412355fccb6c31b439e" },
                { "fr", "211272da0b5d949009a41feb6269de0e50596b973e11b164ab6451b64cf299a74445a26ded40a5121fe3455091db92f358d6bbc60211f7e0fc5f90db09c5a90b" },
                { "fur", "62674594bdf4db7d521758e021d8cea647fc663d415961f0574d66455f7d7fa0e5fe5d62b5df8dcfe9843e80b2951285b81fe9baec2051cb6a8861e4df0a4eb2" },
                { "fy-NL", "8579b34d1e5518d2deecc48450f62e465a6988504952989a874ac19b579d9f62cecfa72ee43b47b13cd4ea4f359c1af0cf04b44567905fdd23f54a57196f991d" },
                { "ga-IE", "d6902bb2d16061ff7c952ecfefa2067f6bac5709815b038a294392652dd26aabb3cc93e5008f14328c46bf3d5afbcce60b3391967a6c907b14e172b34b68fc3a" },
                { "gd", "a11331ff0e0e0e4807c772df9255b0d525a13ba94e0e3e696ad7ef348d5bec074321725a691724d6e5621d075cdb51fa73717e3b378d52ef5e22a66a2d7f6814" },
                { "gl", "dff94cf88b4024abdf3d98f636e811bb86f13d90ec40d8563f3eb1b1240d1fda48ba6432adf39573f71642cf3fa62019e3e9e9dc77208b11e63051e7de0a7905" },
                { "gn", "17bead7fbed060df4c04cfdcdfe1aaef44f2bb81438813af6fb8f1e7138e50c4a0247e142e7341eb8f0343dd8f30224011a837ccd26a67e317e92ffe3b5ab56c" },
                { "gu-IN", "ff0d06f4964c16834c32753d395c1c02383a3d8a5f224283a592dcc0691c7307100993e77d26ba347cd43d9f2a6470d5214b9a21d9b54fa80a4c1aff6456df14" },
                { "he", "6826da05caa96fe22110cc52bbaa53696add4ad1c5e2431c3b9caebcdae4411743cf9c0686b33db5c2e3c45bbead32641ca1950eb044e326d7887f853c7474a8" },
                { "hi-IN", "31c9cb248e8c9122affd5e653434c8f26aeaea3e49b65ba52bce20227d14d1d5d94df8c10925fbd199aedd3a6f1ee2add28e4ecc86dd84b4e26430eb3e1af6fe" },
                { "hr", "5132885af406bd558f8abf5d400994c06ba7a72719356eab92ee1ef4a8714e65814b2c536da0cf8e0961da79977d4b45e7e5eff1a451ba6fac2d7deae30f2b20" },
                { "hsb", "0444f717b89c9fee551ce1a689e0809b314e0b7179bb9da0d6e85031fd8ba0237064106775274a117359a5a38d9957cd569094ffddc0d6889afde542b1b267ef" },
                { "hu", "5beb007f6836ba57df4aa8dd53c9d991d834f99a9a68eed20435d7a5e94a5f6fffe3a6a3e59880c5498d013879b3ffdf09f4ca7cea7cb55adc5df31b11597263" },
                { "hy-AM", "205e85341e5ba7a29f8e5e2a8b07893f11871a416327901ec4ed19907d576dfd27e82b63c90aa305e7a99e05c4b542b381b6097a2a35defc0960b0555cd91819" },
                { "ia", "79325896a90bb70df17e716c07a91a88538b39c849f8aef0c306eeaf4597ea60e77e2adcd8071ea1336abab86df4ef1d63ae9682650c38614ef8d6052407f64d" },
                { "id", "fb2c6f2ead5258a1eb992776c0f3399aa550f273fdfb45dca869eaed8a467d70f93276641123deb5369a287a9eb5e4e0e7e0492377616646ffb9f2d663b6b4f5" },
                { "is", "cdb5e74aefcc2a576fc8e46b59020daba58bd7e98b9ee1ba033e0d22dca44cdb58bb5a29d628a9c0fe4dad6b9aeed4e5c7217ef04f5a016656d4e66c10d74bd1" },
                { "it", "e01762d2bf141f5ee05dbe9fbbb36e6dc6ab78235f7f72e1722a8d396151bce5139a72b70430b5dd8c9feaff40fb6032f437992efc3efb393aafd66cf1646abb" },
                { "ja", "5ea1492cf738c433908b62c53aa09a7c6eabf1a679458a90180353ce27b8d5de1fdc4d4393154f4c07fa17c3344f872d798e94da268f8d9f4d5b5e30b4c55f88" },
                { "ka", "c8cf2558166a99c910c1a8e2baccb0c767714b1045e85d7fcbbdaabbdb1c086dbecb22703dc26cd34fe983c5c2d932b50f99ad3cef83a3c9a5310a1137a8a666" },
                { "kab", "0017793b731ba94193143ed1f718643683050811ea5a8b78025ad203f0cb298e60615ac67cf9707c64f8e536ccd3e5adebe40b1ca17cb36978fc143d9ed6ba68" },
                { "kk", "15badad1a5e5ee7ae763c3d267feb29995aa831b3f5513ab675d9e1e6ee48dac6c15289d4c0b74945663f988b5547e6c56e336bdade098687e87e34af8d6f85e" },
                { "km", "3321069ad1f0e556b22b9c82b571674e993a76d7cdd8b230388e097eef34bcbb80af5bd5397918506ae368fb4732648a058e957a65b66593881748d991c3a46c" },
                { "kn", "80d2316ad364534b4329e4da3c5afe00054ffb122c0bf135b528a68890fbb6dd27252eec47fbd664b8fb1a10293d478a228336c5292248e936caf30d6b5b31f1" },
                { "ko", "c9ee2c46d9bdb5c994527d927cb23d655ad35fb21eaf2aa4532852f1f839dfbe20aee62c666c77d807548379937a87c50f07bc6e6fbc0a41ec4e4d2ea64b8fee" },
                { "lij", "36ab2450b6c6bf99ba4640da7e1469b5caf100cd2ecd10d8330c1afad1ca049e97995c35a9149f7e61172fed87267e1828065e501c3ebfb23e9486c61df4db25" },
                { "lt", "cdb939dfbae0c6659e099d4167dab788d2c53e92591c5d15d192e4373d09edb3943f303dc7aab2bf8275ff17ea9cdc8578c6e49b41c1294bb8c5aa648d4a2ce8" },
                { "lv", "93df2a274f4d6909c8a685ad55e0c457d3059900e56292288bc0ccd3406628c0d6cfb6c7d1f0a2071cc4851e8163b5775bc753beb55f786400238fed518d79f1" },
                { "mk", "3b100f19e933683b13da6a042339d96a8a0cf99bd3cc09962ff002613c50b58f1edc7488112ffb98a414dab1837e008fc8547b0f52fc26c8f740fd565d095771" },
                { "mr", "a1f52bcea9820fccc33e0b4d6c623c19fbbb79f2daeec9b5e80b0935b1f9afd5a3d7ba366b4eebb5c2d328458a53f25a2d8bd7ec45c02a4d76f08a669a3c0faf" },
                { "ms", "f3c6584ca594c0a3864db1d5f962b6af4c940b24b840e4a609a8cd9ffb57915fa3e025fb14be8853d0e38ed89191df656d8bf0e9bc9004a42e8f08be4ffacd6d" },
                { "my", "682cc61992604d9940f08b0c3f20282213a9ec95de8a682f3e16eb2e295913e997add3f7d12c61bafa8d6bbc6a765dcf5cda94c7a1f08a6a5189757221d88108" },
                { "nb-NO", "c718f5c3afd28650c55fd77c95db687d8502550fca2baeb55d141f59540ab97b5f94e69dc6b71122d0d79852115fb444ce93002083220de6c14fa0c195a1d47d" },
                { "ne-NP", "92c0637933aac25c6230ca7553ee786869e3e505c8e4891f7a1fbd6a08b5bd983811e51945acbb50c79a74bbd39b00d301856fcc2275378ed8634d5d627e6784" },
                { "nl", "4f139cb6ee9d72a2511266afbd070e74cb1e37581755a23a84b20e59451700dd166ff36dbf9592df8876420a6d220ade027a38b3dc2fdee973be359af4005579" },
                { "nn-NO", "bd94a0eb6b2b8463e196aba0c1950680db7be248b242820c8a3a16b185e5809999631a22b822c137961d1951d3983dd47e7c29e0c6496119a0093e9246ddcb96" },
                { "oc", "3d721efb64bcd3396d7a159d47ed04c4d501dfd8b66ab31351fb23640fc2b2c338c448b1d9fb0a9fc3f4e93a8144406ceeaa060b68d9a022abe2cc8bee695481" },
                { "pa-IN", "9be605f9f831c568ea0f627dae6019c9f4d1df3dd77122b279eea84200d75a137e0316011c39eb73313d035369703ec456ef40cd402a70862705bc4ed68948c3" },
                { "pl", "3f21857b29ecd68e00eed534f6fbbe3bf476ec2b9dcbd57313846f732f731254e02ae963fb86e075de2cccd9dfa4140751e912ce20104590ab437bb6b33195da" },
                { "pt-BR", "bc990bbef53d60f144ef7f0d195cbbc54547f4ee9e08dccddeb1904fb94e1da7b89e1fb30b3388df9e31304aa9d40906f65cd672c1d4bbddb4907c53a9880a5c" },
                { "pt-PT", "daefcd0b7716dd400e6c774681b363f4e722198b41030b6a65ef9158f3d44ed541412cbb4e684609748803a5b8ca9acfdce62dbcf7a79462fc86949cd3dae7a9" },
                { "rm", "db07e887b472ee337a01fcce7b43e0db82c61dd8c30df12ac66a96d14949530abbc5ee10c481345e86c89120b681701b1b321293074bd1cc3ba161425469504f" },
                { "ro", "de98a6a052565e404e7131e501ba897fac1384e98ded7494b5c72203db957649b3a67c1974f51e6d635a5c56366f6ceb769852030052849cfc06eaf04368e46a" },
                { "ru", "158e8ee7a84e059f776e8687369dcd73cf69eaa8be6bea2928173d4fa2699fe64c90255b1f8eb31469cc91d7cab7610b8a0dfb393644c9b04a6705c596e3ed1c" },
                { "sc", "8ac4cb167b53c273b08e633f01d3a4579299c90f146112a70ea5a01ca642e938c2d0ea986592c0d7024b5681b039c71e4e2981159cca7e7e69398e64ae06d7fb" },
                { "sco", "8b04d2bec2206f6ce740b5f87cc1bdedd9be538210bfb556b713d47b406d77f653e80848a9c83e560d1eb6f0c31d9957f424e0df8679ebb2e52a6ed51ca76b24" },
                { "si", "f3f55a249248d8d41137dfdb27d1ea973144bd8590d1ac89647949b8e3d5d74162ee57ccfb72c99b1208574d64b8a4c318cd987a0652a1c3bc46127721758cf9" },
                { "sk", "f94be5f47e23e351927c8748530d11efc6d3be0b03c6fc56d41ac94647ead1f64aad830845142ffca9d235784f96cc9d552c13888e0d0448b50b1368fb269cd6" },
                { "sl", "27bf3f68141a3ce380ee50fcb1fa1c25b83a7c4fdc3c5174e9cdab779a273649268528d5a9f91b3ee03c4c3f232ad4c096e4a2fd8998a0b8116cf269a1989ffb" },
                { "son", "0a7c4952309e2e5acdd16d6375175705a4d8eb90f561a0f534bf911c0650f9e5a6a8019d02b5d14ad5dca1e647a91935edeb79bda57d3728454b1fb32053ff54" },
                { "sq", "46bee6d0459cdf0b554f62e33f4e94ea3111601f21ecd81a3528678988187f46b32d4cf773034e059139faf878df79c0b60126af736fa06763256176436c3efd" },
                { "sr", "9286fe6804ee05e42e54dc914e0fb81eb7b72db98c79cbdb2c6b097611f2f435d5155d5074af2fcf39b7a02eb40c936040fb795fc3b788e5517ada19107dbe1b" },
                { "sv-SE", "f0acba985ad9b6d49360bb2088d9ff2f53670e6c7b832330e9b81f1c92d927aaea555f15e45e4f38d68de92e55874680881bd4070ca12b75f57e21b17b2803b1" },
                { "szl", "343266a5ce76b459e91447169f158db453cadee36e7ceefefb9f3d2a6b0a89a6e0fca7f00017e082dc21a784104365e9a1a59befd1c78d59e111b43cbf797d41" },
                { "ta", "1471eb3659ab7931de4abe6d53857899922d2d532319266398e5cae8dfe408e74b1017811e6af0155d94fa629e94d1f718398f00ac539ce9bf2e6706226be5d2" },
                { "te", "3789e5f80a5860cd09a558610e1effcefd52b740da30577340b5dc3d78a6cf33cac0f13cd227fc1993172716e6677b190dba2030c5d10ee8200945202dff7e44" },
                { "tg", "33382f34dc7143fff17542ebc11dcb4e368f127978cfe3173a81374192d50a262dd736ecfe135d234721c29577d983217aa5176f293221dc4c29a31363be9118" },
                { "th", "05b09a2bf709aa89aff3803ac2b82a19f3002f617de8ceb1f18db7a6bd147a34ffabc136c30d1669a7d5340b38aa232498be48637244bffdbd9f58da1836767d" },
                { "tl", "8bea987f257b953b5a3a13f8de301a42e4783a4bf5c8e926de0fa855cd97c99ca2ac8cb85016fb25f6c1f0ed9016edc1a2f1001a0ec565719e25071cc3795da0" },
                { "tr", "823d152b711fe719abea6527894913e90cf604190a451da760b5b9300dc9932e214ea136b151290003378e7fc56437a1ab7130f76cbf00847d048fbcfbba4682" },
                { "trs", "a83f6311cd03bd5144371c967f78f804aeb3a35cebc12057fb9b95d96cba922b2b7d36cc73a00d929d8d3c5d4f7cdddce343bf7fbf72e2cefdf47c03431862ea" },
                { "uk", "50b2fc8c4286a380419e6a9e78a5e8fc85acfc0e5cae1dab0dd1b63f4819f28929af70d7ae5d77943e568e515b07ee8b883d3d8319161415b331e2dffec52060" },
                { "ur", "7ad456519f8b05e25bd5ffb574a82d4a76f3d865cb0526dc732c7574edb6a90a083e18882c329dbadaf759145c31a94202e570ac8fc91522238cf345f964bd6c" },
                { "uz", "d37b2a7acfd8102167532db5738006017de3e5c6ac816e5c60f9234f35a5b7983846e2a6d2287ba417b99e699d664b2be75e3a3387b18684077681c563a64e65" },
                { "vi", "4224b9a34266380e74bf846c0442b9038a3cc8fe8ff41175ac39acd7e47210ff427d32b1926571d38c4edb9424d5888145a90e3377da5e79555bcbeb1a58692f" },
                { "xh", "9c94686ad2b899f368d2fd95dbbfb343e3038012cd994253922b5e3fb8214ee00ee5a152656eadc06f3dca87c3bbc214473c39ade8a89694ef552824c5ed11f3" },
                { "zh-CN", "1132ebab20348e6ed608fded46272ee134938e86697a05537e17d71ea764413bed5f46ecf830dccbb617eca517bb6b869daef23a500a2ba4c2bd1a4aad3c5490" },
                { "zh-TW", "776cf628faeef10e79295035e5376bfbecc32fe2fd10d68f54556de8ce32343d8bec5b5a7906a9c1ce7f0602533eb849c49d11a497d6757a5b234efe91e6a76b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/118.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "85c59745b449b7beea67551ea757d97946509c4cd2a4ae168943596bd56aca0b87af7b2121c3874f817cea3ef075dcf99210bafc4e14e9dc07a3ee20fec85e43" },
                { "af", "9f110657e383d2a08a26042200243b8abf539e9eff0a50da63a6ef94af28683e7488d9fefd4477b5254fdf36b0e135ecdea29209484430714e3fb76259459b6c" },
                { "an", "1e545ce138b4a7c35b225273405ffe46a6118a8aeeaea52aa625a0a7f8fb2f73dcd08e64f1f145e5edbc2f4832c66cc491c0a19f4b59f73b69d515e4492dcc7d" },
                { "ar", "772dfa8cdabbb44e043b57fe97589940f972342cbc3136ddc905832264db66570f420c01f632b33cd964f76412fa6cbf60e5e49c485dca6be1efc051c71c6aba" },
                { "ast", "81b5358ebe17d73d644200133cb3ea9621b9474517bbaac9ad8e020a4fa38a1600aabc288314cbf3d787eb00f1895071167b0e5b948d4decb77adfc8b0d7f030" },
                { "az", "965f15e0c6c2b2062c908a8a99dcf41894b19ba59f7209d86b54cc3dcf2c3418396f20499fb147069c77880b5bc2228b9b6093ba517f82a695d3b4143dcaea8d" },
                { "be", "487561fe930cff1c911524f613137e9271f90b89a3fbd5a592ef9ff29e64f53dc8a5187cd5996a838b93fe563f4eea0ec30b57fbd22aab7967c05df986fdf1f7" },
                { "bg", "4f4aff9ca515695923a7f437ff0e02e4ee1f14d3427655b4825c216313452f29079a1d98ed95dba03d20ea9e078b39bea02759c2e91a3b5fb3cdcac18c937154" },
                { "bn", "a31f086e5da033b0ca2f89f5f037fadac7d01030e97c0cff539fd739d47a58013211a938e3a66f3237a992849ca758dba664b91724092eea1d0146081b51d2bf" },
                { "br", "aa62e592d338735370e37747616180614ca9d17b98c69fe11883db6981ec3c3416c62017175923df61f15906fedba341ada481c8d0815d231de5dcae765a6af6" },
                { "bs", "dd349e4081bcfc846a759ceae8ebaf6b32e580d127509e812f442e9863fd84347767c317b475f68f225c64b3fd854ab85b9ce76ee22326e1713f12c4094d2351" },
                { "ca", "a20c540cddd9a75a4b921acf076ebedd687a1ca1e2b8c0e1edd7f609b89d19ee0690a6fc46db1bd39c197c04857d51492d392a00cf23372566a63d14b652a24c" },
                { "cak", "d1cde44c4b067588f539f194fa1101dc5082c729e1a28356cb789deda0484c69dab5b62e4d12be12105f324e8f0db98f26046268a70ccefcb171e6d0f074528c" },
                { "cs", "c676555d511b234b4c9486f72bb2666488ade86fb991a459d42e068e2bd17a56c8eca1d6e31fcba687db38d33815f9f3f1cb92cea60eabc5391862d199dba5e8" },
                { "cy", "2b780a1f65d49e9a6c965824cb77bb91ae932b730569dce1d05c6d25b9daf5dba7fcb4815c971c6560f20306a006d4ec16c7214fb75d95475689cbe991beebcf" },
                { "da", "59595ae6c215626fbbcc1ed59a732e85714f9a97fa04ee00cb002fae36287e8658659d7083d38df4e0f00e3bb43aaeafad80e3017f04d478d322922937929677" },
                { "de", "27e7382e2ae809d80ca4edf69e2a6c14125a2a8a7eb2202e4a397873b79b28508bff56f1f0e1d7b3adde423e7671010dd88a1f0d424d9993e6864cc728a5635a" },
                { "dsb", "e40649189c1758863202ecd051e2c485393ce36a0ac988fe8fd8b30415963ee1f44c2195dd0c77a2f52fb025c0166d337f8b58c9aa46dd0f76f9339e33eb179e" },
                { "el", "250374360e43add5d61f6a79387b184b8a68d3a4898608ac909d94bceb6af1587afdd8bbc523fb95c1ca4f3183526d489841675828dec8ac418692c653a85873" },
                { "en-CA", "84439cfcc68a8a0213b17e8533e1d4bfcdfe7c0acd72d43530b5e81a128b9747493f5c737a207a27271e2abd54de9be188725caf646eb7501ea0ae6d80cb1f7f" },
                { "en-GB", "45364b0e08adea2fa760e6ae0419004e122e02acaf8b7b3f72c2fcdfa45ee411544bd9c12852b29057b9cd77e306224a17464d8e587db0720e86a3810e899bda" },
                { "en-US", "8f9641c039ae62152c4d6d4126e76fe64bb293081edb666a3dc048f0b7c7ab44b757631ac22791d7f2538b3331e10e4438aa679adeb22e67075767f253464b72" },
                { "eo", "c28ca2c3ab21aa024f79f1452ce7a1b2d2d5e9fdea770810c5ae4cf9f7a0bd1e234ee55058b70c380764cb2abcfb231684ef80773b7e2926d4d1b55800f785dd" },
                { "es-AR", "cfe7a3139179258b93175c5497e72e6cb6ef9f5d662cf1fc2959f950425289d0e42cbd4058a9737fecb821e51d3ae36800b05d3b295abde6001ab221045d22c4" },
                { "es-CL", "133787234dcb2a43f1c9a25c55072774ceb3b0430bead017c6c232b673038e839a7ae7ab2128326e7202b799ae784d7db41b399efa147dd0885215212c972f89" },
                { "es-ES", "6cea599cfe7549079adcf69ed223bffec4c521dc179abf61e8393ff91b591948a1b8af2239f3fb332de6e9b805696b62ab6a7bb636446c40d6e9272e411f3094" },
                { "es-MX", "2382df4f4ddcc1708cda9647f06cd118e6be5e5682901f99f5790bd3a097eb699add0b69b840b46478dcdc919fe45eccf544ff1519385ae7cb8fdceb11563beb" },
                { "et", "5964f9cfabc225005c18fc0372deefd934226bac27008d91807d6224549e6b9234a51de18fab4594d01b7f228efa7485d05b5432953dc45014f0820c7f6cf69e" },
                { "eu", "6a960479c4ae06b20c212f886896696629c62170b542a686cf8b7cb7d22f6f3355c8b0573f05d375d9fabf6b8abb55fa1f8d26211c560bd3cbe081c4b77159fb" },
                { "fa", "f9a814e86249dd60f622f139d6ee754a101313c4c1a734a219f73c2450bc29d0aacfb4b8823429ac553c675344e23018b9d7bb65def8148b8ab03615451e50d3" },
                { "ff", "337e53f35daa7fa54e15911427b70f3fb2d26df1d1a23171245881af4b01390fa48a719e3780efb53838b3d7a9353fa51a17a3072b812ef57bba54654907991b" },
                { "fi", "80249559cdc052dcc0c7e3dbdc9ba84a0870d63ada55744189722cf2b9ef0aeef0080cfbeaca5bf8ed3eb57fd2adc3d2f5e446b3418aa5269964be601cba66da" },
                { "fr", "53feba1f7e64261e9cb24503b4167340134a12c991029ec51bf162ea76b83ebaf50139529ea60728a0264784dcef36657a4a4cf85691b3df6f63b7e4c81fa438" },
                { "fur", "fac80eb758d6cbef417a50d1a1260409bdfac51ac6c5b6eba0827e282334058e3f79e47edb2243edcce1cbaa5c83bad6be05166c274956d7cd356e301632ed54" },
                { "fy-NL", "49e763ad50f7f0a42bb414ea168a7700d4905a5d4f5cd6010e41770f0395b94f5a360a8c7e66ebc847e07de327597ba86b31a40eeed73e53eb64acd6a9aa4b3d" },
                { "ga-IE", "b4846f7d32964abbdecbd0ceeadc4193b079e2ca2062d01bf8722e3e075b4900f16f50e45bf03c783bf26d47d0cc2f085c0d6ccff05e4ba9372b5194a84f190a" },
                { "gd", "065abc90a2a68c68b8b404afc6761a913a47fb642c5d7bafdf92a855a4e85052c104d52284a252a30c918096b71baabb1657e729efc43f1c4b703659b45e6d26" },
                { "gl", "b4e7555382999e443cf4d154830e50d422fd4921ae3744434187dbb852865c9e644d60ceffe288c919f54a7497e889a74d9be81cc12907a1ff20b9ea42b16a58" },
                { "gn", "1b1d506030055183212f353e979612fe7bdf7e90be571994608cf802da9eca45498a3a71fecd105dcbdaac1d577a8a5c434a9f126708d8baaa4ff07541a4a18d" },
                { "gu-IN", "3a92e0d05ef942aafde7332a338ad403859e5452b2b5ac9a5e50d627f3d4d6856915fdfb5020db33a548ff963e91a2e5773caffd7a4eb5cd8f29cf4ed7ab428c" },
                { "he", "bbbebe3f82392569ab70cb3fe1790394d039823ed9b5d85c4ca8aa2c407260e9c42a58e7e20977e9c95908709ee09eb96c9b74550b7bfd1bc3c17df5bcb988c5" },
                { "hi-IN", "fb8a349c189460ca372712d4b0e61fd96aab630fec8dbb094bd4dcd5450693c8270da52f76a4b0c8c26af5579257948b8ad49a711d0c769c2b6b1a733e9c3222" },
                { "hr", "6e34a977ae4707523c032281cb0b41abcdc2395a1475ff44a0fd7fe9f37c90319b311be98a8502ff1c66472571e98acd274ccf95e8735dad683b03786400f926" },
                { "hsb", "22c6e35db4d90d5f4e27def4424ab475c826cc6586c78334a0537518348b24132f9212974a60fedbc609e9e9831d7ff5b81ecac40d3f11921fed61f2d85bfeb3" },
                { "hu", "8e8309e390091ed38b2f5ab5211d863217099b9c2f65529215da9f57338764de4d825f3907f74c55adf0df78a089b3f17c2c0024f211b0e93f494932ac3df2c2" },
                { "hy-AM", "fe612355b59cddc28ce6747a1102904585ebf475f93852690ee017eabbb96187fb81cabbcff1ce52e58962d8cc674f15ec7890856fed9cf2ce83426875b43305" },
                { "ia", "356798aa7473598395f232b7015978981a3fb7043fd6369c5cdee5aededaa0c64b2ce34591aa017d73f1771835f50d0b422713d076e7f3fa0fdcc9b997383417" },
                { "id", "43a2a892d82e23c2963c0af302886f59ce79f2e557fece253cdbca26a3674a16eb99783d710e43ff33235ee0bc5c2c6a40328e8dbc0112d96596b47acd0841f9" },
                { "is", "39e7b258498d1a1be9939d87c4e92bb960abaa9bf08d7422cce04013e106c08736c5c8d9fd6116ab01c0df8bed9fdb389448871c55aba02c59303a1c19a9ad80" },
                { "it", "fb13ebb992a62a5ba818c8445036f62e984d1457605958b84b62018208ce6f74990d390932f5c2b76b2ba2c30590c4533fbca95013af398c235e1b61e795ca3d" },
                { "ja", "6e2d1f4cb9b6040267b45960ee938ef54da7fcfdff4383d090b13127a4fdb281d466e8a298e29e90815aecaa56b3423f68633f5996d51fa75589d91f70541324" },
                { "ka", "70024c33faf21c3f245ce5aa5568c018b13591e1bce2877176966647ab1c1b086712b8ef5d7b95436b581eb196d84a76441e16008613e75b97a01e2ddd9c8e62" },
                { "kab", "b8ca6444c909476baf3b13028164d93ef5a8820e2f8650b1ca5389ba0454ad330e05b5540bf1dd2705d45250457bebe5474007b99264d26fd6800fb74c4cdc12" },
                { "kk", "9dbd27ce068170ded574eb8facaf5e66564ed8b48bf2d9619466631bab06a4574ba53334a2e2454c36861c0825f0f53a0e152a4e437dab961a952323cdc67413" },
                { "km", "823471efbce3a365f0bf45a6239c6834ecf52f4dbe37f3ca9c80343bb3da298ddf8dfebe2b25da1eed461fd408287eb90fce97f647c9295171042fd8b29ce4ee" },
                { "kn", "502b3a07b0122d57d3ad7496e973ce44431403d9d62b03a6cb965d6873cd694edf634c0f7007aa6bf58cf434ee01b5d7c8ca094aa7b2a1cfddc557f65c85a1b4" },
                { "ko", "fc9bb3031abe20dcb4251aa95d4e0f621f1eac9a21cf1c328c211c3b82bdc038a5fbc21cac935bdd3d0f36f3fd062228c49eac040200d4613dbd425c99478d62" },
                { "lij", "67b592ab7cebeea70e7e8038dba0dddfc863f24622dc301f73323e0906433ead2680cdfcf836a74d42b683a99a206b2d2d704781ec8dabee31326a054d5b6951" },
                { "lt", "d946d5064486978a592f131209105e18bfb64a95cf64ad7c9bb41ecc75d459b876440d74f587eab71c213cff852d0fc0a9c7693fd647b8cb3c631ebfbbb8a6c1" },
                { "lv", "3ebd6e74b94719cfcef94428451031eff1bfda7e0354fa5e1b203c30900fa847948962315f2b0db45165bb0094e611ed05d081e5c6c59087716f3840bf5cdf88" },
                { "mk", "093aa4d7c44d6a8435480a5477de115ba2193f63e63d63e26a6c32ce9b298d636526bc092379e3d173323e52dd4aad47d45a18aba4cba9675b2c20dd9f0771eb" },
                { "mr", "8821455b5254ac14d7b301f95067a26c19c62726602bfa0b83bb3dc174881e4b98507e6cc49898a581050d34be65312c666061f8eb16ab3d4bad0d1e0755032d" },
                { "ms", "9a88436f0862b82e51ae2755b5dce1ee5fcb4244cea3b301929f109612ed98672a453e6bb6b6f869556b2d8f553586d681e79af9dba01f3b57e574b0a75fa0a8" },
                { "my", "527091ccf4d032c278adef4e7facaca7b2393e94d55bff15d53fb355c2bad055a24f7d034b01fb1b3fbda136c835f552b5ecd022157ee6256ab33af34e7e3218" },
                { "nb-NO", "61988bcb5b4f8a43ad5af3f0145913fcc019338064c07aabd788db707aa73f16a2524b0e923117a056ffa6f5c76995081800c18952ead3b79c890ed0c563e9de" },
                { "ne-NP", "cdfe19ccaef59e8ad77449a6f282ee8be5dab0f73166bcd6ddf897adba8748966a902fca5dd06de536eccc05a6e2e6783cb8195243c6e8199e957848ae61794b" },
                { "nl", "faf53ed9ad822342a155245999c1a156bce8f1c90565602bfe699f66e604aef56727d0751ca6c7be50e0bf699dec73e7b6b1116cc03240ad11873b596ea7e1c2" },
                { "nn-NO", "7c4737b4943344c8dd03cf7fc9c0bd6ba34df6848d262c0829b6cc45ed6531aa8837fa4727a1ac525fb889e1f5182fb4312ccc346973ef66dbea663b43a94979" },
                { "oc", "e74eb965c4b7fa753c1ae128b7fd54125a4a54603e8c7b0d5a9a93e331692f1b64fdb10b66474e29630861367ab20a0092bf03153d1497f782175d80133ca2ba" },
                { "pa-IN", "a30d61393367d60c7f286f4e4c772ea66b67567399903299b668a4e4a192cb8bbc3bb1cccc541251f913a290ead35c5822c32d665feb4379e3bf499bba6ccc97" },
                { "pl", "74c17833bebe529425d477ef7b24c1b80fa1137ea72c6801ff44f51b3cbb7bd5296c47eb2cd5809f10e74b54b6cb7932dfff1620a94aa8ebc39b342ef0f5a653" },
                { "pt-BR", "014c94f447f81d821d86ec729ea8bc03cee6bd6bd7f90b24827dc2b7179ea029d950f7b7b2dcb21ce25ceaae5a0f20fa3bbad612586f3e5355736cb5d85f4b68" },
                { "pt-PT", "4995b7c11b25b2219023624f848af26b5416c879e17d84e642cded938ea0de8d01956a1f4cac19d61679996d355b92ae90b850891359ab441fe67e90f584cf82" },
                { "rm", "22c56bdb862c02f7d4d4296fa4d75adc6dcb9c41d54cc4e3efcb131b817c9f316a56e9f46b4cce929238adc767f4f1eb71951afa6bb0b21f66ef55babbcce962" },
                { "ro", "cf13824ee4c4b8bc2c61e586434465a2d4f2eebd4c4c03aa128505255569430239b5fb50ae35a37cf9a9c6788baf4333483c174b059048575f480368139debbd" },
                { "ru", "e3d27a7e258881d7144a7c65cb7fc58660216504617ad149af66b6a5e29aecc70240c24755d6892a782b45a981a2b854b419f749f7b1eed6c0f2b81332aef394" },
                { "sc", "fe6a96089c21732e531f39d8bdfa701c8df7ad1054390cbd69f09196017dd1d380eac02a109fa3668333a0f9e4f9b09a5b170a592b08552c2b9150f34607d505" },
                { "sco", "63835cea926f99616f5e4f57a0c43e3afeb4493ec74ed23bb9509a32b65afb31b2aadd6bb29d14ba78e4be5a1ee7943c56b46310162b88650713b0ae18a5a4b8" },
                { "si", "bee4249d6475f7b32190c50b2590e9acd4b4962db960b9ec9c230d6f84f46accb7caec77334b61190249438c91148cf0bacb803f1c0fdc7d8695216ba858b823" },
                { "sk", "d05d83474eee0ce25d357fc887d667f4ec6df6853c2868f0492f6b70d7623c819f7411042f56fe1c3d079a49b2c3c5a80dd526ae5028f3fb960b0aed497a4eb9" },
                { "sl", "56bbf6771049e29d0207fbd706e5ff8793f3bd391bdcd943c393782d891d2fa85203dc84b3f154e235fa7904244b7f560f087bdb8b21f6e24a74bf49c674748d" },
                { "son", "d58c0b4c0a2a8d3ad112809116b6e86554f12d76d994fe5e50b37c6360d85a2a9a5f4640245cf97e895d66f7368493e5bc0cd65ee1d6b4deb328e34e84f91768" },
                { "sq", "23721e5e97d5b46876aa47bb9a78520b9b8f7b2bc0438ba972225e49d57095a34528d754f2c7f94f5fa7f8f94ff3a2c208a66dae46e7ec2e76b3ea785b2525f6" },
                { "sr", "3d6c76af63b4483f38f11ec85cb68ee1fa57869d3a345ae3c616a48b2160e45b65c650485459aa7de64d9fa1a394a249e5bccc92b801ca8f13dcbb841d3643ed" },
                { "sv-SE", "49f550cf4a9b66874b55e5d39b4abf8e203fffb98d25864ef0a57447b5a69a3b7e27b06e79e5dbf5c40ebcf2ce8a05a268042a2746d7ba6ed5ca983490f62ada" },
                { "szl", "f71546df35220e36f9a5b29c06c1fbc90bc4a05b972a3b97088bd50da50fa8f8e514d335b23d4f5807b38b0576a2b006e38447592c26fefd282b6f9c1c95b83c" },
                { "ta", "b027ed599d4be94a847eb4cd7607b43d5e066300ff8865590bd25341b9ad32ba7f0151b37ed5ddf10dc4c144ffc889461c7dca81d491d904ca3658cb14ba6418" },
                { "te", "84cc0230bc3bd672d6a6987d24c8532e697c4682b535f897a6a6eefb85ee78b826c08643af9af8d1fa0a5c2c119143ec14fdda76947983cfe7ee7eb53c88325f" },
                { "tg", "4fa3f3315c56aea7762983181c0b49118861c583bb3e3ea2a5b203aef9855585861782af133aa3431e996660738fba24dadf2d246fe58d7929a0ac11ad131138" },
                { "th", "ca4590c2af86a09030fc5fe74f6456cc00b9ae0eae57e7688647ed8a6a9ea071a9e45b8cbc28da255df3009cc67b06320f6de80a5830c5bb872c03b899f9765a" },
                { "tl", "37a2a6e9267e71dc18dfc71d30918e7e08b402964b4f555b8a6074a6fe261ed6b15185deebca6979157d7658ee675d7a2f5cc7f06ffc37f5b8fe5814ad11f9d5" },
                { "tr", "300aedc914bf648d739d7c3b97862d21f17797e233dc5b51af51877e5c6bcf28449fbf4ec16b1f11fecdfe37166f3b44cdf891b7b823ffdbbcb79dd0c4b6c368" },
                { "trs", "19e7438b3fc2349fb2c240493d165ad71227c677006657617475e68beae5e348936a263c4dcb5db1b7b67748a8001134ef5ab8bb9482d7a8e2563dfdcdd6a5f9" },
                { "uk", "e8c192730e45fd38311a9830f2e558364344433cea715f5b8d1d0c2eaa46404137c3986d6e9fb1a1d0125631b2f6e4be3c480c0c51f638e618cb6accc68eb72e" },
                { "ur", "23c099f43169b9b9677f25122e83e1ec46aceffcfa7ae37e43c066a70f09a3edd140a828044d3aed0277fb010fe29c6ea082e030d7af48425e0f14b6e02c677e" },
                { "uz", "7508ffcf1eb735ddc75ebb9da304854481eb1291c1df8ce39a57230e1a9fc4150e18f3e281bcd8088e6b36fb6b9513c2c2667d3b0ed1e8ee26556c6fe2f9a608" },
                { "vi", "33989dd47b1ef8e247b4dc040c0290a0adf8f465c632e99b96790c1513d259534fd08a9789a26b307c611930b8bfc352aeb1ecb18e40f3598414339c1fdd1475" },
                { "xh", "e7ff3ed9c11c61fa8ba64853e223a028a71fbf31db873971a52d6f39d1a4f99ebb2ab7dc25dcc08b7caf82012cedee16da97a7bf87306322a2dd417d6c3a043d" },
                { "zh-CN", "b568729553e74b827f4d77bf770ddacf4b8454eda43124a65c45de873d824c5f3a63dbdb85b9aaaddafff14e4c6b30d833d7d10f68457aef7a5d428205d3b65d" },
                { "zh-TW", "e3adde05ad2b73c3111aa3d683352b5821e41215f035355ff8cf3d104013486a62b8dec0ac77161742b274874d8c419ea9e8519b348846230a81fc49e7d6ea53" }
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
            const string knownVersion = "118.0.1";
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
