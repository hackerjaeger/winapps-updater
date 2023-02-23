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
        private const string currentVersion = "111.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b4/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "ed495484b90505546939b47b13ac94c2d855b0c00ac870404a4b766868adff28832c1e93c011c34f498a8b10e80e797d7d6d3d82ba477e93dab94be7d7aa012d" },
                { "af", "359a1f7f0dd23e907c33cf17f1b0a67c0b951abadb698a44e0db9abd8a7a4dce0c4dc327c2106c49544053e4055ccde00c23dc9106b3c9689b653627b8b12f8e" },
                { "an", "3d2653732ffd2c11a2da2d823f95e85298a9d2213cfb2f05b66549cb793ad921fc475f9dd01451ab4a99370feece2d1e1a5941907788455c4a6772bb82c3a06f" },
                { "ar", "49b4f039d2dc644d7bbc87d7b9ea8959d3dd8addeb32d862ef8338194ab6ab8bf544f8058d43fcdf8fec070481e0f7ec351c4e0ab60d217b13de84858481fc4d" },
                { "ast", "b4da4ebd30fb72bff2a5087122c0b78ff76125929892ada486c53d7b8ad1a40eb266451efe9b2206a40cec4679250c389820e6c0f6405a962f5e68cb13efda4b" },
                { "az", "7dea7dcec58ce341b6b3c0c56104ec3aa18d7a0bce6acdea61896c63b44f25a02f0f9bb3a6d72c2c94172d7385523c2edfb1238ab393cd4215c60835e2229030" },
                { "be", "8d4445a8ef92859489467aa8cddd000a925a887237bcfa76f398a671159474631561293e125b3829f8a283e586df80f456c85bd8e8d9d085b3445ee3cd99ab9c" },
                { "bg", "b4d7d7739fa2fabe71cd9199a66132c183089f3be170e78268102e67d4ecc1df216b5605be1a46bd100053f6b6248a297b0def54f6a39461b6fe810fa5d9861b" },
                { "bn", "c43bec7c22bd103dddc9292cc4b98dfea3336c3a79eed4052654289cde073149a2f23ce6fb0cac3b664f1d3061ff2a92703eaf66d9ad06debc432ff0d650b6b2" },
                { "br", "e2b052f7960ce47e78147ef1cff7d983e8d4d9a8e5e5b7be1718fa4fdab13b59e5b4d0a5fb1f059037c83eb2bb008374a34320f7415ee3784412d282da9157eb" },
                { "bs", "2b177e3acf48c86d85a3fa9576ed9566b8af6532a42eeaa0833c0aaabfacccd2d2c948ac48a9d2af2c2e3c2ade886a078679b12149a26f2bc2339443e5b2d60e" },
                { "ca", "946b02b7e86b610ac2b30a830d90c156757b6430c5ec580112a8551c53a5609c40d241ba0d471f9e984c1a7b9775c8fabacc1d1b40ef7d2e0c547ee660f49211" },
                { "cak", "85f4a2be93f43b14462b7eef11c44c931fa92e2e146e3df875850efc060153483b48b9b46b5acd0cf2d728b8e7c3a35434318c1d9fcf916a816da668d4fff282" },
                { "cs", "777d01246efadb11bf4e90c9b2a0e4f97473df2d6966f9c22acb3c9ec4ddb8017e214b76247b815647f27920163e73e8f43ae624e49de6af3b81544f36b480fd" },
                { "cy", "5dfcfb3cad00fca5b1da3d53d4257ff72291f2248f64b4771abd2e85d77d6fafb7f340b2c83c685d824cdc9a2eb61b35749be9cf1bb137d92b36881e2fa48808" },
                { "da", "e9e62150d2cc4565bbb0d2ffbff1dc749f5b2994d413ce8f0022e610c7448ea01329d5389bdff38cfaa1985b72d85b4d9b1651b62d7f8e7b373a74bddeb27f7f" },
                { "de", "f910e99fe6b599554c89351fdcca7a90416ff246096e1c3b29f8028444358b09cae7c850b66e02b43c5c9104006b3c04590725448be38fc30e3f98d410721e81" },
                { "dsb", "a1cb36e2c727d3740c076edb55b1a42576de4a54a62cdc7f9f64417b82f3991773a41ee6e8352c1bf669122e45d456d74fc54ec2ad245ee990a6617940bc64ce" },
                { "el", "8ef272af0c1f48a20e0aab8fbb5b2b4375ecdef190b192ffa268c9467c7df0a2cf2da5e6aa8a84a163c8c04e0f60b5b997f40278bc2d43774d0e72b236c796a5" },
                { "en-CA", "89eeb51a92f0d7e63760497532cb5651b63ec6ce8cc7de96ebe2ecf1b94cb7cc35ac6aae5c3bb551e9d4d45366e3403c63e3dda1b3ad37827428be7e679dfb13" },
                { "en-GB", "fe955c448235e114b53107bda0b0dff07c8a721fb000ccfd0e687669afbdcc2a3a0fb555918f3ae32fab18918a88c3d4c5f62413dcae0718e18b67dfcf764a09" },
                { "en-US", "2faa49808ece81c977f53a72c29437be8ae9f68540d3859c4d5b0ad777d1da99efe021cae1b19c0fa66339b936a3eb6cc2a3a795d2e019e04c4ab7c11271e723" },
                { "eo", "e842d08f11b4079aaf5e9f97747c76eb7ffbcd9d5f400b03af6719f17208b9d54703092c5599a01c360a68824cd549bd453ee08070fed5facec8069ba5c7ef9a" },
                { "es-AR", "a8ed258980369d1f21ae2a21243e0ad9b17c7fb4ad50d4f2891f2d2759526b21a19bb93b3fe5a6a0b2972bf4c9ad8ea74044f0851094f763cac5503d91d99b55" },
                { "es-CL", "58e9ba35dfb77e86e185f4c1f1b022463f787aafa53aed91d87ed8ee892601826cdd7e2c34756151495f75d4de7c72af6c61a83f2afeacab03ef14d0f432e170" },
                { "es-ES", "b2b830a8299570a2f1a5d0c325559b142c61e24702a5c5016ac9947168544ce2a8320e23f1004db5f99e9d356b4f6a79bbcc4e3d4fc0847ce55635c8493f8bd9" },
                { "es-MX", "aa9672fde7d4ebe8d5e0cb01cb0a50d8a5e06078622c55ae73acd58bf5daea4b7612a5dcb059bf1c5a57177654cd7a2cb7ed5f7413797e10e77024ae74b2d5e3" },
                { "et", "27cec51e739f5abcb3b3fc84a4e8fa777cc5ab3831eb407f3b0ba2a0ec21447e6e39b75d514cb337baa2a48b907f931390c89d1c0a769bca0f370e0f20256e1e" },
                { "eu", "75b1861c26863379652c15694ab15bae324e143ef3e96b6356c5d1bddda99eb57dce150b331b4f2b1d2ed6ce97878af3ae6de22ae0c9cc1bbf56f99245157396" },
                { "fa", "e97e8453dcc4f6c624a5baf19f72afb993caa38f04c7437305ac031f53856913824f34bab542d4dfcee747fe36020e6ce2c743745bd1dbdb4786ec5319e05346" },
                { "ff", "f6ad303f93c5bd1a9725adb5cce51be8170dc03a0bdb7d4c470e1694a257db71916b600ec3a620d6292578b3047c5e84cf77e8204b6299c05ffc657c6b157bf5" },
                { "fi", "743ae6987b9686dec1d865898809f6664df910c6ef3bb48a18669767735b223c685db70adfdb7a2ef88f08ea85b3d7e53765fe9a9a6f683ec4f7edcd98cc8b54" },
                { "fr", "0b564884bf8831b1a2ad931c5169b7bec76c7722dbdcfa4548293019919020020237d2e0df848cb8d18096014629c747e0e13ba6f260580c9528796cbeffcb6b" },
                { "fur", "be020f8251463e086b26af9f1771b338032922efeb14184ea5be43dcf28089bc4f77ade3b9f4e55944d0243538d4d88004ab93c7318736b135b21988c0722cf3" },
                { "fy-NL", "09f5c9712656630662845c4bb9e38ede42aaf956e83bcd9e4992020a589bc6496b8ce142a7c889ac15f56c4b43b5641b7506731cc25dc6e9a167eb0ab896abd0" },
                { "ga-IE", "44124f47e77e61b035650528268fba1f3a92845b3951cc64e480aa37a283e78e8e9a9dc13d3b070141806794484ddd919b90adbff0d15f005da1837cfdb6415b" },
                { "gd", "26e2744eb097fc11edfe4656ca7f2e60dee85900b8f623c96cb40c79ff8ee829063eb3249dadc1fe32e8dc6bc3eb12618f9b020da320b54d36534e2c92d2e7a3" },
                { "gl", "de32d6f19c597cbbb9d3125c87ccbddf08d0b78931c712f1999f5aa7ef99018de357e6a0cae1fbfc8f4eb456054733fd5516acb377017bfbac1deaf8dab4fe45" },
                { "gn", "c0b3d87e2076271bf37d48ff81517ddd2413d92fcae6d4ab9275f5c22de6291bb468fac95bc5456c8fedfc6706971a1856d081892f81ed5f1a576190dfee5c6f" },
                { "gu-IN", "ec8ca3e2b0e978820046d5d72f78ea5c0b605522655870434fedc34258224cd0d1e50dafb81d43931fd6b7defce179445577ecd9661fe3a51196e92073db4957" },
                { "he", "a861f85ed25dc41c6bf3dda7d8f135edf382128a5f273040b51be0a4b50b5c7a621188541b8d6e5662c84dd53c4d6e743384520fe92ec11cabb299b26f760ad5" },
                { "hi-IN", "da2085c6b789682f854b48c0e8e6b05cb3302a543fe779820982bce7eea4909693b55ba779bb8081ff24a6db6c53bccc73554ae8cf9e65bee4466ae7f310afb9" },
                { "hr", "c4e22e37ee45682fa3981ec637e5334d70c35bd5a2329d3433a550b8d6ee2ce903aadd09f12652f1ee665614b7a32f26d02cda4dad8b144cd17da76a4ec6d23b" },
                { "hsb", "243aa96225e237648797852829319745d9b8dfe11afc73fd1212095cbe27b586ece257aa11f2386b98b2e1420cca500538fd116509fe208debb78c2a8bedc3bf" },
                { "hu", "ddb55724af82d7acf8d1e4f9107b8b5c6c18c22ff2ac6614315f83d4a04526571eae26a0836b42c085584357719bb95fdd093977bc8b0d0fcaadd52803490178" },
                { "hy-AM", "d8eccae716986438b6aa5051ea7e4bced2536409e47e751e494562866ed8f3464e942815439b129bda567ff93d2d7e1e0b0d71b0de9f46fd98e60e014fe63054" },
                { "ia", "882efea918f6616b451bfee1233a041b8856c090e510440f29b1ec6330cf3525bfc9ecfbb1d65060337d3f2e0ae798367968b078fe22421e0500b0acee049e9c" },
                { "id", "7913cc3b92ec9c6045e83b86fb4e48479088bea71f7fddd652247a572bffb547efab9da9473f1e689536a9d627da9a796b5e8309b7f5e51cf37306fe63a222bb" },
                { "is", "f74b72ff543ecf427e2785ef7fa4d9a0d557adc2568bf6fe49589856fab5ea9f30b3450525067e45e41ee8c6b5af7e97ba359d98abd307a37aa775f82e596544" },
                { "it", "c7ce43da532fd14152e25aaf0a71af5b1db71d45523daea5c2dfa3ca8763a7d2fa29a55ed5cd6bfd65f05d4c71ad2d20341b1b18b7c5ec3e375c3e4b7b0bd687" },
                { "ja", "18c7fc822317f3ddd8b0553442221926e2346897465ef487cac99bd11ad6b55487054ce7092c0cfdc2671c9598fa2378f381a112b57f5f7c8da01c087ea5c02f" },
                { "ka", "007d37b0e8b7001d26e916eddfb5349abfb106db874d58b817b144fd902b1dab9ea7f215eadd079cf7de95e8aa6a56502bad5b447b310613197f086272c64fea" },
                { "kab", "e666b1978496f537252900fc5b24a36beb8b4eca7809e487e9de6c14eba9b69ac004a7a0b220bbb30f421a0f493b5d471345246bb911c87b35d8327edc203498" },
                { "kk", "0d83be9b642a1295bee298994915ac409bd3f07a7c21d55e15695adaab42632e971e51882921fce02d858eff682632609a45b62901552b255f4eef92fbfd7372" },
                { "km", "64c8cb7075a522c5e3de63d0354fd9610c4baed93147066ee78d0c92e43e88ab1d87c90474c71159ff305410550ffa0b1610768f69cda66cbed80b079d17d04b" },
                { "kn", "16cdd38b41964f8537478c0d5aae2fa161a461af0d4de16dc3ec7ffde61637411c664d0525b972172153c6f54675970495a262c4fbb89fcd8caba373cb5e0fba" },
                { "ko", "2af446ad30a84b930d7db4a7a05682f1047d0aa4030e252046b19f73cf4dd315662ad11a9cffec72e8c447276b75cbcf55306b45f411c5417d10f866bc5429d4" },
                { "lij", "f25bbc707822140ad3b7a0d346f390c39f4cb5231ea254fc6b876fa2facd993567a9709a4395a49971034f80f13fe2597ba44fd2ac75d5e26e67a9756860b47e" },
                { "lt", "05260b058e68f46c55c3d129f4c0a0f3d2578b00e1112ea268e68dfd8720ded069bf709811a487ba8d8f81d476d0525f6ed8d8998125016019f8c94d8467ec3d" },
                { "lv", "8b79ff6c06947ae66e4b7e7232690d3d02398f363f87a7b32b74b38f99bc1fae51b7f61742c738118f73933ee706dd3c969a04af135b2abd734837ce4cb517f9" },
                { "mk", "5c10c199bb5d725dd4d8dd8516e1b5fcaf46a819564ecf10f4d5621d56b4886b5a78d3ea636aa37a0518f1d3ae3790cd3b4666e59011aab9b5db68d3bc0f8b63" },
                { "mr", "6b711a462c2c4236e3f1840bb0706557d5e415e6da29d5e5696aa32c0a8912a745191982b755f634885355d4dcfca2866a1006f76061d37a1e18e3a9075f58c9" },
                { "ms", "904246ee6a99bda088fb40dc882844797c993dde054147c1c75a66fc18ac73ab7b6f172eb6c86e055376edd34bc1f112c860529ba058cd9bdf0a6393f6ac5861" },
                { "my", "38b5c33e0c492e64f44c7925c10d53d1c94aa3cba968f2a2865c66f5ad642a3ed69a3a11ee1ec58019c0dc414ce48589d1b400c60804c07eb69234ccf02450c9" },
                { "nb-NO", "ab3d885d15f5fd7457d30078dd2f6c0e848908df22dcafbe28c9d433b59cfa6fda7baebdd3eb498b91781abea39e1a38fbd89ff36ffed51f46fff516d035abb8" },
                { "ne-NP", "eddec7c4ec3a60106432f221cb03604427a6dbcd7188cd635f97e7f16481c799d7251d17b13e35b27495a6bdeed757b4e7cc45886e534436678c5e48fc12e2d3" },
                { "nl", "be26903da3c1f2ebd4fa99d52e8489f964c4b3add287fb67854204a14206e5c666b3fe0026cc924771cde3e42c7ec5c586279ce4573001088f2c71dc77c3e6e4" },
                { "nn-NO", "f58c3589a948cc27eabdebbc896583a4f82453c01e3a32ed97d94c86dc0acc6434aaf5c40d67019686e494de06a7ba23837604e89d2a2180cfb2599929dcc3a8" },
                { "oc", "f4329ce192617120c4a2ba4bf78156f15504bde75595c13c89c285df4660f283716da44a13ca0b22122720ac156378c8ad474ffe5f697606b95a96aa58a81462" },
                { "pa-IN", "14114fa3308a807812a6b4e9659c129f4107b95e805ae7002ba900b4e095a1626bb258e46a89842537857f2de1919a4b820fd1575896d8f70d11f3f0d1883555" },
                { "pl", "d7b335f1641c765deb39c71d4e0e503694745afece836c2b85cd1aa1fb2b721b5bd23272d5af692d33e8155fcf526fb7e9c166ec5eb4f9b2cd3a4ae143556f3a" },
                { "pt-BR", "d0ea46c539719e1303ac9f60346d92c7e50708459f8ab17eda5824d02051c778d4097840e8ad33402d6f60db9deb41af61864c3c5397ea986a14404de5f54635" },
                { "pt-PT", "e902f0acdfdf0548900cddb7bf4ec2de5208bec3a7dbde9dadb84415aa709600e53191a58e34655119c394e4af570fd20df5255e566967dd4626c9e6974fcc70" },
                { "rm", "f4c61a4bab2274566d25a9aa04ea2d9976ea566b8a26ea04c0b938817803a0e05eb1c65879ec43fe265e9eebf1a06329527e3efd0d0032eccd3e5df230a6a9b4" },
                { "ro", "b068e3da98f78a3f48bc3c5fcc0c5a70ca0cda1733132347e082c593a92bcc79c6f66dcce6ed008d9d6e826f2469426058b17030934a428d2fbd2ace74b211bf" },
                { "ru", "c98715d92b5b6b95e4f07f13fbb0d1c73f572be78b776b8524cf10385c50d13635111de343a5ecf141f31211bd574bc143a1c430778f33bdcb4e0e32cb7b0ff7" },
                { "sc", "c3a79ebb15d19cac8b216a6aa6b49cd939d089dd7aa3bfde097bbff13c37dcbcdfd77e51be41f535e47cf025c8761aec2105dea2fc585aad9640aa18af9d633d" },
                { "sco", "98214dbd3346284f59f500bbfb3a7b1df148b0ae12c1ef82cc038efe246368d327ebfe197a4c606923f2cd2bb0d8b787362118c102e5469eeeaa6123bad304c4" },
                { "si", "b02c46d41da2176ff85bc17707f91823441d2c270be16a0932eec5b5b891cb32ccfdd4b4d7147652ea46f512196caa5981c9a2ef7e478fa81f9a4c577fbbab82" },
                { "sk", "ccff1c9330ea6b4bb7827eeb3a22ea04430afe348d583178c656f0cd4fd0d1b5ded2af82228cdbd32cedcc401ff400df679473fe6a5ab3dc908b2dc335288d44" },
                { "sl", "bb3849f14cda1ed8b8714eb0a6d540358811f3c54e905d3f9c1145c92c35b6f47a927d5c6fb89f6dabac3e4e627dafb000c36b03f87e5f58bd6a5f233fe04128" },
                { "son", "62e5a6269c1d8be511b49b6ccca2af096b921f6231c17e2fdd2df87a6c541fe356177e32ae24fed9bc4a1ebeed5e6dda12ff718eaf226dc0c3489acd7ebbe097" },
                { "sq", "da3611b9e31b52c759585590b74a426ab76dbe94a2200df672caecf440fdd6692281d321e01268877839adce02263c772de594560ecd89e9338219192c48bd63" },
                { "sr", "d104f5010c4af625897f1f9fac960a84dec333fe59d0789f8c4245b19f23b995e224ff0c2347c840d960a0ecce0847aaa6b2a33eeedce908f692a04d671c595a" },
                { "sv-SE", "18781aa28cc853e61a4204313f092f20ea8932b172bbf6989bc4dd8d2d3de7b2bc8bdc647662c508552badd347bcb0e11e7d9e405de05d6bcb29bef0c1f90ce5" },
                { "szl", "6b42e89cf1e57e4346c2737ecb0cfb04addf4e8620890e165515386bf8f526691c6f023667c511ecbb2c37a847deb11cf5a844f7f7938a3dc4599391cfda10fd" },
                { "ta", "114cbd3ffd38ef3d3e1fe35ffcabed439d853629113cb4e81299592ed7d2f498b43805e637ca65c1e23fa95fdc19a44c758ea06cbea255ae139f9ee0ab5a6cc7" },
                { "te", "0252d9644b8e41b6092c7fff48ca78d1c58632d4bc4bf903439c8210e85581e9c53bb0bf90d9927ea89d039ca2bb9f74984aabe1c595266e9a1b33b25de66d01" },
                { "th", "1f738da89effd476f6bdcbbbf6b6010fc7030f621ba105021cd54313de4faed4829c1e62a242d3df756ade0370b7c2f8acfdea5fa9dd384f15e31ff814216d94" },
                { "tl", "1887b958fc67c6eebb556b91598916aeda6ba2fc0d52b0ea73385342845b58e70518156255165b1273e5150b49f7b9376550cfc8a040b3c65079f96de9cbfc30" },
                { "tr", "89843b2baeedfe0a5fad6a7e18f2386418cf512f25a93468e57cd80d896362e9a9dab89f4daa36d44185f9e609ca6ba9927eb7b9fd64bdc5a57e88ead3a16705" },
                { "trs", "d7f2d3135a7e7d5538273769cf12b79c319080882db2ce573a02694a0eab31ba51749bce73414f069519dbfc1da4f01194a968ccd4aeaa376454151cda03cf4d" },
                { "uk", "86e7c3143f73958dfc2f1714d5fca528f5503651f6c945f6c91e13c285ce800a4c1cb94f084bcde1ad67ea864ee6ee875d631eb2d513dc1a3e59b70996ea665d" },
                { "ur", "ad3e7dc68668378bc27a96134d4e8b7d169ce86a0d85759b2fa9213e76209c54a12dc70e8848a0855666b806f852fc26faa48ecfcc8c5c2df6397986937e04a7" },
                { "uz", "0fb4c93ece4c9b014046f2358da24b971d382c4f60540249c6b25469d7c39b9263a68c2141acb3b9f60b2c7f98473f286a03a5b55b1a0b23d0865bc7870d2df1" },
                { "vi", "ed9de2ab455c863c84e886d0b6ebcb4f10fafa9ee99c168f43e3d61d403b2f58bdf07f299e620ac91a44e96a450dbf0e7e19dee0cff446f14f7fd11da514bfcf" },
                { "xh", "36894e707de614a209c1800624c7517cbfcfc7a462892dc64d2a2bca1a99047287ad5cf7ee6db09266627de29fed84db737ca7a3af1c1aaa7bbaa75c202d7a5b" },
                { "zh-CN", "499d55db78d0e6277290e05706dd4f0f597f73125ab8214ec86056763d6b64955fc80911c3d2924e8bd94f591488dbb4216509db189b437673b74c9e4966b4f1" },
                { "zh-TW", "8f7cb274c0997b90fd6e493cb803bcb235c846e6f9fd23c23a495c9e37c49e9202acb925fb9980424fe5d6a24eeb00c887b987df249006e25bb47ad194ad9b82" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b4/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "2f28e7a26f8f7b1f9c1e24d110f44e71e9bc04c5d05c603dd4fd358385bdc06bf884eac6e063d5d93a30de2da58626ed1d66eb289231c33ff0ce1b8d82b67cb4" },
                { "af", "f0f05909b6f2ff1556f0f7fb7e01377870fa61c503124dc699f89489d86ffa7b4dd0b62ecbef08cc4b7f789e3e4b71108bf1f54d26e86c389e687a370b82818a" },
                { "an", "4ea95b69188c53867c1a9dc18982c620cd49cd3ab3cdc211e4e8084ecb0a1fc14d13d3cfec04998f9393b4f85b801718892f35b4d700c46a868ab5737f631989" },
                { "ar", "4b5df9734c42e4effbd7fd59ca1ad084a8c164edae17fd1f0c0b2d0a4196894d4f26fa23be6caa855f687c7563571e95815461588639b4f40b293d181bc28cc2" },
                { "ast", "1f27c810067eeb1b8bbf84b40d84e6a8324309243b5e8e6acd415144818c47695e17875a35be3cea0e5156319596c8277933c4b0ec585cbfda4b68bf5fd5c23d" },
                { "az", "27fd15717c2d38d61a12aa8fbd6116d09b91a1b38646c2c95a5367da8e148c90803fae3593208b5d0b92fe3bdb0fd0d78eb0da7469a505707446b6ce308ffa57" },
                { "be", "fb30937b42b2d4865ae71f415d0011a2718574f1f7d1fbfe8bdb7998d8045545190c2cf2ef2c88412329007e2176dce6bc33c68ea4ac8c25dc35d9ed98a338a2" },
                { "bg", "21923a8775813b04919c1c8ca670b3d356afeba91badff2087dcfffc22e6d9efcbbe913a227f437c764351b3ff8b4d58d0e88e7e7c1636490c34235992eec501" },
                { "bn", "b0f6ace9b9118ebc3282d841e5bb801880f6d37ead92821db461d23bbc23b0b5e80526209168fc2fc39aeabcab9d4e33b1dabc8e6527a305ee0ee80fd84274c4" },
                { "br", "1902619755748f08429f8c78469a3fc8e19529d8e8f75084332d7205c8b085b4c3341551759fe229bbdb206bf7475d82a6ac6c6e8a9e837a3be0e2fb1075bf7e" },
                { "bs", "e0010ac9fce47a8396dd93467ed4bfe45fc6dd2d57c21d232f7afbc0b79c811a54955cfa7bda96dce4d746bc181b3278fd053e444c95726ffd6765fe61b59726" },
                { "ca", "ba57a5d203c8ec6fb0c5cc498b3fc56853b1f8177ee94b8c1054cc60d9038f1cc5070a8c79643156693b2955a899b06efedaad3a9667d4b77567923783c3c57c" },
                { "cak", "e42908fbec50b807eb9eeea1f9366eb7778bf10e3e3db32d7b1baa356d4fc2875c03ebc3ebea8443a62ecd875c1e0f43affe0cf10bbd7e9fcae49a947e016aef" },
                { "cs", "a3a554d459c448f2c5e1386496a6022c0ae3f6ac3f26dc385e13b1c1678c8ae6cffaeddb1b2fda61dd097dfd07bddea284bdf574b6325b3ae10ef52212d901cf" },
                { "cy", "e6e82c139769efc92d65d0c6f9a02ae4bcdb9b7010b58b250592746f362cd592f68936eaf8991934574f43bf8ccac6605700b51556c5c33649ff91c8022633e7" },
                { "da", "118dedde5bbdeeff5cc2db27237d0d0b5d3088f192827fbc3deab9ed18cd21501d9f2ae89d697f495e556dc66bb986ee17faf82a979fc448a2a23adbd55ee6ec" },
                { "de", "e860f07a6d2105123dd4207638f0db43a2e8fd5054e9d0e51de43c911d6b005c45378336e5d407ba461d81c6fc65265eedd42e77ebd0fe8550546ee44ff4312b" },
                { "dsb", "2f0511a6207c212e419d20312ae3053a93ae912ef042fccec9f85ff137826f3c132a58c436c3f4e1ff2ec7a05a97ef72bb46b62490557a58bd118262bf6f41ed" },
                { "el", "72be0372c3d6f5eb11eedef6e9a3c6d07af20e6995f90bec3d8d79a697385f1e0aca63af9d2e1d61425ac9b363a2da3d8d316574480aac66e7ae85b1f971220f" },
                { "en-CA", "f0fe75f0f50a4ac4b8f921e4ccc32ed9ec294fbf68d4a483f08f9431e1e1e65b4179c2f48fe9e5b2ce6203f71eb24dd85c6b44451c41b8d3574318c11eeae6e3" },
                { "en-GB", "3e3334017982298c30a6e3b6b67c19d0c03889e41e681fd5a9f8cde5d4ca20b24dc24674dc3878c56be858948f446428c50f98beaa6f311161b9b00d70799a42" },
                { "en-US", "50bf80e2391f5eed400e68d34bce0576a24fb56bde3230ef960ff34ee4ee8632fee4fadc4523c3c6a75acb42e9cddcb0321fc37114cb01d68a7dfa417af60a97" },
                { "eo", "f765a50b30e04c830a4abd855ba84316c1a61f7494d8d674cb8c73bf6c1f7ddb665014276c7806edcee3582c0a52e4d77e424c07d51bee66d9f709ca46d20cd9" },
                { "es-AR", "d5bfae65666b42ebcf160194090e01fa647e87ae8be3ee42f570dc55c107e8e4683da2c8e849bd86e5f6f78344c37c78d5a003a3d15edd835205d8a01395e38e" },
                { "es-CL", "9694cca39a45771444e062f221fce0b4f2639444c5b79bbf585d72c7115c03906b35edc3b1b4be390bc592d8f7ff461e7e63994fab1bf000894dd21f4d84da8e" },
                { "es-ES", "a8b1df185ac8f276997a67dd02ba1abd46ef90b286aebf66800d3dc65e249bf864bf7ce0d9a0f60ce096e7c25a1049893890572ae29f17fc40a93aa58e7cd285" },
                { "es-MX", "8de1552223777cd4cc75444cb442c8de2ed42cf5fd9b3eca41365fd0ef8125369478a2e137d7ac11f0c51e782ad04f091aa8bcf6e22c9bf4a8a16edab3f402c9" },
                { "et", "02d0ebb940f49fb9884f59e38049935b2a8bca47383b03790eb17241c1f902c5246fde3b0a3325e6ba86e4208b25cbe907e920afd3c70f2d4eb352245e57a68f" },
                { "eu", "b70a730007a3d7719c3ef803a0ef622fc58f279b9e0a68af92399a5db34969bb7a2a840b5883e4f13acf22bd33f11b5538b763388228d10dbb6729af03970c5a" },
                { "fa", "10047b7b987f58d9a6b1e7f34ea2548916029123a03322c80e4bf24182a44da5eb4eeed7b79554cc6f99245cea3fd29e3ad39f601c66ea4103a25109ec6c42cd" },
                { "ff", "477ed5796edd6dd4fa2977b2f43411fec1215e6ba0c506ea6fc9d6a3974aa19a4b13729202aeda1e4fbcfa6e30d815c6889b7a2c1655595b39d3b5fd04e88edb" },
                { "fi", "8440e94adc84c8709435d962b5dfc623af13792ca1887b517a83b02b132ceb60849b27b39e80d36053727886b9c0a74051d619a59a987fe2a3101e7e0e741bf7" },
                { "fr", "71fb8392ac5b73c54f6c588963ee4ccc0c22b7f33b623aceaa6bc60cecd5b2c7b8504c138c5a8ddbd1003b26f0520f76f41b409f4882bd53f35e3868c23befba" },
                { "fur", "a5d585a084374e2351abff4e9dc725dab041c2a471bb635925be603cafe90d5fe42b2312bce9af081b29a117c2a194708498e18d019d6bf9d86f697aaf3826db" },
                { "fy-NL", "1c4668cda06f3a4c25d8d8de9d1cd94427500ad58a54021c5fff260d8208f27eca287293607d89a25a12faaab57ccb3885457554fc09206ef9efab17f716da95" },
                { "ga-IE", "439c57f1377c4b39bcaa51d9238548984764f4ace7a227adce3ec285556ea33a3efa66e2da1659215c1b613815bed34a3783f758da670f667096489cbb2eb2c3" },
                { "gd", "055da18a6ec5abf45eda8179178dc44de030236bb1e4e20150a4109be2d5aa971dcdbf736c0f0f063ea84888197f1c40611d3dc218ba9b81be9928aee0660fa6" },
                { "gl", "82e76e0c99bccfbec19d932ecb5a8567956ea84c7e810e7405758eb3e0c106b95153b5a0bbf8f6d71d210af819db6e4d2f8eb949742497e83b95129e24b5d034" },
                { "gn", "b7543ae73d5edda2b12b02e4a0ee795f7adc581f6287927d8f299cfa3bdd20bd19adf13071bbb84a0e9b6d5f1c56520499edcfd4ed308000185d01ffdb8ac0e6" },
                { "gu-IN", "474cd76c8fe4bea406eba736e6caf6dfe128017da4e88647cbbd68cce3c0d1b1dba16704e047144588434a031b9c29b1f99492813f2c1d1cae17dab68dca79c7" },
                { "he", "2e5371310a9e5ccf331b4408b816cd52f3b068d8e54f600636a7d0f637497cb77c5a0c7725704a128e3f38098af802e56fe518acbf3b7c57afd12b3656c45149" },
                { "hi-IN", "2bdf06bbf425fc8318085ae7ddf70053d861dd79cd3052251f1be0899a818a53379bc98e39a1a05fc137a502c1650f292c4a5e0e080cad942fa1d5d3e7d30863" },
                { "hr", "b361bc78d2141765c4cef615d80f275d5a7d77740e4741fe019eb36a26536398352e9603c50633b68d21d6764a729d0ef53effb4c10972ff75f6a45f4c6a455d" },
                { "hsb", "cc76d77cf5289d9f016c5713f8cd7aa65ed8e090275600d3a66c47ab7e179f7f119dace182d73e75b7da191bb02dd6559f2388f9d37ef158fe0fb7155fa33bef" },
                { "hu", "717b3d605f1b3d231d0ab072f7d41bd97240c5f0e4c86304cdb390810b0fb5e67c82818dbcad6d52308d632e38afb6bed4a29758017c89696a20b7f10b6ab719" },
                { "hy-AM", "5f0f3411c15a274d9baaf2e520a7a5be8c6800a51928c8a73c3affc5d04f8d12865fa983b158151e1b08e7b4188b4dce1d802da703f4a52358da15ca1afccab5" },
                { "ia", "ace160b5a1f9c623943acfb453f0c29f41d455749f5c3305e0dacc3a8fc952ebecf326fa4ab5438434c1ef8ae04157ea932720b91455642d88f05b52abc5c467" },
                { "id", "530164f8f1538c10cb6789ba91deb3e8bd1586bf4a8f61a4b586dbe7ebf7ed8736e57fd130b01b8b8f67ed7973e2a4583a09bba2fa514d9b207c5929af40529e" },
                { "is", "ae9d4191235ea040a7cea1bd5eb9b2a2718394eb528858c492c01f947b24a5dfaa7026b1fecd5895db1548172cb4734b5521d21c7061960baf8350e535a7228f" },
                { "it", "149090230c196789e252efac3f6c3c73d204a1bf132f920a33019ca69d75bba1b1b95220e9c29fb7a75a8a4d3a68a784c189f8d4d38edd26571dfca0cbaa9898" },
                { "ja", "3abbf94a0e3abdc3db58cfa9cbc62bee223c63bbfeda34af42ebfcd46888f542b7a8d2d1d5a1cbbedf2c2cd7502287b1dc94cee4e14927d0d7be5053b8489ba9" },
                { "ka", "73cd413095e03465283e914428789a82dc740a6edc549d986512cdbbc57929afd982114115974d7da1d1f5a3d82f3b6227e596a41570706a3f7fa977ceb232dc" },
                { "kab", "46800ffc7231a13ac95dd83e10eeb7472d58ab369f1e74ca3f5007720cc1cb0261ecb682b4d5ac1e86628c894c74e216c7933b59cfbe969d168cf53fcf5bd505" },
                { "kk", "fd7754ee8e349a2ac51bffd2ee6c2c19b29d1fe588e5787e9646e1be3a00d6f84f09856a1a24d1cda4078e935ca9877653e53a74617fc377a6ae1cf4b50aad8c" },
                { "km", "86340c76b2ed84ff77bd41d00254f4c8fde0cb34d217e0724fee79bf3d3fea8a192b2d81cc126f1aae703c7414f6672f706ec9ee5fe2c44192b02c4fd9ef6b17" },
                { "kn", "d7477c2084109a52c6497b00d639f2021df4fccf87e8c78aa32aa4a7122db3930fe8b4ee190cc61c83f325e9458f704ea85067de58a3a1567aceab1103b915a9" },
                { "ko", "65efac3d1b938b8352f813bc552e45ed04e7695fdd6fa95d78510f1d208cb0440c2981331f015ee30fa91c416402db626a4654e0c9a56f4f78a91a0e64ce5fd4" },
                { "lij", "6d013907b233ce737c218dbb8f4dbb9dd39544c8bde56d5207219bc2efa239d6ba35f6ecf601acaa49cd3cfdd740cd38126eb07a677bf9c86424b4fb584106b9" },
                { "lt", "02b622d5c2f7d86ca0ddd3b14de7c9be4749ee8340f543527c03f531756f0902528c7179bb5a891fb78583688d63e62ecd162079052ada06e8cf41cabeb6c104" },
                { "lv", "4f12ceb7014fdb5447d3a18562ee0b8ba7cc77f95860c63cbaa7ab6c4a2a7358a16106dcb05468a0bf4e2d76ed79661ae3b317f1377d7ec361cbf6804202227f" },
                { "mk", "0cd603acd73cf36121d53966c14df3d931a33f8fe9a171b0b73229ce7c07788db1f7502d89f0739a17f4288d7dae59ccb1c5bf4d95bb11f3412df5359b7b867c" },
                { "mr", "fb885bb537dfc162321928457dd0dee959619cdb42d804133eeacaf1e205885ef5f06696998fd96b4e2d6c881e1152d2553a86fbb6479635c43b544abcb62c67" },
                { "ms", "38af039045d4b2270b562c177089d6dff1bf5359c8122ae3fd5e8a03229e9637364e80a472ec1923c4217121c3ba015e5d895b566481d283e26c408fa9a1a948" },
                { "my", "b3c3d2617693229d9f08f9fa926be3c08bbacc83e85b3cf0b0bd5ee7ef8df0bb339b8da56ec0a1b176b17b31d671122b3b661fd9c19dd60945a3c423fdd62efb" },
                { "nb-NO", "9292bf712ef313dca65669417d5449582c0aafa0ce35d6f00f3e7ae471c2556818eee26e2c25a60e80f4c269fe09dbd684dd496f9586af575828f0237970e7e7" },
                { "ne-NP", "7fe2f0394dd4b5de1291b15ef0812c2c34cd1619a3159bbbc95bc37cc56795ff49b32a9af664ef755bede889c0a50505b3cc4073af8a95e394242fca101744c5" },
                { "nl", "9e1b1bbf4fe22c023194f23cc8ee3afed1fac041a33a7b5f216e6fbb1a6eb73d39ad8b866615cc8cad777e8aa102b05df7b18a49f97c8cbc0c676bc9381d879f" },
                { "nn-NO", "4cc012d5e11d5b3c685f35dd6184e69a544e321f7df907b88d9a3683394f19b6ac255c97e1bafea4285a40b9640c8aef47bbee44705670d116f824263823ede0" },
                { "oc", "b59e19a63913a75038354d42887338bbd309db788591366bb2a1f8b0cc83cdfa51234ea47ebd3aa12ebbbe62ef05d6e0eacbd2017b1a01c5f799dea3ce4370f0" },
                { "pa-IN", "5741edd005ae974d2a2d7e76f9af472dfdea4ef7cf60b2e099de589bd3ec2e1468a3f3be2000c64cbfccd7e135a723206825373be1a99fef0e38f40821266caf" },
                { "pl", "179a76cec1cca61b2c5830343e36d82aefc5115ec320f73594cc63998d5dba0c1ea5f95009ed9ac58f5305ac7c63d117e78188bd4820d061fc0a40dcd0dca0ea" },
                { "pt-BR", "4d3b5f89ed1f97d9784380f21b3972b033ced66574fe2f1181fca61a388409e7b3d5ef3f55b4c73dac9d6fd6ba234960f8e88350d767c95d53d3bf9dc5cf5b92" },
                { "pt-PT", "50e2284bf444bbe33616c7a0965ce2423602621d43dad6c7b363753e38febd2b651554238f5cb523a8b745027d19d3e1a8880e79e850ff26c10b63d1353a141f" },
                { "rm", "d50c9dc94975462806325e8d90abc56a47ec482b731835eff3c10c4433df80bc7137bec3c394d1d7c4d9703b1365acbd11602d0f78da4c097cdad26d1327a66f" },
                { "ro", "ce55f140c7be34639d0dc45a6975a5970f4f497a17e27749d8ed3f654f514c9faf777d357bddbd660ec7f415a537654c6eae546eefa5be544825a3217051f5a6" },
                { "ru", "f854c97ef981c38135c795ce8a8a436892ee2d99f5ba157dbde35ecf30055ca4a8d06af74b1dee7c1e7ebe3582ce1956f1bd358ceb2330132cac0d5b1135068e" },
                { "sc", "e4f6d0da19feeeee0a3029ac220a860b9119f9c6d84ee919480cb74327aad1b98616d0d7310f965e5d9c7e94688d7387548acb21e57d9f5bdf54f1e6cce07f5c" },
                { "sco", "cf68e2542b7637dd1059ed233c7819f1a37bf5300120d094d11b059f8aadec1c74b116ad9a4338504d025554668ea6a4e713d0c2322d2c502cd6c4a5b7a8ae0b" },
                { "si", "294d9f65c3297e7c3ec6646adbcf91857044f8bdf7d5897e3305df8b8856f116120107dde953d1acaf12257ee1fe3afa952ec666d507ac4ca9c526016400acfd" },
                { "sk", "86ee9982f7c2d8389be58692621e3ba4dac355a32c07cf971a8b86620768e1a69b7b702c8edb2742d511d15102b79ac9ce84a46d2485fd5727e46222b8c4b9a0" },
                { "sl", "9c5c4c3083539b4590cdba3bfcbd5d2abbffbbe787227ed34bc69ca587b2d42e0c2e83172566f139dd92f0a13e546321d3d40b611b66922a10e3d41912c5702e" },
                { "son", "b396040363f37458bf1239e370c42279e52f4717a4b130bae785da6d0ee716a7d777c46a712170066e04da0b394108f34464562914cbb0f07b8a0c1f602077be" },
                { "sq", "c5ab5acf65669f5ccaae717e4b36190d221e9d012da9de898201df50e056395b4cf3b759e5fd47b1dce535de7247882cac5191c2737d137a38cba7538a9be92a" },
                { "sr", "25d611dab26b852edf93595a3734a7a2c55a2930dbee0f262da277bdc9312dbacef14e1506bed80ce26ca8cf5e4826067af4316df3560f3c7fb44ff66c4baae6" },
                { "sv-SE", "4a576a2ac06511386ff63ea19ea82bed7ca9b0a0e9f1e501b64cd608caf3a7b674a127725ffc0e7bc8f626c4cf6fcf65dfe6f115f801da5792cbf63f6cb0e480" },
                { "szl", "36e255803c335df5399d52b798ea68e4e1879978e75c7e34fde52a92dfcd3eb4ee091c69acd6e478d47cad4ea39c6eed472690454db2fd8fa5c35c7d9f0fde95" },
                { "ta", "97fc8716fb4140cddcf5d0ccb35c08d73b8c024be9f0acea2a1d1685fbb5a9502969530e387b22d7b36f9bd9bc26a32c5474aa838d0645ac1de2c0725eb3ac56" },
                { "te", "97b6ec4f4fc4271c7dc6a704a29ad2a273805e6d704cb61d5f03a9f8901e06354f03228d2532b60b69ff05fa0c512cdfbd6dbc5451dd4aeac0d3a04f9ae3532b" },
                { "th", "3b484413b9f7886e52622fa34113fad3c1bbe23f8c09fdce117684c4d89cee67313f028289e1c87a3884ad0031c883dc55f408a19100998beb17c81263323d04" },
                { "tl", "1454ffcbf5531544da573ee6765400e0548e2f03627e98e184df00c41bab88dc280b6afa46109c196348d490ae8751da2a105483294efba2f9474f11d0461a7f" },
                { "tr", "ba9619bb1e9a20fff5b2080997eb658efe698ef4597dfb7a366d0a14eca922ae0eb8920688af53ebfc5ec8b2e2f75c26cd3844a17398c3a38a03eeeaf9584188" },
                { "trs", "6b1c03dcdae2a5cd7200e10c005fde89756cf8c6b806148e8a207ae2a710271e3f46c113ba1b01bfb2e230125d8e43933b6eff4e9f8709af47bb484396ca57a5" },
                { "uk", "48699bc6b1553a38a961616fe496ae531a53fad0ddda9976b467e65d71bd8e3ef79d846e19d2a3ba22c5dfd2867686adc560f9f9dff564b0a6fb2faf2f6f53ef" },
                { "ur", "4ce32e543b25488e28d78427f898f4bc4ab3dd0f096f324374c21f53f3294809f41997c0414c5f7fa20f4def34a41108782794f21006b03aed2e72d98563ea23" },
                { "uz", "5ca393b3a9e1ef6afeef0b7ed71f7cc7d7b82a2136f13d6aaab2baf113988849faf1559f0afa246eccc4fcb21c33d156853f628b813ef2bbaf8297f8759c50cf" },
                { "vi", "8a6534853266065a6499bab17dab450f7540cdb8568eec4e8a07f03269f150b31d546495c75ce8592e3644f3a247778af4c29e7d0bba497948b1babd99903d06" },
                { "xh", "384a839a11fa8498544c5f95220f5c03e7feb7b91b6b258e20f89a946fd4f6283b06e3b85b86fb00d1f525d343a41f53ad1724a66c3709375fed496e46c082a0" },
                { "zh-CN", "762718001ab05d962c48791481e23b22c3ca43440a782035cd20a9894605a890a054d1e9065fb570f6cedce337629f58c26854c681f8a71d85df86816869a031" },
                { "zh-TW", "70e2dae68b5dec0c518fb2df6f4f22eb48c62a30fb03a4b0bc7ce79c4bceff83c57eba418dc9e973794ed15904c025aaac8e704c66570ff2e9c48251fcc20742" }
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
