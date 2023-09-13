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
        private const string currentVersion = "118.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "9ff14e578d9df827127beddb9e9924086762e7975bb62a17f1f4e233a4db2800c89697005fd94538d1976b730a18c3ea26c7d0af65e405b23f50a99fb21a47e6" },
                { "af", "17d2338efcbfd3a735153941a0af7a985a578888ce68202cac8a65d23c09e3cbb843a29ac8fbf724c23e72bd69d903634aa48aaf682e99ef0605cef89ee3139a" },
                { "an", "0cb33af887994c874cced1b23fa0c41332640a9bd2804f2996838478745bfc9185cf42d42ed0cd80e810f510298e0c5fa1a3027fbd6f68944ed8789919be61c3" },
                { "ar", "c7ab1b2105368aed3657685adf255ffcca616697b6c17459f8c4143e26885af28abed05de52bea16fa992513a40639ee8f662e33be76946443665ed264f894d8" },
                { "ast", "611827524a50eaf6248f56fbf7e1e0be7981cc560e1a91c4531d209b481afe5e777ab6d4be98a336af013dcd8e4af1ce4aa295bf598053dd9db38a70849ad96b" },
                { "az", "4c8b48143a82bb558e7f3468d911b2fd180fb3d270966780d27ad6f7455c93768027351bf08e614f5773e7d76b85be229650dbf519628a79679838aad2a095a3" },
                { "be", "c067a9966f68ff6db0e701d4d69100605770cc9fb2118584e7436ce63ef92fa33b7fd95ac17c1618687c9d77ae083d0d615dc8ea66feb0baf8b1e1c7719531bf" },
                { "bg", "201cf554b5e23f7dbed26c2c5d7ed58eb0fcd1424c590f13dff292e0c3baa32054cd101150b74ed6b91db2d7842ba8908b20ff741d7998289f57447020f06dd9" },
                { "bn", "209f0324e365eb8fdac8b50589e6e3d4a592a919ab11d253e0e2fd100993d0fcc58335e98fa5865a01469a0fcef38ffdb09437780c000b4ce392e1a616485761" },
                { "br", "7d7127d55632aa8bdf6b5fa82904ff52260c3ff815014f8272409e0d32a6c9d782ca4b46596862416bbbe6131244d33c8fe39bf34f953f0ae8d32f935c191a2f" },
                { "bs", "a0c8f71c5789019eacad4767d2d68e62568359d4f63844c8f410642351e445997d0a1ab8b5a965bcf279c877129cfb530b5aad53dc669725303ff2079869799d" },
                { "ca", "659fbc49cb5c8904292f20821408c433d2d5ba6bfa35daf84248993c91c96e7e1a7a98706da429c80c4c88c8278d818712b1091afab671d022ba901e6e223df3" },
                { "cak", "b94924c6c7f98e272da123d3a476e8ddb430c55bfb867e3d37623fc72db6100309d56c0507f6e28a1a7cc699e952fd0aa749862db89a75a53e662ff72ca5e5be" },
                { "cs", "0dfe6d3bc89c956a624f9aafd5b3f59756db53fe92c586e77932cdcf15ce4ce45a5a7b8328bbf042e902732af322230020d775cfaf9a64e88fbb598290ae43db" },
                { "cy", "083def37c105e641d4154c96014a811b4bfeba8998d548f5def00ee39ba98153b47a45997d5c99c0692e813231bd131b8c5c5f3ea4af1b7245d6e17747b1d514" },
                { "da", "2f6dbfd918c935075f52431215086bd6f4342dc787a0e46382961a9b6f4e18dcff671362d276cf094e7d3fabb185bebdb3f2f2dcb50107af2623133487673b9a" },
                { "de", "d67739288649ef622d8c8fbd44b0af9c580d83e82bdfc062ce4cc97e6063f5f7edc72d97fcbf26bf703c7ffcc4be7ef72880794d1cca2ae850261428bdd573b5" },
                { "dsb", "e41b987621291c127c40aad0a993e02dad4c2f2fd2e4eb22623a0b83f3813fa875032c997d45a669170b7231a1170bffabda798bc7684a85e6c56e3f57f59992" },
                { "el", "10ee8852374235dd8d1e945ceffa11f3577735c58b419c0c51a85fd28220825054d850ad5cbc2c38d212071511c297b7296542d217f8a437dcd13d36060801b1" },
                { "en-CA", "fd67c53020377b40be27fbc2e74c21330dafe56c18ff30f1357858ff9015fdd3eb79a26b4bfb51f5b6f736cbc57ce9bb2ae4e448463a9e95c8d4e5de0c4d544f" },
                { "en-GB", "973e83c1ddcabcf4bac69391815088901b483e35882957b8780b100aeeb306a5c98909f615d5c0e3d013b96e21e7bb0e80a92c1d13a55549d9d8ece70d28aa46" },
                { "en-US", "7033d77ef820eb8ae482cd65f0cd4c85308a292a573671ac7104869208141f0a4a81ee364d524cf88a47f4411b35b84e9f13e254a5bafec3b8ab5b0ddfafaaf6" },
                { "eo", "05ad6dbe28f7a1d9c85bf4001644bc4ff7abb5b8a8996913f7de5a432f7434e45ff01000c538762e3be18099f67044533d0ce41e75d4c2ddf98c8f6ee52a3899" },
                { "es-AR", "39f68d8a03feae11983604629ce06baa4e265fe73fd8bd14f69c94e6bcce24dc49619e1c17d7037ae26710bcbf73b7065f015353fd04d486c55f82c6b94953a8" },
                { "es-CL", "f0800e8424e0238b0ed15d67f8d3e540c903a7e8564411fa56fe3b0ea4fcd18b458237a528e9731c8d4adc10c3ec2fc94403b2e8b5c7d11851e29f28d917c003" },
                { "es-ES", "2dc0a0f22b881dd7c47eee94ad4acfcdff3f5100caf728871fdec39bc78755c8d188bc7bd32ed9be585d99fac1cd3777a52b6f00d684382b6145aa6154edd0bc" },
                { "es-MX", "41e8d012421af3c86931df1c4bbbac74f82ec1027dc5a1124dca2d8829732741ccaf9e8fe81c0fa37a609abfb556b575f65c9759eeda804c1425df76de98a683" },
                { "et", "633c58755fa6f03811f9744efdc63f5c847b2f6ff18ec2ea37031618f493c67c16012d13e4ab465b4e40690d952ffdffb4460ec3db9d0e0414af0a35e1489d95" },
                { "eu", "6d0ee0dc1ee05db328c54afcf773eb4ee9ab1c0d3f8960a80714a122c205c44fcc64f6fddb7928ea2b999bb14b7bbb08b452915c9334190a73eee2d9bd61daa1" },
                { "fa", "1cbeecda3eac251e0470f6a6d7f6de807aac37640108cb977e0f1dfa555ad691abee00c8a2a4c8b37024b977c2e653ec75975564914156729e4cb9a4ece0c476" },
                { "ff", "13db1163998f8ce688d71311e0998e6e363f2146b10fb5c6122b51e03bec3a39af376a76bd4d14d129bfe4befbb49c3a32e3fe95236860297fda063f8bc5b601" },
                { "fi", "bf842b60fbfe3ad854535cfca212e75f05a84a1846e97b5ee9e053c68f4986fa1a3eb924c596912c5077e4545fc0187bba2c788c2d3325d30312e9b4d8c0088d" },
                { "fr", "f1d7a53787164278383012b0b8c4c20de19e24a0294453de2bb8b3f72b2f616836d8241e96a438e602ffb359c2cf24c41a848140000ee5cdd67bcb3f080829b5" },
                { "fur", "a3fa9e27d7f83429651375d2614d99b2ce9bf614e0439567c7a379a1a113a6e7272c574074442c406c3dcf99724502444a9f1d312b2d5422888627df01cb9355" },
                { "fy-NL", "7c5e7cb7de63a7109ee20ccc3e3a975bab7183a0b0f10e703fa48696c4ecf0a5d15acfa886755f408f4498d290cd48386fad1f21cf7513f50494af5c38e299d1" },
                { "ga-IE", "fb0a60d44f2ffa1a2740b28be95c21a27ee27a88fd7d9a9a350e520bc0e266b90848fa4053d1d1add3e4d522a4f9ff7b8b200923139a02fe268f8eb02c7d5188" },
                { "gd", "11d3a8d0c860ad1a8fd465a5521d14ceab98ea5406d00723b3c36f307ce86de84252fcd9ef77de0ef3bced356919d22ceaa7934ad40c5647a57471bdc9fac8cf" },
                { "gl", "922ec6f36141bfa06b514cf4a5f93da0a757bec83a39f2ee5e12abfb0340c1fd16bfd42529a2cca5579eb6838c7b1b9b02af291c907bb6e5ec835c6e55c21e15" },
                { "gn", "d72528e0657c8782d6fcab4597313b98afb3358b34bab48155eea272fa6d0b91f906b31fef2b8bbe6b140f81696068c47ef702a903ad1d29c1dd2aa6672e6613" },
                { "gu-IN", "55c0409b0327963427770f431111e40c6e9d4f81383b406fe2edc24083a00bc94643622663463ac3c3c680c78644005e9278fe52e9ffdc1236c38f033f243233" },
                { "he", "aa4964a262f0bac842d69b8ca9133cd3edec6f5bfb67682b2e106b10efa9c5d1674f07db3c969679b87f00832e2a5e5ce60a1c64abb6cac210302f2afa4474c7" },
                { "hi-IN", "721a5f931e746cab58867b88f434e040eb4f69fc7f1b777e7755b10b86b1f7e448d04346fdd8d4be6c5f2efba8f9d58ba7fab3a0cb41c9d837dc8eb81aca2b52" },
                { "hr", "7bf8000a440edbee0cf5a1476f6065d4856f15141dcad5ebfae7fce60e6e674c602b90b31cd6357c47e50e4fae67160ce1899ff8dc9f208b5e9e85f6da6fa760" },
                { "hsb", "206df34a4bc93a60e874d3e991d32e4d3e53fd19dd907463be8f902bc854b63e9afb2dce8ffa23c18d56e93c3ef3210f474d3f4e7327797aa06e19c418dd7c49" },
                { "hu", "a2ddf84f776dc5ee65f06377893efb15e5ecd36b6295c30e3d188833bfe58e618cda5cf454ac0705bfe0f5fb7a5f9828739285c22ca0e52e574acf8a967faa33" },
                { "hy-AM", "c52afed2aa38a6661920ed3988027036147710e9f61e8e30402afc805c3cd8c300c130f380158eb990fc13532411fcf96f0fbb37348a4647f5134cba21889e85" },
                { "ia", "db8a8d3338c3e87878d0f56e23c5aede5e3a05edad004bb27aab7f3a2fcabad6e6bdf8ceb6f45f306c28a598712e80142c03b4b88651ec43df41db6400c8c696" },
                { "id", "589506ec88500ba567a252d358bca47e5a0446497d9be3d8a6f1abfb52b12c636cd8315716496e670b0348f5b608e17efe6d0a27b588f34a295c3a3ad307a5e7" },
                { "is", "104348c9bef498027a44aeda3c31edfaeabca3e412eefef5e512087501a97996bd117dd16b5f6f8d6d6964c01c3386d36bdebfa01c6db78d3c7a58f9f83c22e1" },
                { "it", "fee60f798022b7555d85d2ecc13bbd49542e82e041137c73260d131a78900c7f1b59c99b190defcffc1c1f078781972512a512c1e3c7ec11ad933e9cd6d94164" },
                { "ja", "c5c06072278ca33fb95a381cfffb88306f38d90fc407c748c9225f73594dfb622096acdc7a2ffabead12f4657480d27a374c9ba3cdb158606c1269cbd1256ccf" },
                { "ka", "e81fe469a3d6ae84084c84bd6a1cfeedaa3029cdb1766158deaa834a3679424f8f1f782a2e8bf32a85022a00fb746c715dfb31e5660583c38e0aa9c9d370b8c2" },
                { "kab", "8d2a4d2f57691fafeffdd2847f89038bc893d6b8e3664d520a87f87673503ab68a40d01460c9b7ec5bece48a6ed2ccba8a995cf5e3a28216990018697b9f409c" },
                { "kk", "48b1c069c298e3b70dabea483b16a192098a22f87d7ba9c5938e704b786c6797d3969b6920d5dafe90a83139df5baefc17aa36b83222edf19d6355415af2d412" },
                { "km", "d98e7e101d2cbb4cf626141cfe26123ce7337f4e47acbdd805be092039123a388e31ce6fd760e9ca2f1301ef69b3fddb27feb23fe485b0dd9afff356718f7271" },
                { "kn", "cff5977ff4f075e6956cb9fbe7b111827ef4e9c17c24286403ef816588919f36260ee801e211dd82f1d01fa11ae0f5dfccdc5c620e21c5f879c415019227baf2" },
                { "ko", "301d92931acffb2beb53add78febf219ed25002a53b9bcd11d18df807c70120e2bc2b1ddfb8c058f9791b15b2d7e971a889470581782e405788599837b4228dc" },
                { "lij", "bf379174f1b6a98b4462bc0b186cea437d5abf5a98b5353514cadf55e284f789039d923892f7ed8f2978155ccf4eba7daf462933ceb1eb3f1ee1e3e860cd9364" },
                { "lt", "b745ffe7efb8af8ed901dbb738e4ce0bffa8d11441d4ac4d362934f56e75570c7aee8dbd2a9a68d2759118fcb9971f19446a084132aaf304ba03f4d63615ea13" },
                { "lv", "641afd4f4dcf1dad5e1954a2ad4a94bf2d3beb2ea9a7a9c85b2425f1b31e424cd74831c860fad5574b8147b8e9f562ba7c0127014397ac99bab2e5963eb1df9d" },
                { "mk", "c75d7dd9abb5829d81aaaeac16e501ac285d716ba5baa736920eadb2bfd8140e8d6f1e190c5fd0faec5fe7646c627d1bb32a7abad8f3526268f39c16a8816803" },
                { "mr", "7fda8c4f6c65dd3481c4c2dec311e85ada4a804a9ffab6916b16c850f0d24323b61edbc24a5241ea01a059732f49c271764274e3f848425103d7f35124cfee4d" },
                { "ms", "f5f4a02f77cdb1d44bb878eb4d1002582ae93daebee47047af6204aee89bd46d2fa880188b3751c05f375b348742855493e0a2b45a61d2055860289c7c7f4750" },
                { "my", "11b5f610187de02b86e5e7bc63ee63f58a6609f29824e6a2dfbccc79f0fc2f60d2b7e61b4e0f1d091ba9401242a1a9951533892fd7eeeb663547f19c04b2a39a" },
                { "nb-NO", "60a40622040109b263e250021c38c544ef984bacd139a8f576b78c32c35716da9786880266c0c41ae26e861c2cb5c44ecccca43c83a1860fbf1de04f51f8eef1" },
                { "ne-NP", "fdd1ece625924be6bfc147ed64fab5c805b5e4aacce3ed1baf21e1ad6e1be5e15e71e4ac5eee44e85c4bacf7a14ae61e5b06e083f401eb594a52a4a0dd260176" },
                { "nl", "6226fe9ad0f2ccf3072353a32ac796b0a3e271748ebaa6d395968723ec280d23d1b4d834936247e54bbe142b4d3d496d66139f6dbf70bd3c323e1c8068452455" },
                { "nn-NO", "a18be050a455a2ecc2c705c8bbe83f26b21309231beab9df9f344cb060cd3aef7270f9606cf0d9d480a4bee1e17bd4177e40dd14e9b45e5e59a595412aa61443" },
                { "oc", "3ee50defe1d3361851a9f1f03f3d9e21cb4f87da9378ea897ebb6b5230d2438b24106497cba5f6808412edc99422f850696af78674711e18fa5feddff519d4f5" },
                { "pa-IN", "20cdb7492ed35b63e64eb905f28ed311d0e2c002e8d65a1c365ce585e2171cc2639d6aff7d4c55f4d0f6f47d7264c883dfb4c36470992384ff367c8107d8b641" },
                { "pl", "efbf8b5ff364fc01424c6625624eb48e1d5315fdb3edd84cc743dc624c78abfc9ffe68b1ff5b67caf3c701060891cf63e74e7150961429348627b0fe65eaa071" },
                { "pt-BR", "38d94890520960069bf44fba52a13ad92c1cb9ac54cd944aad8daf655e30bdf719f4367b4c98469f6c0aaa1094522df6653b5541017e874fdcd7c33cfdadd37d" },
                { "pt-PT", "218d153829b2a66fdee10f048f27ad2db72b1d011402c6cfe5b8faebf861ded9953f4c764b67fa4b6b5320aae238d0747175d7ab7eff31138337146feefc4a13" },
                { "rm", "40e381d06c5545ff1a52729b64aeaf793bd08e1135715105be5f5a391e072273c8b5ade465f8dd23a1fa0a7a36e4ee64ffeca8b7b9fbe5512c23626310b63b00" },
                { "ro", "d7dee8848a5a874fe057328a53c50790900a3410e502ceaf1945586deb6949e437a0b16bfc4e4bbb413221c67eebfba5085255f85c460765298ba41171c3b761" },
                { "ru", "0cc8578079e65b3f99801d4d4f2894c5b3a473db70fee2a0481a5d783ca70d20511af0a49f5a656d609050a147438f1714fd9cf12bf0da5cbbcf7c00d7bf8afe" },
                { "sc", "d2fc7dfd7a9a1238a19ebf19d2d65049aa69ea0628c11b44c36327cf60cc36ea9950bb9ba824b2600bec2502abbb6706a4b5062fc4954c2dde22264a19a426ed" },
                { "sco", "817e55592028c9a5c7c41accbe7a9fb282c70213583487c2cd7d6f7ebe237f2d50f38253756c96c20e0da84c11d8770e633613677dba11a0f291687b9f95e93a" },
                { "si", "112d7218c3ff89f95e6f16f3b32adb8e510be33757a711b295e902ea7a1f09690e4fd348b01337d2e493d4301e974b95ba62d8a311f85e9a507208b20ca9bf3c" },
                { "sk", "c0ffa350741d3c72a0e2455b85b2143d8bf6a7e1f05ff27f196049d9f7ad489d80f6194b8797b85522e102d717895254deefabc9fad16368613b8dab5b341ca0" },
                { "sl", "b1319780bd0e5ce53b1b319557117c211f96e4a1349a5f518b50092386893b56d7d6115813c3155fc7acb9f1f81e9267cf28716bfb60b7a9cd418aaf94b0c872" },
                { "son", "eda98c2b5e7ac027534d54814b09e7e67e1a8b39b6e0bfdf0090fd3d7a3824df18c9c56e8b48914fcb1f44ef959959076ea8e68c0142e546e962d9baed26d65e" },
                { "sq", "37d22d4608e2d482102dca141e9db94ba5542ab33526532c3f4eb75bc00286067f7e57d3a914f557efd99bd218bfe29234edd8e08995fb2a6b24e7860b3ff563" },
                { "sr", "221f0f535192febc4ae5ba02c473c3358999265df01449568c7cc59fa92cfe9cedc1da7431836e9166484d5e287a2fd1ff9da3c73fc602c3ec7730e4c27e6a51" },
                { "sv-SE", "e1e8d297a15d55e3d2312c137443882ea734ceefc081e654bcead9af100758b164c5b8621b79c366422a69ebdfb1ab2bfdd2457f31ba2acf3d8bffdd8f32edeb" },
                { "szl", "a125849d997f4724062b0623270d210bddfcaff80d10cf2506710381e5ff5373ad8536e53f25887cd257a033753a0210e94b8588ace2dfce9cc6528aceb03c55" },
                { "ta", "6d154474e67bb0d8f91ff5b30f6ab30b694f6feab0296c4041f9e244fba2c6e8b8cd99cb8ac435cceb7c79ec956db478ee07fd9e05a6f9b1317f59a6eb6043b6" },
                { "te", "cbcb9d7ab2d576ed9833a59d314ddf311b2f09f0251e9562e4447400af6521aeba364b9bbd3cdb6d67b870ee06f00553674a8df39e8410a639b8b294dcceb241" },
                { "tg", "dc60637def661f1752f17fcb4ad77993d7865edb1867c4d436df58249c4b76f7179dd216c0195b490560610aa2aff0a0a2ae7b9f5696106ed5b0be06f7890593" },
                { "th", "4e395036b984cdcb03c393b8cbfcc0ff35d582091c0cedab7668455e9c379e2f04794055954cce2d4895163956e1f7d73eaffff646f8eba75e23f2d2a840edb8" },
                { "tl", "26b1dc5f3edd1d1c5efdce519c4a11b31ffb5fe05673b75b8f9ce85cdf243ff4801ef25c2793e510fd51c078228bf03f0b805ce3aeeb589344b41a66ce15bbda" },
                { "tr", "ac9cbee5735841898280e2bcf12da65be18b18854abf318806a35e709dc44e067e2301294be6b9f38f209509f51546fdae880d0b620bb9e1beb8806d530474cc" },
                { "trs", "ba0c780e100e219a85d6ac748f1cbba41d8d9a2f7abede588bcc9dc79c61ad5ba6038e1626156de9766e2d9873920dfdbda0c598774b321202ed41dc07f417ad" },
                { "uk", "d25841aee9f4e0afd63d9ad1c6e9219833441791ccd7bb8be0bb7a43bf2d0f9182c36cb6b957c4a5c96d0efabe8a08606586cd30b7fb157daac5c34718e0902a" },
                { "ur", "074c7097d2a1740f83305bfb91f249c19a73f46848586fedf9a4a9af2ca7467cf8ec7c45cb164dfa735846dea0f11d48e8c8a687e7b4cb7cf049f36ccda55a28" },
                { "uz", "d7b8fb542219c1bc0b81d89ed8aab7c2e415011524df7c537136fb25bbab47a1ba9b6beab092551bf1dcf6956af06f4e5fffa834d2d9af7df012386b87d62b06" },
                { "vi", "f8ffcc387abf607561fc529afd384f6d2a639e99412259550d9fede86438733c4f7137e22d5f57ef93a7ffd986028efa52f7b09b970083b1a9ba25568990c51d" },
                { "xh", "007334cd869df5aca22b0adf6ecae639c5bf3977d73b4346f0e3e29e51d83d3a1c776ea60bac2900b471c787cdb05ea42746fad33cfcefe5418b061cb9ddcca0" },
                { "zh-CN", "a83120facb4c1b153c9de4d41d37e3634f17a9b19b5690edfab31e541efab7b611bc4804ce3b346578de7a71203ffea934e6d278534a907d96c07ac33e0bcf9c" },
                { "zh-TW", "45bc64dd640b12f69fc821602ac034d559d7deb2d8fcf11ee241be74d4e73a525a4f6593d772d0c1b952407f536b8cca7b2811f86a8b92ab715c2570ededfca9" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "7c91f643da161ef8088b30675d3fc5cf05a6faf75a52cbdbf1eaf5325989b08613c99cd664654a9346feae381f708a4e51f3ed634fc4c06ce27cfcf0a6406114" },
                { "af", "070a8895cdbc728629b84f27f2319d290c2c7f9cbb44a9cb65daedfb1e021f9c663e373ac8bb1c672504bc6d2ff0546cd0c488793c937b0dc8604fddfc6f88a5" },
                { "an", "d23e61101d0cd94d15cac26f188e42c40e3365f6f49f565c906ed87cc9c96c13397dcdf97f9a44bc33459ac23e8a68f2f44b9ab03da0e9aeb623205d85df02c1" },
                { "ar", "97fffdb50f5dd009691d57380b784fd64fecc7cf7c9794e0990bc68aec9f13440f3f91b66f198847012847d0d0e0dcbaf40aa6e6d8da6074c5c9c5bd118b534b" },
                { "ast", "18e910c29f0ad72ae5a05d42cd6026b378df87e7fb997a8ca91c82ef742076f4826defcdfcf40f5a20fac05ca8edde2b08ec702ffca842ffe89fbaba3a6e9bc5" },
                { "az", "e4fabe173e38c3663d228b0a64e4b6039e6ebdce2297ddf4c7adadbcf162cea88db4ca24470ae3b305f6b5ad65fbddbde89bc272a6682f16d6b950aec0d5b048" },
                { "be", "bd307ad1b16e44a6cb8565e0a0ff843a45e8f3b6733fe2e6cc7088a5ccfd32de116181ce21a08ff759d14ce9e4b82a080717d73a84529f8beb8afd32bbdce339" },
                { "bg", "55f9d31f452dc20dcff27b98a367b9c160599cd14d001ff1d8e8f692c41880c68e23e56aa804b54ae500fa82ddda9ed44a9349eadd23c597748c9ba5d605f18f" },
                { "bn", "76d91d92693971e37de28885dca9dc72ee0831c5a22d5ab5ecb2d2a3b22604cc9c581a4e451b3785ba0d48a474208f8f0e46de0fb41b101576b9ae383618820d" },
                { "br", "35dc83a22a2d1affe1cb981cdce85cc7925253bc0b869f6e5f880b0b3129a0b6c62ca00609f83746499eeb1b5b19a6723cce47ef55fa3fc558b385094367cae9" },
                { "bs", "ab673ea978dfe2c61725ea950d206213d8c010067e6bb1aab850a6d820fa03baa720987f327b977f709506fd404921f6fc696597e3534aa6e3ca77201a415644" },
                { "ca", "9d0c3e3138ef38ce320e68968ede5c36b8b1bf983d02d28ece8e8d953ee63af14207c39d5bf300552651ede4b0eba4442169bcf51fe8c6c5dffcf09d283effe0" },
                { "cak", "cbd2f4ea36e1f6fdf8cf03e5b0a4bb5b675b13c8ed0709ea1769eb0d1f62006ccc38929c176d9e817554a6e18b0a3145df9bbc81956e7df667ae8dea18350f35" },
                { "cs", "2878e9372a2c8b6d5feb3217cf17db67429cb19c83b858e485ad1003b61271e553d2a9fab83d63c56afe6caf765830468181f10c2a4411a873a439ee4a4c6b68" },
                { "cy", "0825db958bc9a36154ce053737503fdf4be7d67f30c45b4e8c5003f75e2773e9da7454683f70584e058608a477e41c363f93f59d5279c612bff29eb2b518cd1d" },
                { "da", "44dbb082a35366591c7f0b1791a2bc0833265089b05345701d6cec3e3a00344ac3e167303d139a77745fc9a3c445cfbeb9bf05eebefafb6372705508fae6968d" },
                { "de", "2f47a3128a42b6bb0e3111f520181e773e648814d1eb4f6835eec4fcef9d42340e9484b19ab054f432a935915ad7541c4db848c7981a56333b8a9c57e49f1df0" },
                { "dsb", "48ba76e679b47d549b9526a0588697695af47e0b8b57a454d18421b70952ab1f599c610efe57f061d9268833bc3dcb1ab6abb265cffd1eab3c41f27c5a6caa45" },
                { "el", "d1ead263e015ad9ed27f0030b9b929c0bd5047aabf966aa6a92210a9f0481c7f9a5f2e5f53af2a6baf33ee3a2dc66651538fd98d3746c232391620aeea09ff99" },
                { "en-CA", "a040e93e484761cb2842106d43e4fdf183e45fefcc49ed228a37eb88d4fa42747a3e254e1a4877b71820c368e9a329095008eeba4330563a3d2f6724d8787d9c" },
                { "en-GB", "3c37da1616ebf657ad98617b5caed6349bf49915b93009dcfc7f7edbd0b41411794c3faf7e65d7fb2725586e8b436a045a67f6d46a8d831db9a52d4930ce14dd" },
                { "en-US", "0a606d0f91f74cf222fbab1571f96a830531f66f9a84ae577782bd64f37acd6d79e02536f03b315fb27f08fe4bf52177050abdfe29cd56b996b5fe80b643240f" },
                { "eo", "26600aa3183bb8b2ed9b7639ddd0e15c3749122268d5346bc5241b8d596915b612ce14b3847d1015e11a083e4e5289433568d5246ac1099835cc85c5e1dc812c" },
                { "es-AR", "5140da7e66c7f7c2caa4cf9db21f65c311955bfb71df788f5f609a9a8f5c8b8647ed73627105c40a57025d37a6952c8474211630c856bd52ef1741783b76ea10" },
                { "es-CL", "b9920298749b158d4b44bc9f9804ee46e6900e50490f06d5571e06503d1acd814da21918e3b4897d0818e66ea701e2162b28618145bb6eadb91d835dd01bee3d" },
                { "es-ES", "e471726910f1666fc87279b4c5d028ef933e353cd15452f03f63fba1269befa2baf8fe4ba2e864a36950e79a41f0c4710f82dc429a369cf2e41927d75e38b517" },
                { "es-MX", "472487df5ae65f7e0c3249272cc988858ce456bfac0b87f61618f8aa1d832fb7e65a157bd7713fa19550d5ea286880cc09137be647e0697be07d5410e2167634" },
                { "et", "fc15e8e551a57afb2266892122c5266e2f7d5db14a844073d3987d27db699b65bad7b19c02718d43465ca3650b27367fce4a5b10430118ca1815ce5199f0e9a2" },
                { "eu", "d07de99b9ecb0d37e2e4938e77fb34336c8cc14d140f85f5252110a686b74bd247e11b8dcb81760b82b10394958a74ff0a041d820da98f85f866eeeebd6fbe62" },
                { "fa", "e6f8653fa8469f880711a80a667b7458cb6a6ad8c52439b2f8e62313804db048e8687619f673a3e99f0cc4a03cfadfb99af707b3d09a362a6d9d33a303da8361" },
                { "ff", "6a31eff57691c861acd2987ef2394fc45571c3c3245da5db9aba9d576d40156661be27ffed11a8ec26b0258402416e28dce318f2109541176313d9c3eb2cc5ab" },
                { "fi", "8f69af6c0dad46f81aacd6a48e249a683331432e5eba989203f4f08bf5eff1aae079dcd4a6b081908f6baaeadc20cde73bef15fb0c94cf9c59824f6717724afd" },
                { "fr", "010914db18b379758dd780785861ef03b061e401a1d92ccb61d6956dcdc9f2904f3bfc09c8c840980295c8943ac545916221c564aae71fee3d7c611d0c2fe22b" },
                { "fur", "e1eacfc15449e34b8dcb6062807e2d60c74dcf44edfd0519a7444ab17388750220df599d61072a9e58c51d1227e23911486b9e3329e9a6a1e99571f2808205b1" },
                { "fy-NL", "56a5df8d1af88ba0addcd078a59a399425d7e80317b4a2c5abebab1b95f88d4eab27a4b25044f422abdcd8b5733094de727f435e0713dcfdb7dd48bbc02e63cc" },
                { "ga-IE", "f0e8161a13e224d9f5c621e382141abba879e73295b3c4314faaed5eb71797b3544c4c9b1751288904f503cc8b9ab0c297050858386a7ce3e13d4234e7a6fec8" },
                { "gd", "7d85d78266484dcdfa3e88633e93b04c479913fc4140251c86031639496b5e3e0acd9c9c614452a438af617c3cfe543c6069becc75b8fde166139defef8c742c" },
                { "gl", "d5faddbb490b31ebab04ac4260f347ef9c4a946c3b29e9359ffa316a45d5222a3cb7f67c334df635d7253d217a67b16a3d9b98d07571ffc3dc421b000341862d" },
                { "gn", "9a63fbd6de0146ea89d45911e3cf9aa01cb45067dc0002339bdf887d50c5dc6b0e425cb61c37405edd0cd041e334d4669001108bee50d0b4b2a55d633b42973d" },
                { "gu-IN", "f137dbe78a69eaccc2997c3710b25dbda9ece713049df67cd4988a40c09bc50183fda4f08681d03159e9360e91d844a274fc6824cf58e9d19714a5788d6226b5" },
                { "he", "d7f744b4e711bcb39bc05beeac8d52ae626539ba6dd9ff20395272bd4085e6ad5b0bcbfeafcef704d87fc477b285c2ef0598e47c61d3c90e8c40018abc346415" },
                { "hi-IN", "f81ecd1e1ef3e4850b8cb695add42f4b29ad97ded34453e5eed0d26171072fd024b7a9080738d8789009038e0dc4e69a8304995d675a68eaddb1d20a2e056d36" },
                { "hr", "030de9e5a446a1531ad13277fb0ac5f83080ed6f7d24622ea772f7c97f4d062f932b1d798b883f327cbb2c531461f82b4a859bf3067a76fbe23e59537b0ef58f" },
                { "hsb", "a003d9e9c54ccc5b34f90a3cd397fe82b4c03eb87d8ba8bead39f5b0f8c2ac525ce77d92f3fd6e464892e52d7d26dc0df8a354084a91c525771c8caeadbd25d8" },
                { "hu", "c9ebb5302d44223f06168717c0e3cf3d3e923783f24f9419d0bb492a42db6721b50a7e2481a2b1d09443b88626069e963e4aa9b3f5e7b5838b54d8ba29bfd40c" },
                { "hy-AM", "f5c04e70a9700ae51f4e4b89f072c6224a269279ae983e05f1cedb4636d97f96bfb05d1b79f8afc507d41eec60f389236a3952d3a4ee664ae39fa2842bdd7874" },
                { "ia", "ede9a7724a850b4aa809c87d4aff732fc2d582f86998531aac0d2b6d9eac4b2d5b48e03111767cb3140297bb53c57c75aec9e275ad7599ccb96390c5017bb91f" },
                { "id", "f047db25184c3a0733c1702762e6bbb634d6f28a795180582f03b652f5ba4d40b8ea1583386b709d859283fb5d3bfd394ffcf6a11bb90deefa547e921d30143d" },
                { "is", "4484d6f1bee2451f634e8099d88246b2a7ad4213c4d0e488ad4b62f0c7ab72d6ac9c66fa054bfaa55b71ec7081bd289673fca016ca1555568ad5d611032658bb" },
                { "it", "280844426d4dd9b3bc47400d0b429d781a5b00e474082326dff27163eac5b05c5c0c8199dc1ee8d65bfd736ef74a96820c121d1f23370f7824d2b4e62324043b" },
                { "ja", "5920d0f8d6dbfcbfc0531d3ebd090e42275eb7e332cc8ac7690a2d37e5ecc2b76a2c91947c2d6a5fe741a6479d8253a5de194e39e55b6b19d7816849993315b1" },
                { "ka", "420f7c7d031a350f40bdb3c0afc6221e3d981f56fa60fb2a98356e7ea2e5eb50523c9d81958929c6e5e1f99279a0d8e6fa6add73c083c10634a76d9860985df5" },
                { "kab", "20c1cf134184c9682dc94bdedbe5c794caaccac227330a4843f92fe831d3f4b217bceed67fdac7c80c87ade06d7aaf1347c3d57aacbe9cd41e305c81b917ee28" },
                { "kk", "bb4af49e041e9fc6cbe254c22a934edcfbe571944cba6ef2de8789ed191f24e8a377f467b352fb64479e42630dff46ed61ba80119cf28ee31743669f53058ec4" },
                { "km", "aa806af5257f5cfe25990f35b57b7e4eb15f9f51874b7eccc36c5973228d1bae0f02d563171266819b21641f9978c4b7953d4e908716896de78bbc1ffb2fa681" },
                { "kn", "8989d478f0be40f419c2361cf9f1e2e1e9ed4d019baef0545c99414bfc080f43195e7a1a46a49f864899527879541618bbdceb427310cbf34bc22e01916922d4" },
                { "ko", "4679d393b533c4625424ade7160703359e64acb16e5cf6b9a6970362edd86b5dd750a1d18e8e8d43776565d6f49c95850aeda3c92666424cb1e9e157da1418d9" },
                { "lij", "46551e502fea22423b9441eeaa4c0c48c28f77b13006ca572c999a9babd7eadee0b767d2b9b2f00103712e8b16d26cb337517596a0b6c6cf0c7443ce6a2e0b7f" },
                { "lt", "875fd7a973852fcb574b635b8fec0dedecda0f79723effe4027a5d7e9bc77d667535dffe842709c378721d678cca6f99290c1726283904b303c92b676edb09be" },
                { "lv", "25f743ed97e6eb5c15980453a73ac8fc20ef90fd4956a23b66ba0590707f6ac3bfae79a55815946addac85e11627e0d77fe93861cec409418d16321be59f4ea5" },
                { "mk", "63b5ead9c1944ada7faa13be0336d115d9282f541cb1e48b19a47152229c3db2c298eccf5451a960e1e4b52033737b05382e825eb2521f17488a2eea219b4929" },
                { "mr", "b0ffe34399b7e65ba21344e60256bc2396949805dfa83764ca453a007d13a2c377fc8a2dccdc4d129082f93db9d786672bb2140d961aca14de7383d50bd940a3" },
                { "ms", "76079860526463bf9399ae60d92667e77d7aa87491dd9ee8dbf1fb9b3b7a4e46241cab5606d74b5605e1c8ad48165d09524a0d13ff26cbf4e282a06a802244b6" },
                { "my", "21846e3896d5729476911b74f95682bd76f61491a5a5ff1483c12c46d5b07cce267058da46e588fef60bb7f3755ae23381989ac0076eae5d5da718456e1f7b6e" },
                { "nb-NO", "e8e0e7c03c075e3a4e2a0905485a6f02b347d81ec6987c34a05b2d238fcb8581dbf633d211a439758b54f88d5f89845083feeac5542c84ac3f51560df3933d54" },
                { "ne-NP", "c94d0e61f1f77485210905bf7cb731d5279833260be3c0fe8cd34cbf55da8ef1c68a371aad89285df89297817790f98a91b319c6521b69e12bcf25984043ef79" },
                { "nl", "897ec260da6ba88a7b904a59e085ff5550772c7a574630326bb27f33d2951fb8315ef4af1db4e55ea9c4430eee36b3fbe156ca77a342fbd1a3389a21e0225315" },
                { "nn-NO", "9ef1371bdbecee5b8350ba8cdb354de4170552c9b7ebab8a181c63014b76d42ac207b80cf2ee10d5c8262c6b6e23a2e68882edf1fcb5520bc1a87d0bd46808d5" },
                { "oc", "d4c04815ad7d7185488a1c782cb6c3ad8278d3d896e1ab6cc2672505df6c74278eb89444c44d88f7cbefd7799cb3e43b261554b408238843d76d01b53fdce72f" },
                { "pa-IN", "624e7763a74a755d5f3ebb7d6037c04f0b112f4089a293cdd727b1cf897ba54b5e38f847b8e3ccb69d6bec35c691737fd074916842fcd9328b1bfde10022e9f5" },
                { "pl", "fdfbd0841d35baebe15a8374e69bb24fe6155a1e5b2c2e820332966d12fd0fd689b9fb2a09e86bceb154d428be529df55458caf5dec7fd420fb64d6af1564c8b" },
                { "pt-BR", "23cba7192f6ff879125385203b51cc70c1096fdbf3f04d4630873044a778da79e84c43376578e86a2d576380986e13e88491120bbf918d1b8b5be0de3485cdf8" },
                { "pt-PT", "a376ff938cea6f79d06ce0a2fc15915ff977508bb6625bc2cd42448d6556ae26e206cd6b5a76c8152cb9b7ec17aca0c58668a40ce282d732cbf256d3a446c801" },
                { "rm", "9ae823c1f0556ee85b27c718737c4d5018c9f0e23063c1ec49ae10e33d088d79c9f62eb8e0f2499229627f611a1634649ac6ddf9edd2799246ee1b7281f3ee1e" },
                { "ro", "e69a378730edc82ed1ddd195eb79e0775a126bdbe336720d1c4867fa14f48baa480e573c4f4c9cd58c662b980ab3c6329eeb5474dfbd6724070bf51f9992670e" },
                { "ru", "1fd4d5eaf58d5a1a05e140e35b4bc38a43de267ad9fa1db3a87202847c2aae42cc03d093874bb2d6a8b7e4680e8ce70db49522079e7cfde85e191a471d16efaa" },
                { "sc", "422d9284d64db7422a6f8991d9c7c25e35b80ac065275f9b30a3c1f286ae2a26ca251d4af978b6ce6a2af24208a44f608073ee03bd5d33a77a9611a1ef142113" },
                { "sco", "d9a315f12145907794e9f04c6f459b39679ba7754deba05028db86207447f36cd9e2a5aa039f1389fe0ab8fe0897ce03e9e536af210b346b881bf79d29eed0e7" },
                { "si", "5c1511fdf71143aa7254a0eba7b46507c7ab98bf9ade14e3ebbd754220c17d17ded8e6711db8a2462fb015a713019e1ea28ec6c83ec788c4d1b9c15de14b6639" },
                { "sk", "92b8003237cc9dbc17103ca401b988a260e5367be0735195ad66bdb7b9f48121347bb33d8f1f966d1de0d4f57d72b950c70d1811bc1069a1b7b681cfe7b90a01" },
                { "sl", "fdc6cf30e6d5b235afc639d56e568de54b18c7eda1dfb86e3d94790dec66a61119de15acce2ac99d6ee492a2caced98331b37f237a1dfc8d43e1ce11943a63fe" },
                { "son", "101f0ff69ffb1159eb5b3ea74411a7de805dccaadc1793399891a37bc3e8558c9739d1d7c80dd47096135eca9993fe89d5348b0ba8a01db1f18ee072cfe4a058" },
                { "sq", "ebb77335c05c85492423a2866b23a7ac3ae1320721e600cfea28a77df986296647b08a08e11a5183c2aa0306fcd2c8ec938646a9edce9de028ab3df896de8a8e" },
                { "sr", "c4a2f06a8b8aaf7afbd1b0a2f9b2a23fdf08a9b6756cd5ed4ffe0c873c673dab1d3168ebfd49171d8fefb283854fb5282cf418e03efe2a013aab637d8bf20982" },
                { "sv-SE", "7aa4fe7b3293b31ee929e0841e3b4ef2a5b0f440431f4191ad099b302c1c360006327a592a2ca0c03bb39b96dafd7ec819571a7e2970621f7879af5af3f2e7d9" },
                { "szl", "8ec38a5b9a884af15283fa2dd382a4af03b56fb0b1f4257f8ba6516b4de698a3e2d823cf9a1b2602505d5f6b0de6637fe3b0ffe69bb0dc82f0103c168ddaaa78" },
                { "ta", "adb218ce77e21df9b34ce0d138df6961b1386d833f0555f37a5cf8098eda07fec9bb6d00fec379da597fb1433a02a58c10a489f42bfdfb32de8fd2b8977f7e43" },
                { "te", "81808e1d9512320b9112bb22e08cb51b0e35e1d003d6e5c4beb955695cb10829534157ff5d9181d56acbf1a1a6994ec7bf2b94b63dd616a74b110f4c11db9b7e" },
                { "tg", "6b1a7dd36b465dbb23828eb6cb0742af9cab8fbd71141f501eed1c13989a4ec3238bfe9662117ec674e034279b0c4440ee17577e5a923c788500f4744292ae7d" },
                { "th", "0ab77fda8e9b923c9e439121700a6b78be75f76b6d87607cd51dd30352e348c609130a56e34eaf1c6172bd21853f75e5da6e080a8343eee60c1b62e68772fcc0" },
                { "tl", "9ad3045ef348bfb22d516fdb71f04ee07a8af0fd028ffcbd5c59ef804b4873694ba203ae8b6bbe440fcec793ea1de1733350804344f5886e979dafdd59f96249" },
                { "tr", "0930cc9e97a73b44e79351ef4173285901a566b4aadbd3a9828f677cec156c51ba0388385eb430cfcfb35d7a453b45bbc203aaded473610e479dc358a6d79513" },
                { "trs", "5e94b7f58978f5a4fd105f632b6884da9b50c4349a1e702f1d9ff97e543de5e59ebbc95b91e1f3e77f7e3c36d82ea8a82787e4c4335e92f81ac8fd65cbc27978" },
                { "uk", "8b7e4d414881184d9935cf366c02bde6168ea676772b0d868ffd2b673e600983cfe3425712f322af80b3d0aeb36a04636bf18b719d0ca7b8b91c081f627123fc" },
                { "ur", "c7fd473e02803583c9d6dd9b70cb730e5bb435f44d22383ece9f1eb899f417ebf860f7de2848e621b7d7f5d1f9260f7554bc1eebb036e270b982e425eee35dbc" },
                { "uz", "69e7a191e5b4658c741bdefdb98589c067e34e3b99dd9ff6ea565f038e073a3143046a0edaa38283e35261ef70e96c5af478b9fcd146cf178a2028175ce8be84" },
                { "vi", "bf2ecf01ea1dca598d4431bfbd5bd5332cfa009716b68976ba9606116ee3d67d782014773a2434ed2888400f1c1f4f5ed5f192061761cd57a52dc1842d786f3d" },
                { "xh", "178a29a17049edbc2ffcdcf1437079a1483845d8dbded520254f3f006153898468dac212c578faeeda88bc70b99e2ad50fe864233a17841a4da89e58bcde8691" },
                { "zh-CN", "2524b5cb5e810ccb2525c5d3a70fca5dae974160969a1d58cb9567d7be7e4ac163a4e605d9791fa47a57cbbc95708d705867da473812dc0c48b8f27966e3e345" },
                { "zh-TW", "5033b9572468c62f809f0a265ab2e6d3fda191767ae69cf5d21fbce0dddb361b03c63a95e09da36a1dd97845977b40b49a05575b7118f895d02d280d958deed9" }
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
