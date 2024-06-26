﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "128.0b8";

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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2c2955f85423a8b49ff817f5fc472e207e01aff68cb073425756fae787bdc66d4bef9707667a038c0edc7974ede721df5912f166e178f506f5468b9b6a8f943b" },
                { "af", "6c5f0189183f30110c71d0ac38a45782ff5fa91fb46954b431ecc96a1b7b16d32651f3dc20a384894b3b97f13a8b04ebae25aa008ee4045b982d21c6311e2116" },
                { "an", "5e739ad9c8148ca11bc537ddd649a0cbbb8b191fd1cdd32539ef2fec1c3733cb981e1fb2a8bdc735b71ca81ba908c37cc776f939c244b121d5811e4377509c41" },
                { "ar", "b8ffdbb7da34454fde8492e3251f0d8a66187a9bc96e2a171ffdcdd9c1a8ec3c1042db755f26897a6855c43c8578f8f89eb07ed3c5696b2e331ead6dce0c1844" },
                { "ast", "52da52820056863550aa635e4eafd2900f6f06585ceab2522c55d2cc230de1a2f7c756359533eea87901f3514fad5454bad30c60f7f70af8575b654b4efab217" },
                { "az", "2aba90277f5fa023e4a654a7816a932c6534eb49801c5c6cbc596672abb51581a191f9c08ef1272e3be24628dcb81c967faa213df393c5314a570fbe69652beb" },
                { "be", "29055862e67cca6bc1d2c294519686b67cd68b50e6c1409949911dd1f0ed13ca5fb8d21fa77c5d9e864367b1e25b819581bc648f70f4c65129e7ba35be9aed85" },
                { "bg", "c852b2fd2c462a0d9593ea336130b1cc69429999121846d3ffe540b6d90793c708d241b86b940e045161b654eb81d2c97dc1da4f39850edd9bcc416473df3cab" },
                { "bn", "95f659e7c174c245bfbf4404f4ee20b951a4ee3e493958417a1e48ec00c9051653cd29e93bba52f75924c308c6e157f834d7b382c443302f886629e5be7d72d0" },
                { "br", "586e37d0a8d414370062181c774c73de945a13619d71e9702c43ec43c3383ab96ee9518d63cd381f0b917e9ae20282059494780c340045ad1cc2812064aa801a" },
                { "bs", "44253eb9c7da6cab2c26ff38129b1fda4f0dd2fdebcdf4dca41755709166988c9e9f29cb72fb646210c2723f643c4ebf1a0324051d57a85af6a9cb741accbd61" },
                { "ca", "f7d0c8a46b342d7140b44711de21375fd7c24e8e2d70328fd22bff875525a13bbeee1f16293e4c8306ae3845b897177a191d04cfc27758f3408e008dcaa80689" },
                { "cak", "4f8808048d8356f374012425209959e2c39e52af60e42ec11ea36590e512353629a7bafd5bcd072d075fe05a1fe04d1f50669a6cbce0caa9b7a0913c8c978ffc" },
                { "cs", "130d111fb61e4efe9949c1eecffcecda99b1546a7a9a793087b90e5517a196a6a627d3abf5b0ec36ca49f2c46a5612936b417e27981331a40d4d48c95a9767a7" },
                { "cy", "ff4a38c0ad75f674d2f5e14ea1c3c7a3b49719131796a98c2352361a3bbb607cb5746e4368f1ce1c6fc6b498fe395417f6040a398078d521cbc2ea7b9bd173ab" },
                { "da", "d216e310c78d249ce4a467370b5288e8ba1391c45bec28fa0b0d523a9b56ee0b21afd42f6cc194f02f51d869c496dcaece3195c69f557ff5f72e9be622a68d0a" },
                { "de", "53e4cdc03da8111bc80d6088fc26fd75475144423307e1447debe7d69888af84c10c5289201fd8dba5a85b9f9bcd98c97002b1bcd37e2b81a76a2a0cc356efef" },
                { "dsb", "ff38ba1dc14d236d78c40dcaa47ce46d3cc7573c73999e6bfcb2c01849da9af0c89520032dad8eeaaedf1c13ac51bdc3033f1ffc6493a13af6ad6331e360af04" },
                { "el", "c0452e513d28fa1173e1adc31f2648797f54f783600f9f685686a3308ff6b1d1282496986db4ef0d9143c9875494299aec158ab3db6a3b50aba637f7b32182b8" },
                { "en-CA", "e6d6721e79134016bd7d74a2789425f8ede9ad54e6a5007598a23b491b3fdf3fdc642b4fb18e26e66ae83a77b64c58e2fd2d6cfdfdc2e5f545fb2e9bce926a71" },
                { "en-GB", "59157b9167164ade7d23177e43316076f51df07564e084644c7692333cce4851eec7e1d858dc4df6079a5d39f22a8b3b89e8821ae3babd51052a4014e15ba6c0" },
                { "en-US", "40cdb8a11c30c9c7e9c7115c62d233c4424e089d11849075e650d2a6573316a6f7e7ca05ae37d7f142f8ac0f4e46328a554c1051a004a464666f9c960a577011" },
                { "eo", "c77c786f44c82867945c20d3e045e3fc26849752472207f1aaf90696247c500053d79d1467aaa4e03a925fb067ec0216bf5db27c04ade841eac52b907a809085" },
                { "es-AR", "e37dda441a0fac5fa146fd2d71d84619822fe1dc91d8012656c0e67cde6b6b39388524f7064304213ae9516b407692d8ade80595c304372f2a21fd9281ad1f8a" },
                { "es-CL", "0db89e2b9be36ef1c41af6502e9babb1e656bfd59ad77ea0e65abd18f47f2de56e1c06ca0a1543ed55721b169240590cfd9bde223001cddd75887e1376346595" },
                { "es-ES", "a46dde92254a936315973b4d0befbf9c3daaeb24f7aec0d675890c85d42c1fb207f98968e7d15d81e005b978720e77d0ca661ef442f501405c9fd69a690f5895" },
                { "es-MX", "0444a441f34f3f8236af06d62d272ce60e5d89a3ced756cd198427a019b2da9de2f4609a194d8d0eb65b7e4fb80d377ef00da0a47f99d02e8821c8fc95d09b5a" },
                { "et", "0ecb14544f2b52063ea35e042470a1876f6c0ad85274fd94f829e4f64bc7f696e33d1f06f8352f2f36b76643dbc0394f4c897bf0575ac39ae5f493b8bb2e7ecb" },
                { "eu", "988ae65aef3219a7a696b1b9daab0221b36d2706bb47e6604ebf0aae599b090633e814e6e61e5f0a9f37aaa8f6e0d3a042a890794068d41b8fe86a4d3c0b5310" },
                { "fa", "12d53f1a76efcf1f462052008c47be20c48220c780af140cdaf4f2ab348c58491d6d1782265c3835334f37d114e622be90dd9acb33218ad3d9016e61e65cf364" },
                { "ff", "135296136fbc2bbd48641dc30de48ccba6a5de6ba2214317be4513412388a7a8907c023867c85531fb349f8aac8436e8d3619fc79e41cbac7b77a4dece846789" },
                { "fi", "d98ff5656806442581c9b89373525af28d6fdd7690399dd3cd1620e2c1b3360f8d1e65e29b24782de2178496359abcebf8589376fd960937931695a11ed56c29" },
                { "fr", "d3146b2917ba00816b6114499d3f3775e7458fb2436ef4d5f433732dff8d746a09a8448c6d1c9d4c8e995456c6ecd2f2a8ff90dec3a5c3ad6e5ce2994d6bdb8b" },
                { "fur", "6135ab097c078ed5026f1a7f1c075ee14bd783c29f06efcd003471ace82baaf56824b2164aeae54fa1d2cbc2fa5363f151bbd83d0531c923ce0f8e6d1a4a2371" },
                { "fy-NL", "9fe453f26aa2a83535dcde26a5fb350335740bfcb41429a986efe3691e1b14185b15ebad23f82f5e26cd95a6bccdbe60e9b2299ee0d32af58222e08b2b810330" },
                { "ga-IE", "8662d8432f6d0ae7230d9a0f531dc7a2ce2185f2695414cdcf9561034584e50571c70e73c7b478b634db2dbe010a9560ec9b2d79a0473814f01ed346974c5cbf" },
                { "gd", "64ef87010ced5c32f1598b55e6941d5e0d27e9475c6aa997350a8917a27eb421cc64055995fa93081710c04086d8a4f8b26016551b10f508b60f4f0481347942" },
                { "gl", "2b0e595d901b3d36ef714eff0b768c0aca31a9dd09cec188fe06154490e004d13c1206f6d785311908b60fa17a7309aafce37543936664e418f5df0b5f3e9ce6" },
                { "gn", "b1ba4ffd83f3e1e82fa06fc99ccae9e749c8adabca94f1e9defd9a973863a909a134c77d275ab0c46411943ca4850c88fb2c68a6e264c091e5b7c0672a3c9d23" },
                { "gu-IN", "c526038364517b47cdf9f58c7ab6809474c9b7d9655f9cd03fa86bb6cdcc9e0f756ea2eeee52ea85955dc6862c9522f6d09bbc7df5ca80a0dec6c47f1682a0e6" },
                { "he", "8c32613b8f6138186566bd077980188a8ec7100c685380f2b9c3b466e6bc3e479daa63d9eddbfab14a2fb6bea1c0c1d384ef4d314a15f57bb64ac3556f5ef79e" },
                { "hi-IN", "5bae34946577cc387d8d162580217dfe7ed95f05c4c15b073b7ca6a4cd9dc5190c5b95ea0cb88a74a5020bef0433e35094887f0cd55fe282702534fa16817c80" },
                { "hr", "3388358da3c7d96a0adaf570cc8c41b56cd7a6a9b70665fad0925ee2174f91d2bf87cbc729fb8d0fce0bf379d08e33ce4e91d3014b26993fcb9ae4e3d1186595" },
                { "hsb", "bc2bccf44efcf4223713248f20f5f013cfe1e6510ac6f9c1a6ff77fa67e31c4884a8b36b7a8a72e21e1b18381b2e9dc555375a7b2b7301c58dbf442c2a81f61e" },
                { "hu", "cff4ddb964a9d1f8455ba35e8505b58db02f4830278e3818dd30274c2cf0e186399469a4fd873b387a7fd46fa2e490ea3e2178877b5d781454250f07d09e649e" },
                { "hy-AM", "537fa7deb211e142f2d0e328f4da8c7035b263561e8a84e9c80af10e0463a03a0e228318987b3867a48e39b3b495b85659a3d504b6c8b34bced9b7b1dcc25156" },
                { "ia", "113506e260f11faa8601f5d096163675cb1422c3c4cce18c889b309be43b2aa1fdc13732521490c9267d867d40d183031db88d81902fba6b4b05f4516e47de2b" },
                { "id", "1d5a7b29fc3b70a7758bd44048579382a3e14703e2b022eb1e2240baf5c8e405e7b1db161dd311dd43533466303daac56ce74fac51095ff0bf77b0c4826e91d3" },
                { "is", "16baf5378daa0ff173f5a547d197a18f7c78dd987d02c7c2fb5ea2df1ff5882fff18de28976f52c209365171c686c77a8f71955a513ccc9e7a609640ceb06a8b" },
                { "it", "d38351710e334aa0973d87b641d1aa9443fc269f08c21dc5418f98cd424b54b40dbc7d1d139f93571d8032f0b3540371d2f477a01d5caeed51ce966d186c0280" },
                { "ja", "5b04245be332df9dbda881c9f81e09e48dddc68e19d07a0269887157f33f283b7e3df8d259a65b7712eb19ec7269f680cf3dc4206750acb32c0f8ba28a197f40" },
                { "ka", "851869ba77dee7f2f6393e975c54d86a3b27b04841f2a244a451a7a01d1ab2b12a5e338d9863dbb3b433afc98919c77a309f30eacbb09e30cb5f866180698b5a" },
                { "kab", "229e4b70d72fee87aa714628954fb465a13ed2dc177139c8f2c30003d732aa6eb16e707188f1df44db14a728eb565f4acae07c99302c3bcb6c7a92f37ea7a761" },
                { "kk", "52f701b9ea50422ffd8185264c7fa60762126ed20f343bfdf8fbae4d3014e2cb823c1e8d5f5f8fb77f005a2a7d2e3b40303e10e78cb18314576995cf0bc01ce0" },
                { "km", "ba09c6f9e708e5d833b9787652b04da791615267e36a4ea3d8507b31c0aef93237a4ed29eeab6a233ce9b75125379cc92ad018701a915044af5f3a7657bf403f" },
                { "kn", "d4536c9545b9f23b747a1f74f01a837969812ca845abaa428e42e3a895c0fcd0b50424ad6c333039a9d913dbb4751081a2e556681b0c7d2f1c151a9cbea36ddb" },
                { "ko", "9043d8b599d9e41b226e419355570c8e8ad45ed169a79cddf65d9d6a3c8b60760183a568f43f4d95249487a88ebddf5ade7f266e49e6c4cc56f6e773f1281771" },
                { "lij", "cef36c6b5e7b503e243590ab7acdcd9836f6f034328b1f8bdf89a8ba787ae7ef27caf6524a4fc92962ce4a934f4b8fe0cf16e610cbc39fafed0de8034142dc4b" },
                { "lt", "278475376c2507bcfc3018daf616432a92a5e93f4b445d310761a84e0161abce45878fc4c146b3d92f5a6678cb6ed0ad3ed0e20939e17336ac731ccb8ce0a289" },
                { "lv", "5d5b448537b5110e76170cac69e068fb62e0ba2d12be031cba760921129c1bfdd3bdb21e5463c48a027d6bf668f025e952b674e6a96e2b2e1eae3bf82730f25f" },
                { "mk", "1c3eeae932a0ae40f1af94115d6f76498992a85776bcba02f71b1e7c3512dd5e91c7e703c4a7d3454d5f9c42f14b81df25c818c24e0a883bc0434d5b74cb628e" },
                { "mr", "8e2352e71ef61c802d44cc2ce5e32275233de84ceddc070b3edb1b3d192f7439ed9e065e4e9164e2986dbc10a0d6fc38e615e166b0bc7495bda95b868fec9cb9" },
                { "ms", "2f8c2400a0c140993b6b0ea8856b7a4ef2fe54069a4f6bd11fbe53d6f140bd201969df4ab956fd5e6eed5858636a82e493e1ef6dd82dd355d8f2ed51590c27b8" },
                { "my", "f4f222f126468f6853042fe3e0637dc3748c38ce569e70dca638d9847aa8f2c9a223baa1bfb43e75043ea69c707b53fed30783541c82ff484e1c689bd09ef1f6" },
                { "nb-NO", "da1d0172d5ad7b3459720fa12c43b84e497504f6044ac33a39a68b67229140df340e8883d4e963984bb5abfa282d89f72150544fb6601b6f85ba65585080cddb" },
                { "ne-NP", "e244e7677e39476df7323729c9e39deef6d8b3e5109706a48ceddc74d7b120e96501868e02e73bb8d728bb9a62902bbb034c54b5664de7793e5a131065451cd3" },
                { "nl", "07519987b5ed0fc3eae4ac8472cb96b6b7a34e32107e79d24bc67a91ade2fc01ff66b0d22879003044d711fe44172622d0e9483cd450d259501c07e7e548c54c" },
                { "nn-NO", "316ac567e5f240cb28424bd77eea12fe3284c7ab645aaa33724c3daf77b5f9793acd697186b3e55c516cebe760424364f1f0debc0f582b77ff54f4c7c8792bfd" },
                { "oc", "70bbe6db9c72451e1e7be31de35cc99fcccd16779f995adff8cdfcc05a917dde32f275f3c150e8097a1544da436ca2a879e3c94a8ef18b12f25165b2eeefa365" },
                { "pa-IN", "4d6b3f4afff43a030acbd65984253d321a0367133f31b245bbf484b928377a4e7013a7aa39cbc66c2f71940509ccc7dfccdcf3aee1a8a6529cecb5ff9308ff6d" },
                { "pl", "197b125b3fb313f43c8fd946027d151725522fe51de4be5bcfef5fcc157c06da67333b514da00b08228687831ea29cc4b296d3760880ba22f6cfc41cdde57fab" },
                { "pt-BR", "b0cc8b60600242e499ef6a13e96eb57b2f56a4bfe3f24cc5ec36dd5c4853c21cf4f99b7bfdadc9c935562fa382d3a072e2ea1b4ceb07f57f814cd76017abec99" },
                { "pt-PT", "57057a05770093ac488a2a53c7ad9874fa8b7e47bdc5e9b7fa474729c03c86f70cfd335eac8bd5607e92ef55e7ebf52cbca3b606d4e6ff815b7270ea43b2e017" },
                { "rm", "32e7cddf1fb8d4078e3e46540c1bad42531808f96376e35b4efb118cd4880ccc245a3119fd242c2985e6a2d22e78d728eb0526c549de73afdd1159947146b6f3" },
                { "ro", "92fabb6b92d2141d71f0199d91aa5b9ff4ae92d69743f5a94dc0fbb62b3dedcb8c0ae3aac9f17f71138460d92e0c6ff73e399638eca6c638fe5b5305dff22743" },
                { "ru", "775085ad8e1eb543eea7db8c887952a382582e4c205402c3c9ae12f23f5e11c93edf34bb9db69c3f5d85cf7b4129f95400e02fdc1be750f69c3f11598ddd705d" },
                { "sat", "8c58038a4918208eeaa756e679750d2912ec5fc463b78594ce73156f988a6a5d4ef5861405d62e555a0e70167420b8e9180b3a4057a3c94a42afd5c62b6d4c1d" },
                { "sc", "d36b9d0c4d1df57489e4748915d92416765c8c8acbca5f70629eabd41fb14de0506dd7a5f39688636d128e1d9fa6d5c93c6cd944c09cd5cb9a0d9694f71d9b7d" },
                { "sco", "dd7553f88e4f087f713e2ebe61d7e53b8aa88ec6a606a8cfcd5774d7f0760a1762ed0928c8c6c1c20053d37b632ce40c68a1f9493a313db51d6ae8cc891850e3" },
                { "si", "66ff05e3f52aa25f5de32ddfe748b461ecc617390878b8e0162bbf4f6af72c87a89ee2a56dc66b4c94867bf86932de4e4a69f436f90157bdb5f6f3c863a04fb4" },
                { "sk", "a4d7e2b78ab11ee13acc581e995c448d207b0084a799794c1dcfcf128a6e203fbd58a8d12d8469d04537c9387fc9d265ef888f519c372de083623b9c9dce403f" },
                { "skr", "0f4d4eef3fb48f5afc04fca5ad09def7e2ef3ac2b7436e10b281d4a139ce8411c97e8b9d0df65efe5b4d4440c6b82caa566f4fe0d16f9315045ff9c7a631c943" },
                { "sl", "dade6008d2796e5aab9addb6b85c2280613e76e3d9d906231958825d9b11b44a253e9b05fd8ca78623f5312a5b43dc23d15e92c44da650e1739e843091acf216" },
                { "son", "ce365797d03d68250dfadb4016a79d3833be091d4c212604aa174d3a2cebc63b4a72d139590a977149a38d2cc42d81f242c50e7b0c7aa9f6e0b19d866388a4b8" },
                { "sq", "7cb268e575147790e268ef220c9a05ca223849ce0d6fa7f0cc05537180907dad0da8c57483a241fdc34875707581c77dc6a01e4eb1f7af9a76ce62186e1cefd6" },
                { "sr", "74d3c638de9a4a87a647c4f3ecb333394d84f9e82e20ffedb53dffa58ebb4fb18861b1b34606acdaa6567415fd422c052ad011dc1739bae22ffb18a8056bc90e" },
                { "sv-SE", "54c5b1ca2b8bb9902df8dab333b4a3c0c7b1ad18fc77ce3f82fd06355fbd8bfd946e9cb33b8dbd3404379be84b9cbc62eb2e6e394c02eee82ebbea11f1c7cb70" },
                { "szl", "dfed62fee69e5c08805c6bd5a9c3cbae298473eed3bb2b3ff274db4edba82eab0edaf328111f14f765711decfb9ab5139eee0a4215c93cf3e1432ad74da31b4b" },
                { "ta", "876ec9ad694f9db9d14ad1b6bb21e997eb5026cd08fa017fa0d17a7181b21ebbe6d3ec2b8353067183d9df53cab3f6c817f815a7a3fd4ecb38ec18a757410bda" },
                { "te", "48d73dc05633d7f83f198a4d4822887227257db2914797cc517157cb3d6de3a218be121a7d746dde5a13294b7aa11934b2ff91c380baec1d48985f0bf28273b6" },
                { "tg", "42337f2722c6bc5c0db9d25bb9bac03a656b6de274f8e9eb4db003dd26e8ad99ed56283803522e1ee4bb88b5096c36acfdf7725851196c7f0003dea1ce007450" },
                { "th", "6e682463709fa23def839a3a83ed58bba46b2ceec3983c8dbd744b6c52aa77ff6981bb1705f18725ceb550e0190bda68e47f0b0a078de3134c1de76d7ab30845" },
                { "tl", "2dac2ac329085dabfe6bc42d60e5d1b96c7031afd6c0d334688b931f54bc1bf30dab230222db3953a893d3fe1a94c93bfa8dda9f7e94b4929e5e494e65bf3e91" },
                { "tr", "c4317b73b72c05ba1553efea45632f0fd19ba538082e25dbab8419e83960f95e52a8fd39af2ab1f53788a6c6d21a4892aaaf806c8f47ecdf595b3c9ae1e6f165" },
                { "trs", "013ba83eb31c13492127c3316e03b2acb45921cbd65b34a3d9ff0343a9b0d752db95365000c6d933fbf3832d37f2587357e756eaca68b2f3164385f7b528ed25" },
                { "uk", "90048996a11d0364b7a7449383508a526b500da5a6249cd0445e925c15a48a1cc794e69be2652f32d2a8d3a8d57d4947136a2f9e28ab357e92aba1c7f5e2517b" },
                { "ur", "e7c1bda4352d8aeefc659843ca82d8fe15f5b7b707f46e9db183ae3b2b97827d903f1e169ebad27afb820d164f3344bf130e47ac378d9daeb7f4710a7eedb943" },
                { "uz", "971bbf11e69eee69702e5deb3ddb41e03e048fa251e4e597cdba193d79d11b86eb51b2ecf220b71a3e25631c46d5d4cf206dbafbc90036828c614767a640d2f2" },
                { "vi", "f373ef594df6dfd2a6fadf189c923723441130f828fceb505a67dde120a3b1c674de44736a5dcfb38f7ffe24fbed1c73691ef58856bc979708356c32eb606143" },
                { "xh", "6fdc4027527c07805aa0815700ae54d7e32aab895a40a7bc28002f7e9a64f2dc31c4f1e92ccc2197c9ed9871a2c9f13648d63c0ceba721da352c52809d68c350" },
                { "zh-CN", "6acdc4dd23668a7b91932fb37e28ad296d55b4e3a41a454c69f96304a4d64d457deb5c0a5165b7fabdb6680b44a81aa26d438fc31b5cfc00e3cc5025cf1bf571" },
                { "zh-TW", "187be7b7bc4256ee2cc5c064b898b0b82b03846749775673afcdaf7b37ef0dd8ece9bbb334c0a6ac7b9333372b0687d04763794917a4c26339bc99bc115b4a0d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/128.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "9785cba47ea4fc96490899492c8b6fded9de1e92c1a828fafcd82700c29c31f642298d6eadc0409c298d84a5281953ffec2e0f92546e172517389ee55ced79a0" },
                { "af", "d2735cdd1cf12410b36e4bcd5360d4976e7696d0c044eab8a4287b7daef3f1d8b350ec2497b92ff377d6b7c11c822c20cd11c759abe7af18765f5de51793f311" },
                { "an", "23c9a1485cff7847c7ebdebf4ca8e955ad3e424b775a47c2adcd7b6ea931fa9d391b3e6923b25c0581c161ef4febd43479d0fc29ada2e2869d6179fc74ee33fe" },
                { "ar", "c1751fbb949096608457f6c33aae4b6e2f644e9887017412787adf64dcea626431b004149f46b2bf5f6b4d30410581b254d7e82f3ffccd175723d9151dedc4b7" },
                { "ast", "8cb5a0dd831474e53102aa081bb98b6dcc07d871e83e1795fd346cff3ffcef3617505b13bc38e1d31c3c7c798d28bdb3c535593e6ce225bcfe9e928674d19f07" },
                { "az", "9ceb98302dedce9102ed066607f3f90f7972c9d64c7b5423e8844e4367bd18877124bfdb5c95794473b390924778d7598b7843d6929e01815c6502b31c68f1eb" },
                { "be", "7b8f6ed65d3279ca8007868abce3d470af9410487ddc9b5dd8a312b77d92d398b07db9155c698e23c6f5ff7ab3b0a33c3cd6f1f7953ef4e0275f69798185fb74" },
                { "bg", "9a657132b34e9c0e9e7a0ccacec6c3e8aa63e6b996e67cae4612f619e774360322601e20dee6a136edd70e61686f1fedd11e20df8c88207d5f271f7290e37d62" },
                { "bn", "adf27e8c704a6d9d87fae1dbd6f9d3f900f4578cee8532a0e6d76e02b96f3fcc9428f34e1baf4df1bba298e39951764ff2691315cd94caf47b5c4411a4c7c175" },
                { "br", "c98f301b1919fa250cf9a5bc7f98a46ebb520373a4663eb8474590c7e5b100441a84d9f58cf7aaae845b6e59ac117f7a8fff643e4a24496fb3e6a5b788437a98" },
                { "bs", "5ad89b56f5561190d01dcf5ffcc036c5044dab82d86be2d0317c02e67a0e9b23b43858b0e7f400c7c45c107f1fb594205c95f6bfc71cc9596c6015ba0db08b32" },
                { "ca", "5cc071589e0d67bf1402bec6698c92c83aebe11d842c57ad08e2a5c2cfd73404c172b01d70a1887308bdf9ad93b7d97ac29178565857c352b30fcfafe0bb2059" },
                { "cak", "fc7f221024ceecbcfee8942c78d8167269fbd46eeee7fe3bdd7a67438ac78e9189635d665e1bb2e472d7698789da81bb9733a8c57505dcd7f534d4ce61981092" },
                { "cs", "ddd1888df35298402257d5e78dadabf362672cde2084443e8907c4efb5ea30d2e41d251c0b3cec2e6ca0a4a5f1bcdb629c0ce16b5925091e396f956298b8701d" },
                { "cy", "987c8835526171d26ab0687903ae18f1d45316e9ade13540fd28d0d709a96df878d210b0b5cc4949503da9297453682c0e2df6ed4353580133495d40049089b3" },
                { "da", "c6276e99deb45dcc575e599bb17a6ebadbe32b1c0b425c97e00057c0c79ec0101ba4277cf8082ad424afd69fb745b699c0f75556c2ba177c0c5285897e860a14" },
                { "de", "4fbdb6e52bec890af49701d71247684841a187e7744e42dbc7e0c00dbfd11e89a24e4e6cb824dbdb2e9b880e19769359fa4b516dbc33d4f2f55aba44d0b86e0a" },
                { "dsb", "995583c5d281d3bb16c0557efda4ebccb07bc367f8f7eec14e4165943b433ecc8669366f8844e743575f29a49cbe544ccfd4b755c306401e75052e156be01cef" },
                { "el", "0063446cadd45ed90a018b8f31fd5db26c3c240ab67ad1c9e9839dac7c6321954bd4bc8218475b28993e39adcabb91770fc42f8f7a31cae511fc593f37f0ad54" },
                { "en-CA", "67fd3c17638d171231479082fa9eaa2beacd9476e4c9440ddb53b9f72f49f0618154a8ae0f342e048a689d5f027ae4b47890821d764173040e9b4f3979f207e1" },
                { "en-GB", "26e6973498aa640585a3bea6b76043baadf8b3517c5ce4ff03df5a1cb0f6300d66c52a0838573aca2bd9e79e64f4355005bb161087e03f14998a8665930e1f61" },
                { "en-US", "5ec158acdce02359b429fd01c6fcb52c96bc8de0773d8723586057263ffec5023c4357fa8187ff65f25a84ac42445b670ed408a92360a95103dc9fb9031890dc" },
                { "eo", "55ad341ddca986160320e1ebbdb4ab5a0c87270593c2c5212497e0e928ddd8c81b87099f2d10b66fa01109633102a1e039f6624fe9f218826e48fc9cadfc8339" },
                { "es-AR", "aed63ce9c6973d790a4c12f97a514721ed740d9336e80b8d2c6d9bf90fd2c4e696006becc1fd20f2c2ec6840ee6ddf8b49d95769937ac841dc096afc20ac41c2" },
                { "es-CL", "2b4a41ba47e7a0b3ed2ce06184d1813fbe485a7648aa80513d463d5df9a212be9f3fb1817be2bc0d76a8868e49ab244e4bc286e0b5f8d364f655f7f632d4a07a" },
                { "es-ES", "bc0b2220f9be6de895871c0d76413e112ceb0a1e4d23e5f059defd5a31f1255dc469ea3aeeb3ef7e245d1391fdcd7705447d6509b899fb1e59b81e4f4dbbdbb4" },
                { "es-MX", "49eecb7fcbbd61c29985e52ad94fcfa4588d147c3c7018869ada4c7162b973a1a54cf19364e393e6622f9501cbee71c009eca6062b973afcb9faf1e0465393bd" },
                { "et", "55deb2ab0840705ff7eefb528a47a28c5478cb12745a603cf4556d4f233fd3846eb266b6513269522d52d2f51ff5d369aa608b52ca3b78922793c3a6b05b87a6" },
                { "eu", "c46a01defaa6acc7bd1de9678d30916f7a53f967d8a6f1e32c7381a49146385236427cca64aa7de67c791e9344f95380eb28e4b2d09d7ef36783c0c8be02982e" },
                { "fa", "3b16d6480fb664e336abd9201e1f8232e9ff638e5986c39d8fbfbde723caea2ea0372e42aaeeb56a1b392e99c4e8390378f751cf09c88096be24d5841baa3750" },
                { "ff", "05a3537edb8c2c5fd4a4f31648b3854cb806c2aa46241e3a8ff20b66441c14494703839c2562375de63b538129cf0aedfefcfb78973268e666e640af7a63efa0" },
                { "fi", "3af30252bd046c71d9eb854346b2c4d4037cefa515c1a1e08f0062353ba0d0012d52ebfab7d08bfd4b8a12cb63e3601a073fc87e54d918b2fafff46da3823912" },
                { "fr", "7e9700fd9436ff696166c7d12340dca0e5b6d372b6470d7e62b419a9367e112958d3e86031bfa8908191dbe7cecab090a6bdcfdcde6ecefbfe0e1c50fc3664c5" },
                { "fur", "6b94cda1371f506b7a5d606af6f4bf5c39fc8ddda8e47630be8f4b718028a62abc2588d3d05d0aa758454683b3b28da6b705b7506cc547a72707e16189a64da5" },
                { "fy-NL", "b2be35be738ffd98ac9da9a94c70fc7fba6c3f9d3434f3bd7fdf05081d84237abe248cfc68310e6bbd6227073fbe1f32316f2b1628dbaebae7eb14a85039f3a6" },
                { "ga-IE", "0c39308f7bbe1fa832d089af21847d6ad0dab69a737f4dde73999eb1f0be469f5fa11d263da75a1f542d6616f0362c7d63c9eb0bd315fd2ed4dac2fb389fcd55" },
                { "gd", "00ca9af55b27d06453ac5d185b7a147156ebdd4ca3cd12d24d9795e9d73c30ed82b8889c50ddfbabde3501ab0a616028d58e9618488f16c517c001c437f7c7d9" },
                { "gl", "a2f67ccde186dfbf77a57ccb0db667842c8e9f961ebff1c7da726115f908c72fe6c5f43694777f4784800fc8f8c1061af0dce5c32b3ddf4eb0608063069121e4" },
                { "gn", "e4ec1d3c0f1fa57b51114568b4f1415d67dda4167d9e45b5f4152b998aa7d83684d6939ba4411db80a0567d67567f6cb3c24365fe0e59e7bc826b6b8d6f6efe3" },
                { "gu-IN", "690546a0f69060bf86ee02c577f85a3b8e4fd0c6d25a06bd91ea3ada19405f56e0f5e95749de5280b785942380de62c518553d6465b7c1096ab89adfd620604d" },
                { "he", "503b9a2ab03c4777b7b7c5c70234741637883bf7bde648fd8738f68cef61d6b65d928702b714c28ba46c06c9c933d3dc3c7f2ef58184aec2c6eae8a248b0966d" },
                { "hi-IN", "376e3a430e3e88be8b4960a15a577cf32030fca0f9301e9d7aaf56076eba00d011fa967c49be3a95e8fe8869ae55d761c4604e2156dd73fe88fdab3f06fdc863" },
                { "hr", "907d457300922dfcf42f2ab45fbe0aef1f9832125375b283bd1bce093f11c860201b4de48099416e9daad8934ad386a65a1d3d6487e0982a95c1988470edd38f" },
                { "hsb", "585d044a49e24d2f3a96ddf24e0e6c3ba935f0505a1dc0c111dddf351e6f9accb9b1bcd5f402ba8e2b4f48f59ae2f13fb11c71a361bfd20201dc40bb4034f8ef" },
                { "hu", "52ed7369007c744cc058a877493257773aeb3ff69f19656939c6e3804071efc106a522323edacb5922b588eb783ab7e6a3b0730608289074977f4cb549627372" },
                { "hy-AM", "83331c19df71a2df012e7aa0f9c46eab53cb4dce9cf38ae7b39659feeeac30a57258cbacce4466c880b367ff5698bef02e0f49c3dfc9cb7744a5928e8cbb2331" },
                { "ia", "e860b4171821796d3b4a6bdb81c091000af6b4d89b82454eb3391f1c77786c702030b879b66d0c8d1da4aa26085158dd93df4c9b8ad796795ecb0b695b0583fc" },
                { "id", "8d7ad173b0b1c31e2b6714b7c59b96153436c7b31952d3a9625c42affb8a16ea07a75772550303641270ca29b829e5f03676169c8831c9b52ceddf36b62e3432" },
                { "is", "68237bcf104b321e03120db6251a6c1d42e064d35918f5a562a789059299803fe046ac3d6c6c9133087b358ca809c0c87e010bd0c71cba93555f7a77cfaeaa79" },
                { "it", "c053ef063d0fcbb548cc5d5b7f27995b8fb9446d785a9defd48e3d6a35120e3227be91818f2afeafbd462edca4cb2da2b5b9010bf2ea992b16874207e7184a30" },
                { "ja", "730a25379692df897974784d743e48982b5056791bb5d1f8e38a5a0fa62e12d4c6a344a49fa9f71479e6a9501754037dcfcf77f4700ab527dda87f5892b84f3c" },
                { "ka", "2dc970b7b7be2af3ae2cfc6fad1a8ae220284879ede4330ce398eb024ca613b312690c20c3eeb0742bebbf5ac8dc82926d66e4fae9c21b9292c642079cf516fb" },
                { "kab", "d52f0e7a8d9962ff8f54e721655ed643caea756392f011f9bdc2dbd319e97d1d721e71ba75b47b238b1438a752823a17e7a9d3a9dcc812af08e66f97af15390a" },
                { "kk", "e06636d70f28b58ae6d483a23ae5b5ebfb55765f553b6552f7435041e9ccaa5eb9454e142a2d4b17baed56d2dac617747844ee906587695d280a9862ac5884cf" },
                { "km", "b1a41ba9c103180f2b23fc2460e0a7be707203f79be8b36d6bc02217f967d4957dde6b683ba86fe0bc2dd520e4cbfee4e82a04e31bdb6e71b5f0f327aa7134f2" },
                { "kn", "8934c63ddc06434dac62f692fde4f782208d7afc04154a109ada34e3537546f0d19a65b3eb56296a8e5d5b0bc0bf4db2808a637c08b191b4d6a311ae0370ba72" },
                { "ko", "17bf36aa82e921fe0c375471752582a3dee0fec9a0b90e2b108146816c81d4c1f3f60e7374d441663d68ed1966dad4fd13729d131c8f8cef759fe96bb1056caf" },
                { "lij", "5de7ab8972a80ffeada3d5f994dc4da82f466977dd14e49fd7f2cc9c65322f2c66e2174751a4a4331679dd8cd15ae81d78ebc451693ac0e59ce20c145e4df303" },
                { "lt", "a36179ec5ce775c4dd5d15685d9ac2c24439397a47d1acba23e997f92b708a4983f140fdd0df04913b057589b559da35f414eed0c7d1b1eb64810711af1a4b3e" },
                { "lv", "6cea22cfed48ab301187f4ce85c72dfac4b3c6f3ab86133b3a331f0125499b400736904d463ff9c7083139ec7f65d39e86b571106c0253fee98c80069e5ede0f" },
                { "mk", "2fb415358655a857c36b5e7003e8949aa5bb91ce921067418d5c8102ea6fdfa3629ec415d154628bc9f18750d918cda58c6fdbe6d4bd9ddf0bcc7daf6ef46513" },
                { "mr", "49ce9978741a79cd66971b99facc2d66acc45482bb9829b0d22058dfe63a789c426ca3924bfc8287596c8f47ee5bc53c0bc8355b510f692647c1de6f9b343dbb" },
                { "ms", "bab93931b09442a9aa2537d0416eb46ae51a95f4b32b95354f4259cf02cef2203f0fcd99987fca8c96431b1730907e5d7394a534aaa69a3c53b5864187be9353" },
                { "my", "3ad7855fbd94b8587585a90e7319fb2fb5f11daeb73bdd7fb0af84497dc4f45a659982543cc0f921c3825844d31079dcf08535fe4ff6569f48527626202ba817" },
                { "nb-NO", "93546a69cb3ae3e22d71ac8639f6aa17ef0a0ad7021ff60682b1ade2def30977ad03758c32e4bb2ec9a89995fd68fe5e4f0d37d6cc94411da0a1998393628567" },
                { "ne-NP", "3fe66ea099fa847455c61af6fd55299dafdce54bac2fcbfba685ab60706a902117bd9445e6813b7ffbfb4a87e7c9a8d7e1ba0a37a07bdc00ee9a8873b7e12ae8" },
                { "nl", "57e0f8885b0250c478de9a37e5a9b22e2b575c08b406bfa13a54190387b685bb4a88b4932bdd95a7bee7a78270785c190f6a4f1d83cffeb667d6c04fde444c13" },
                { "nn-NO", "5fb72105f3a8948b45e966ab182788f71359c611b07b4c4b7059f2f084177815755ae5dadc3f608de91a4e82fda09195f995dcc46ad9da7fc6709fca54a08940" },
                { "oc", "8e95d1189e9c6ef13a0560609aeaffafde40e23f3f74846a249af8c06027a2b99629be6af979d69ddebf3345d2f2912d40d55a275aac4dd88e77ae651f89e764" },
                { "pa-IN", "c9c6e0cd75fb0b37d8368ad6cae2cf75a09b11b8fb38177816580953480d45893e58a17f9ec7bae0e9f37f95ce3458ec97fc3b664b6fbe34f1be7062a63303c1" },
                { "pl", "817c6123742f7c9a3684599c32dd1f914f92ec20813a10d47faf0f1052c5276aeeef4596edfc1f7463c04446d1c0490da94e6a19d1bd3765afa161cb2bcb2dee" },
                { "pt-BR", "2daac5fffefbbd639bb2511ab26b97df1df6dfcc3a462220ccf6db3e6b2c1c0a3a3626687d579e4486870d1c269533b6e2b5dc69ddffe351c4d4a3fa66cb3d77" },
                { "pt-PT", "6e259a38581f31cb04d5fa8792f3253111dd0504673ada6d7d62d413a344f0f290ef6f5e1132e0bfbbdcbab93d982deb3b6c99431d8b9fb59e222d89cb377712" },
                { "rm", "f1d1a16e872502ea111f4c457aef28ce17a26ade36a9a2b967658c322f85c73a09a87c2a359b50ef387740477722839e1d4833ebd46e8308d45e5bace49b080e" },
                { "ro", "a5f1ce35653103233619a698c426718a0e21dc2a2bb1af25ef93c3490375f6dc00f1e609414bfee41836d0efc9c6056af08035fcefb1ca8f2b7f1f3b63af9cad" },
                { "ru", "d6a26631fec3a5ddd169dbb56233b09c3691b9614e9150dd2da748e99d635f9da675e9f65ce7eba4a0bfba84ee74cca7b574c2de3485202f8180498998c842fa" },
                { "sat", "9e272e0d43f7edfa6da0d457739309c77dabfc1729e3f58a54f33c3e2179169a9881514fc182fb22daca7008489e833aa78055556d5bb432557357706fda47bc" },
                { "sc", "8d827b8eb9c950b0096f0a3a6d0fe61eeda48d358cc2e40b89fdf702572a860323ac526128095fac05f61bf45fabb9bfd04fdfed891bf6b2fa244891eda0319a" },
                { "sco", "685a274a5a07a280e7ec9250884e53e6abd458a629541e7dbcff97026acabd31dddcacad9741973421493d8f84ce574b7b8d91c3580bada9821432dcbff821a7" },
                { "si", "b44545a4c699a2351338648203e486992c5689dda4325de78ff3f92bd31de8c95c1b5c075ad9756bc1f2031cd4e30bf9b489efab7a3bee45024abcb40796bce6" },
                { "sk", "f125e71d0521b2c5505521857919cf82c3bc2e25a3c0e0bfdb91374ad2d39be6bd0f64a25a8e22080ac79cf55e361aa7ea82bdd20656645952babcba68a96b77" },
                { "skr", "20722a1f4a14264080a9116faa6df45c12e09783c56b234a1a182f08732da9c5b6749bc0c735082674ce9410bc422daa7b7ee793d3153019fc71a734e8f6b1c4" },
                { "sl", "2ab7cf447d5bd7e5454cd71947c5e649dbfff73884b19f4ec53232ff302894c0b5f0c46233302af405527ccb6adefb09152e5d208776e551247713c3c4666559" },
                { "son", "cad34beb994157e60fda564f716f440cd03b0f6617e6f81f0be5e7f36e2424a93ff84780b17551b134f64672c5c809df4fcc76d1521538dd056a98b144a6f25a" },
                { "sq", "0dc22df6659a4d78f8debb87d87c7ed509a4368239d14bf9152998af02f4389190a76ea4c3a41b219c2c18f0b034fda40582129d529b8be482433877b0bbfa77" },
                { "sr", "9b989f77ea8f6a777ba24258c6da943e2d4aa72819ea74215ce0d1473c49168d2138b96b0f16323f19f9f13ae58546cfd04d8257021207dd557471c66bf316b8" },
                { "sv-SE", "c6b279a16a880c822671abff0e280f631c61ccb15e14493e077429285b2275fb256838e91612b9e07a56736c10ae7cb7c03c50969ca536cc2f133af000a248f1" },
                { "szl", "c02262b5a0b1966ab53f0037f07bb80c30d37640cea43108e5e5ed4d246baee3132253a8f0edf79973a85044edab7f26bf7990c836904f490496a3cd1f06d147" },
                { "ta", "12d1319236d05362f9e51778fc2f43665af8b5878e9ddf62c0acacb909deaf8aa8eb5945b36fc86b820c446264bf96299b13c0570b79a1a825eb4bb6e906d536" },
                { "te", "4b21cda969eca4a2b1d88964332680f4e1a86c6da544a3830b269ce1e1af801eb87de2f496c5e75e5ed28ea3609e715f76b83992150ecfc8dfa62ba419af955f" },
                { "tg", "684c42780a9330ee33ef12af61c088e3b29afa00061a94ba2c58f0ef35a8c12f0b08b37f40ac01427dedf8b474cea1a6d4bd44f56b83b14b06876b2a62c25d26" },
                { "th", "49acfc14e355de3be1e8feae74de39573d43e399c701f2c6d65f03dc1d6fd50d88702756a5c49a377facc76dbfbea501683ab6ffae316ee4eed1a1fb5c80c825" },
                { "tl", "2acc4eeae2b40523746a2713794df36cd89addbc41a8729250993e3432d5ee91daa7dbe5f0fda68831501d0a917fca04bc6bfd83810fc3c9c70693b1a879666e" },
                { "tr", "c3b2b42009130845c9bf159d7d603b674ae07c8d44002bebfeca1176b61e55e15b41a6c83dc85ba7e4fc66d97e16ec527329fd2e6740e41f4d4ff4fd6e2b6a12" },
                { "trs", "ed173ee427bd0ceddbb2674aa322b00d2bf51ed86de76f95c47d8595fada85bca2e94c4b37058484134021f9ea2f702dfe4b54fc41466d4bafa935fbb9882075" },
                { "uk", "841500b9e6c875e5da27fbfd0b427ba86b59e6c9cc2d2b9dd8dd09ade53c3d8e0e2ec41e331697916ac6ef0f17b587fba03f9e65a4ab9914e8d49fe818e817a0" },
                { "ur", "d6e3cc9987b70ad46eb5e117e74359d44d62849db77a21cf778771fe112270c4c04449f80ee23462828557b9f9447bfb6255741318f886ff5bf5db64626831e1" },
                { "uz", "2bca8a2b68991ba27d6757a9c4a36cff056d767a06aeebe139e5b22637a987bad3c9378872e4a4e770f91eb6967d0160896a93299b7a480973ac130856e128db" },
                { "vi", "be24cc270ab5c31a420bb7bdc8cdeed4b55b4983f751e98fb9d6cfa0fdf81f775ecc5650dcba3008b72242ffad70ffea67a747b58e0a909858ea6816fd21039a" },
                { "xh", "96b7ba8023742ba881b54be1cc798e21d239f818729d7a2fc4cb35db9fd521558e9686aad42dd7f920d1fd9273ba6290eaee666283a8c5e23ec4955b364ee0a0" },
                { "zh-CN", "9b75b8aff0bc759df8c19c15f03e8eb288012e624caf6a6506c917aa6ac4864e1b106cd8c95d399d8f7f665436646d7a10c20ba329a8187d3cc50167417af9ec" },
                { "zh-TW", "5e80b0dfb83b6c6d5f3fc99340483a102dd33b9c536f5fa8885a8288ebe35dd424b064c1760d1eb76cbe9fd917e5fd64a74eb4b4e54061eb1ce3997528e4e951" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                    // look for lines with language code and version for 32-bit
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
                    // look for line with the correct language code and version for 64-bit
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
