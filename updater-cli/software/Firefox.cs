﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
using System.Text.RegularExpressions;
using updater_cli.data;

namespace updater_cli.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);



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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.0.1/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "fa5326d98b7c7c2ca01fa848c9a70616d42e275bce2c50dc194eb29ead1ec5bc777d97413bc0ad8ec8ed158b90ac2cf4d75fa79de14fad40fa41d9152051a072");
            result.Add("af", "9dd9c87dda39d375dcb8148fa43ce00d59d6c7d05030b44d08d31652b36ab1433a8d9e83f8fd958421878a37c946d785135c7c4f747a66ff698e7adaa9598cd8");
            result.Add("an", "d2e538789eefc0563ba9d9aeae333a13249427593262a40f44ac751057e171edfc2a6b6b3f457c6fe001a26996b78f047a80b7c3dc3a4ed5a3780455c998ea6b");
            result.Add("ar", "f39f38be35af316d7c09ef518eb4813a165e1d68d10381fc578f034d541c8a83a6dfc7d2b614c6b6e7c6ac6d4325d17672215234ebb65a3813b36d0da7c8ae95");
            result.Add("as", "1340a2b6de0eea1a9aa8893b24afbc219b08e17e48f16465aa1fdef908c225eed1c8b13a89c45885488968806015a0c80c1d5f80020c88e9cd7980c420808d74");
            result.Add("ast", "b5cf5558981ae13b73635a6e95cd676292e5099086a24264f80d799624a739ac8497cce5fcce6dad08d42088b39001ba26a9d843c2462c71dde508b312d3075b");
            result.Add("az", "71d979ce56fd6c9d2150c32485025dfd96aae5c39e269e1d4d5ba220767e743bb96256f82b6f12768c5ff0912baa89b8dcca21da248b3ffa5afcaf9360640ae6");
            result.Add("bg", "14333e302ab5634367b3ff016e430d8d99744962f31e7d5aaf2065ca0b04a2634aa38c33c9582361d9db648a6194a0b1f6a7c5526313aee5572c4e2145a5004b");
            result.Add("bn-BD", "780d05c3a7c7d5cb002723695e549dfe89060a50218800c53c316d643d7ab351fb5c79254b08761b8a612ea59c206ac3a54a821f10717e25b52fa6aec1f60473");
            result.Add("bn-IN", "c250b9c004228502e27cc6a7c84d4591f14cc5a9cba16dda78b8038b74a67e7769355da318c63e4112c8b63e03c7c041bad5cbad068f4dc54b2314f36bc5ef1e");
            result.Add("br", "20b05c3330de3dd66b22d40722c336184cf5f8b5ad04ceefd882c7337a1ccbfadfdadea6b54695f637fae97341a0ffd0641859d1571f30182f126fa35fff0f5e");
            result.Add("bs", "1373fa3e0af501a454353ba85fd2fc42f26ef1d9e4e0be194c6e7e06fa113aac40d50794f788e8f90cef9b06ba812b3c970633af72d653e116fc43ff9724e19c");
            result.Add("ca", "8e7f5973bfb27ebe921ee2079e293848ebd6581fbe7144138e201ef8a4fc2a664c529104e3e70f8d5b662359c984af0b8938516ed7fb246db28175b1394698f7");
            result.Add("cak", "f82196cafca593f0b60fb97180f2a6e5abf3ee53ffa56b3357e233f49d3e36016f8623e6db5cae828b6dbaa33e06e3f687a0225db36d84bf91a0a21870355d00");
            result.Add("cs", "40cef7f47f892156532d6f97687bd32605cee1122b37338e4f8320e22073d1335e207bfb5e7a807fcf5ca3df9614ee147b88731c5090c287c92f868ac3221f48");
            result.Add("cy", "c302593bce41765a6bb1d8c992c1435841b379dcb9c264026f6a0816c4dbb9f7de310984bb0f527d8aac582dde5caaf3187241230f6eb5878a79766fc29dc249");
            result.Add("da", "0c41d2dd13e4218068b96c9bcfc1f937359b56321884d6269b1f42d6fe24eeed8eb4e822d0c7ef6aa24cf682f2f9399673e6bdfab58928d33f8c5e7a10fbdcd3");
            result.Add("de", "233a14142333cee9502ad1e1c82e076edcd3c0bf204bdb0ad39a6e183a963800ce54d392547284b577d3c1f97d63ea9e8b6f295df266a2d431cddc2c3f5ca1a4");
            result.Add("dsb", "63927fb5dc6f26a0f49ff4ea0dd729124ec07bfb404849025d418bec87580f4b45337bf2a225a9a36d14fe8be9f3f28567911b0f9abd57eb5617d85edbf1227c");
            result.Add("el", "39645f8920e5b4a5578c1ba0cc8b2a3d5a2551f3fab6107e65ec3a0571ea06a8ee8d6fbc6d22d3e6bd8eaf1d46169dab1250dd63a39606af73204b36a1c27e61");
            result.Add("en-GB", "2a0c3653211197836d1df7410509e4e93b7679220c211c173263e7cbe1f475a313f785c656e6665cd799eb2ddc1262fe7deaab5f84e8cf28a59747ac906908d1");
            result.Add("en-US", "5d3aee0e80ce1731883b4ba48fa8d543cda04b451fd4179db061ce5477c25ace4df01bfac24aa385638f20b3ed7447208ac99b3c164345732b8bcb0463217558");
            result.Add("en-ZA", "945f3000f540e4e75a3cf0cfe1378ffd6a3cfba9940a75a2bd7232de2a7204e332f94d7ef5aceec22483a04bab8823717cc2788b0f7e884fe710086a0578f9cf");
            result.Add("eo", "050f8a855736ddd40356f18c8ff05c0c00d08f483e562d72c6f68fb34fdb533f019c3d7310599a9aa365cc34c21bc74e102bf5ddb4bbbc148e8eb87420a5b929");
            result.Add("es-AR", "01b8a596cf794cd2a336945d1fada4a19b95891aa53f049b668494fa548e78763493fe75eafed876e54055600fa41ab4385f63b43d0a5fedea29071097f6a6ca");
            result.Add("es-CL", "aecb813cec221fec2e14082340ce300135f5b357269887ee195415cb902fb7e61d5cbc35c2d65d7b81a4ca69d1439072fd38e339cdf111b60ab739019884eda7");
            result.Add("es-ES", "e79c645622f73b431ad01a9d3acc4916509cb7f07a31979db9c423ffcff2af2639f47e6b7fd46a29aacd068956e1fa308b46abe5b7db5351b1b0f0cc7bec84c9");
            result.Add("es-MX", "0f918a06870e23dd9aabf7139c3cb3dd21ed0fd688ce4ad191f9bf089b2aa9c29ec446cd20757d0e5c1e229a2939db997f2ceb6a4af0d9797352ae09e4fff6d4");
            result.Add("et", "9e2773ac37799892371921c73acabb890383ad07a2ef9575a1eb04eee41a7d8f36391d2ef5fbefe427be75dcb48596b9a23d1082b1390b609a2363f3a0d12b79");
            result.Add("eu", "d62b63069ef20d280ed874f45fd88ec0f45ebcf5f45d7435e4ad134a5740bab17d81b131478b8173963480535435bbffbace781efefadfe246b34c5017b0483c");
            result.Add("fa", "2afd4b4a1140da2f86d2eabb64c3dbd55c787cf9c7726c16cc79edd2f9f1f96a08c8ee1696bfb4358622b4cfa63980720431ce00b14e04a629aca5abb7d83870");
            result.Add("ff", "a5516ee9fcbff0c9012047ecd142a82bffb9227e1e1cbaa0e0345545c90107b6d12bf41e253f830924553f1ba2d6a70aacd343aa073765b390ac138ab0857821");
            result.Add("fi", "0f74f555f82024f9087751651885b17b4a4fcb83ad6ba4a52a3340dd2ef894d3e9a8634c062faed0b0c93197f7aa69cd344c56b093f99e4b963ec1faa50d30e4");
            result.Add("fr", "7bd882ea8aa875c3c34fff91d5cdfc1cd4099390bb36cce4e2d4a9f9d399d6a10d6fd09e0e6284c87507721516ac2c3a2702f33bad1d527d0e7c1c2273dda5ac");
            result.Add("fy-NL", "73f93facc386efb5daddc56ffae1edb5e49e4136be721bf23237e7feb0904710e9dca266e3343c194ef78f88302b45cb839acceae522bb6a130b74ee28efa9af");
            result.Add("ga-IE", "33a28b2c5d52b3a4ae29b5174d4174689b6ea859f2acb7bd571dca6e09b095cf22025bba2d213d85f6ed34b4c50df8451658509e94f1f3da6800f1c7c3b6b78a");
            result.Add("gd", "fde1350bef666d321d2e7e0b036e50a3df7f61a6a8e99d615d3473771707111f798dd4539c45b6b9dd48d54c365f51dc93ec976bb512c63b6bd5a9cd2fe1d57d");
            result.Add("gl", "f336970deb52e2b9c959050d9bdde8dfebe08cda7a94ec4c6acc14e490de0ff78c8bf0a170c74a0cf035003d47578ffede64bb0ebf5a074cb06fd51f51e91625");
            result.Add("gn", "b8a1f7fcb789996cb6d83d8ba7cd34d2aec505c6873eb28f2c286f399af80d51086fc78dd966a33e9151422f90143eb9748de772b594f5f36fabc8e3d1c41365");
            result.Add("gu-IN", "61fac3e10169c3d3d9ac79dc0189efd3ea26ddb8703e900a41cb8275b8e51389fbafb134e0d5918b0370f95b216aedf33b26f2deca4798f06e294429250bceaa");
            result.Add("he", "6a3e3140ab8e9398a8028b6ebc42eece2db5fd3f03434c6a7b6ea9f5bbe70436593f429fce12664b88da2f3e0944b1796cdd32b4fb9a03a84f3fcde8a66461d9");
            result.Add("hi-IN", "b80f9d8a2818b61195f1c40f01b731be28b9ddedc078e6e47f583662225324cb72f68fa0a8c6d1affc0560e9f0e26db2e5e5812fe7f540000dbbb29c40f41b42");
            result.Add("hr", "87f0ccc057708555247427d755eb72892f8e812e300f90b9208a4f00f1be73de09418f676b08f2371da3d3c4c40e80d66f796277cdfacceff1261fa956f4dba6");
            result.Add("hsb", "0da427b958fd09184d1692eeb6bcb2135503da686ae4aa565c8ec0042553761e60cb34a6e9b826ba3585bc56e6224344fa39aac66ec9417b25b00667a011c39d");
            result.Add("hu", "0573f26a95a225e2f4a6d2f2c60a5ec986e8f1ee223b048105b0f32ee74ae3d0571c4c8ed1900d9c88d35cb1a6187b6cbc625a4ae97e517f3eb2d5ed418fcc0d");
            result.Add("hy-AM", "8b78982bf2be68026c2e88217db8540d033d8f7f907adedd3202b10ac11348415faab2489eaac0a393a30e0e85e6988bdd95870ca497c359452cd017c41ef5f6");
            result.Add("id", "e6ae5caf3d8f670d4650b8b26f0f03660b2cc852f0a9c817b777b223f413e8512817b12f87afb5d4192b95933dd9e31db43caa7fd1b22e083707ca841d116b97");
            result.Add("is", "fdb6a0688b35fc65a55b168b6a5b768b30496d8ca68403321821ae10a5d49b6b9b7cacdc73f1ecdd9accfbb9ef6770bcadcf408fce80432c796108b4425ce69a");
            result.Add("it", "863e84110f75439dda9c37396b6f6170f00bb8e225f8dfddd4f9aec3e1c5014feb5524b9c9c8d132c9924aceb14b7b6ed9c1d0498984e12d6e19a54aca6b9d77");
            result.Add("ja", "59cee28eea46e97a9aa38ac5f86884a9330b65ef2cb657acaf8fd0d1b713029df2addbb660319e98388831aebf33957ed2c088b744a54dbb8c6229d01f69a528");
            result.Add("ka", "1663a4a67780f6eeffde1792437acdcf1441f35fab8b9f1c4b57f181390bd4b123864a5599276f3bcf741add7c1de34510d9f931b15fd78d6225ff92a729e544");
            result.Add("kab", "2faee0c9b7e3d3a44a4c142f057c5272441655c23fd27106bc845087251513a2d57295f345330397541423d58c892e1855d543b6900ea60b1bbf4eb5e66c3b4e");
            result.Add("kk", "727b2fb90430e5bd0714cd79bd61f5cbc62e2555ad16a01737140f1841a1e1988dfd9f91329d9e95dd27ccba87412bf8af7cd1c837abba04d00dda4a3a02352c");
            result.Add("km", "e39115c9b398fa67672ae3ce2673de882a5062f3ca7d18e901b7d1766a6a44b45b6c2c6dadb3f400345b84cfbc81b5ab6ef6ef4ad9875ddfe58acb0621a50de6");
            result.Add("kn", "ba10a4eddfef270ec10be395b7fbee1535c962ed74b924777abe415b4d7860e0135065abfd7c085cbf2e7d99dfc84a530eb80c2bb5b858f72d1599fbc140338d");
            result.Add("ko", "a27c1f3a3fb329f33a8ba323edcddef2ce1005fb2a1deb9f399bfd5ee911dbd9d9549202043df44b54977ed73ff02e24829f3f9fa1eadaeb8b9504e040dcbd2e");
            result.Add("lij", "ba7faa1d7720ffa977f664b9dd8016b99230926965de4c4f82001c1ce05330b3c89351bd98748888425637203bb5090cf217fe1327a28520cf13900c454e882c");
            result.Add("lt", "fc37196125677b23acff2d4a1185666f7540fd7b4f8e577fc28d3d6ac0f3d711f1467f3dbe4b31bcdd6715b9abadea6f7a0f1ac5815b07954ad644ec7fcfb775");
            result.Add("lv", "9143bcee04e2d104e20611389697b252cf72c50ecd28ae776df4be9fe138926af9c1d42bb2e25196b1c19697bd632a71d2c975d674ef8a0d8e594b4f41694912");
            result.Add("mai", "c8b507bfe418676c35bde14c5c7dfb10bced79d6ec0ce9fef52bbf1507a091988610cc29780fc91f0d8afc1d102b6f92f6f6836610faf98c5546162db2c28aaa");
            result.Add("mk", "8d7f97ed24115d0875a338010e6b6df00c4e537ea77d94cde651fe0dd54f1bdd7203b75b4cc3c4bbf4d39afba6ebfcf99e29f525f48baca2f906ad2fbe59002e");
            result.Add("ml", "ea8459734ebfc5a89634be8245efd74d942dedfaeecad50ea382c1c7c17b013f46c56c708c3d024b4dbd50672d5c974f1e320f2288885a8d2ba83eb46adc8e28");
            result.Add("mr", "facc6915ae5ccdd8b405923fcefef77efee9f06a093737d7f26c182d7e707704c9d1f0d44e66086a73b5ca777c665584ad26fba50ffe1bbdd668a644da8cbf5d");
            result.Add("ms", "576d313e4dfd9dc4b17db79c11abf1be09683ca1107ee1f3039ee4fe20704b37056f9cb23833c73046e2ae3eabbc9aa8e910af7d00fbbc0098c5c04334edac92");
            result.Add("nb-NO", "cf341b523b3d1e3e37ba4247727f185823a89dc6acd86a1fd3fb0face48dc22a444b2100c8dbfb161defc5c93f04f8eee6e78a906f3c9973dc37c9536558b392");
            result.Add("nl", "26958515782acb8e69289ec9412bb74ccf2f72b935283bc200ddb1d314ed3edc93317c195cbcd407548a73bb5478bd71b7960ca4c2245ea5f680f8b408312f0c");
            result.Add("nn-NO", "9ecfe5118f4fabae1a5d1bd9ec540f54b6e00fac67d6f435a850d5596ae99f623fbb0620255e162b8595422cdaa93ec3ee102ccc29f5535abf882cd9c470ed5a");
            result.Add("or", "13209395377ac284347db107ee99cf14a23b894e8fc22776f28a12f1ae26c307414a6b85e64ca6f24ef29b641e8a37f09088c6d16d2f62f37b9b69a24c0b21cd");
            result.Add("pa-IN", "a85823f6909497401ff8790162c1be0adbb91d4e5b73ea97221a0a83b04de85d332b4afb277abba5cc0a13ebd1b77b75809d44ea137345c2271c9ce812885ad5");
            result.Add("pl", "f78aa01308e71dfc70ffba7b16769f7a5ef9c35af5d6d181b80be07ac321a6a4eb8e1d0e8cfaf5878a15426584bbb7182f59e2a3760dd8945dcf2ac5c6607219");
            result.Add("pt-BR", "906812634d0e1159ad9357d64e1d44619d7d31ec74bf91c9b655a0a0764993d1728d79e16d9d57bb3d6ef75bf9b6988363e102c31dd436050cb69e65ba60b9bc");
            result.Add("pt-PT", "cf3eaf8eb22e4582bfccb1c2d7c2ed8c4e1e6bb699b868d800ecc3fe571a34c4fb281d391e6659ca234b3cd6c882899494ae4acef336ae56bdcbc8cf947149ee");
            result.Add("rm", "415b321200a0306dcc45e746e470c132d7c3aab5b83762971f78f3829d68aa1d3e73a2964cbaed0eed83e8580e0e9428a27a29283b8fcfa8e821ef10e6c6cf0b");
            result.Add("ro", "1b3febdcf0f757a5268a333c993f8f86ee4c3c10e4e68ac1a6a16f6ef5edc22f1c1f85492b1b2fb03d9981f98cd7b2e4e17b96de9f403c3a7a952fc46cc6d2c6");
            result.Add("ru", "37570c3297d683af21479ce1125eedf28ca615278eda0529f771c62ba7e291fe5696a27c39cb7fa2729c3535e743835faca2295be8558264b96e5e0c9d098e7e");
            result.Add("si", "716ab800948cebb447874fa9e94d09464064e950862b9510182cb8eb1a9cee4983ebe81995d6067d831524d761c0de14d463b7d61e3f12cb0c0a89a09e4fa87b");
            result.Add("sk", "af21fbb43a002c4c781d63f02f37fe3631a39d88aea64b7f5db8b36509cebd560e402e946e2a4feb9e55ad3eca10b26e8f045fa783c78a96588788b591e95596");
            result.Add("sl", "a670d61bbfc6b2433b4550fd0b024257d51a0180910f1d6ee5e7193d0e7128375293f17df2086996ec53a403bb6fe64ec2835d8aa9c0b1a1d4a42537fe284e50");
            result.Add("son", "7e9b063dc87d30418630575cf8c356c14642fef4f6799fa1531dbd77c7dc4bba9dfba7edffa638d19fab54f0b1739100412c03fba8ead88735f27e2817d8377b");
            result.Add("sq", "317657f48e73170256e59239d18a037bfbb66d59eb33017f958dbd7d908a63eaf239160087067c44613d5b2e3b390f8c6bae3da90dca011007b747eca39569e6");
            result.Add("sr", "7f9c14ff050149ffb66e8c37849f92e47018bdae15a2fa54c55dd8eea763f8c637cdf36663708f6feda294707150ade11098b4ccfec60cff817f7c87a490b848");
            result.Add("sv-SE", "098ce686d5630785e4df1d34197f1e3fd06516f922c4f2e24e0345ec279527fa385096b4bea60705dcddcd1b01ec305fb17eb52341bd24391c06d7f2d23a11a8");
            result.Add("ta", "8e69170fdf9732c429c3d758666c36daa3089cefee96e9675d3c33d67744f44c3953d86fe64e07335ba95c2275c6dbf563e2f532a50b725ec57576d030362f1c");
            result.Add("te", "0f2c1ba44690922075a98a2e3cec2401906ab035c533ccffd33262f03483c4441e66a9e9d3ea480a603c28699602640b4adfc233dc0414a41c980d72286e4233");
            result.Add("th", "677639cfd365c40fd6bfdb3854a402f825a3d1592db79d0e12674b17ec0846de8d386ac58eaaaabc9cea8856fdee7f807c49da33280335ce33500f23e07cbafd");
            result.Add("tr", "8b3fdc120f26f882998c201fe8da1d7883ebead4b28af8b72ceb1d3a5eaef3ed3004bc82fcaf39d3872268c5c2375c3cc007b4aca2ea91974f7d3ca97cf6e675");
            result.Add("uk", "9aa417d631abe466e8a7939da4129c2953f10e9d80cc7a07805f7168e684fe5cc305c379f57f2f97991d436508ae02ba6ee8a8921ca093f9e4a5efa636f195da");
            result.Add("uz", "79224e1ca622be74cef2131ad5ec72acd58ba07c97ffe614d5ac10906bc9d391e9bb2f401eb7851c15b4b94269c5ca5e9efb676d4ea6c9f365529fef677d699a");
            result.Add("vi", "7306593b5b487e5578c3e24beb77cb719ccc96826aff955c893144930af5b44f0785337682cad6d2641b329dd8437bdd11e125260fde9fd01a860edce80fbfd9");
            result.Add("xh", "bf9e0836fe492babfd7afb9b06f72fa0437e7948db660fd1d49f4e736ed197ccd35429334f1c9a11b444121f6fc1ccf3a83b457504295d4a03bbb9cdd80b1051");
            result.Add("zh-CN", "6c825262b12a8c2278afb9fc8c76218e393db546b046b20713a9a2aab3689e2fbfa2fb94d4c3a57594ffce707eece89f8e856b9dc4e893a90ca9f892052d3f2d");
            result.Add("zh-TW", "45ee69769d20ef274f681709c51abc47b2da71cb6a1300aa57c849613bd96ee35399b93f5e007e152e8d3f696cd95cbb94af66222074b6e80e816db2f4558966");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.0.1/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "6e366cdb26efe2beb54aee2afab05a21b7fb07470f4473245a493aa124a1bc49e5e276686e264a0466d0e7706589abd7572d554e86fe867c82be184306c0ae1e");
            result.Add("af", "96406e6aad6175dc21e5cb3c25d8f026d4d110179d2ae7e5ca5803e8d6b908e443a1dcbffc3ad9ea62f75ebd2134f413b9ad39cfd0c7afc52cd3d7db6a528024");
            result.Add("an", "166d8c2554302053120ce9f5789a6d0d51e4cbe84fa1cd5f11149c9c1b39460dc4b0549f4cb7262b4984f5876adaca4ed8a45886abbf3a06ac65180357e83fd7");
            result.Add("ar", "12151092b536057f2c1c3be561cd0278a7c1fb30386d963a7c63c41ec2cfb72788dd98cccbf7c29126c63eaa7bd2c3b827f160cfc4182f23365fe4219de7bd62");
            result.Add("as", "3863039fe9114be53e19f6af1827192f8c1fae144eda6ecfb75e684a3204b60836a2dea615faf7ac4f9f35578d185b5fcf6bb3285cd80d996850d0f1c28ed5b1");
            result.Add("ast", "c35c80f78f266a0dcc0b1ef514dd176f6f11678e1eefad44c39702a2d8cd897a449a740c19d6c79b6a3249de43a053fcacfbd50b32acc7fca576634a7dce4571");
            result.Add("az", "988c2ecb5b2d105866dee8c2c091dea2652a5a4264f3cbf525fe63740187e1e0b72d5f05b62f0afc8542105beb04d669fa894a5840320fe8b9364ed102016652");
            result.Add("bg", "bf1a88e65517b31c8a723824e057c875e1a6411de54445a8f73595b20d321b11bd94febafe1f86d78f3a5ba1e2defe2b54744a286e482e5eeb0605bea08d07a1");
            result.Add("bn-BD", "21267cde47b328b56e07e2c1b4fef0b8e48e9ffed502e748aed7deb7c8d8fa4a504b05352d34a9b1480e4e0fbaa9537475b99bf292d06f35336f074ea9b071e5");
            result.Add("bn-IN", "5f329f9db1d0086e4136f6bdd239ee1f24c01ce3564afb2e63e0daccd7e4c84d34eeef1a38dd245a3a3d8a24bff846f3a7062c44729715e1a3c6fc140a04a97e");
            result.Add("br", "3bbdcfaeb1aac0f02e6be505382c23f351e10e56ff8da36eae32eee61443b5d9c270b8d428bdfaa05358bf8fb5ac3b5dd6c268a4c0129abeb68ab802984bc5b1");
            result.Add("bs", "d671d40d1da818388c0d8db049e1bfe87eaf7ce63db0fba9f070fefacbecf39d529a614ef60ff7ddfc27a2fc8d3326361f22ea4c0a47dfa17b024e2fc2799550");
            result.Add("ca", "951c9396d8b526ec8c8fb55e3255418343ce12081b1063d8c3d4752781dc8cb9320fd92413d46e1a66d48a0cca2336c8fe5809e7c02d1362619c5207d30c4bd8");
            result.Add("cak", "18f43f62cd3e18ff8ae687c0a6673e376dc8502e66c38dc50ff90c8f652fa2af1714add63ab86191294422e5cd8f5253dc28b6a19c86aba61fa2e4966316a263");
            result.Add("cs", "229ea325455747120f163382a9fc70e03e3b198a6f991049605565d5d0abb3eb4f8cbe992753e3fc8725a59efbb325813a9a01a916bf3a061a61349fa924c860");
            result.Add("cy", "1e80562372cb2da7d951ea3d42ef582e37d94abecdc1893f7481c7afe50f917f07337aee05911d993bff8ffce76f6214e7edcf7a8635a02b5f6f78443d449382");
            result.Add("da", "23da438c5bf8bd63c3aa01fb43adc5d51daf959422c680d2f1ba60ffbf2e1b7d839808022799ad99f1198a49bd212aeb517618dd7e9f71df0802f4ef0a3c946f");
            result.Add("de", "ebceb732e80a0b943f65e24dfc668a8a465541a6ddb6916207aae3af2f8bb7e0da6ea5d55e2176d377f3c9f09cd647ef17a3898a628ca3be96b6a405d2cb27d2");
            result.Add("dsb", "fdfc3d941fa209ef02a19da65a8c91afc574622601b6d650a981c11ae9ed22d532f0a0be3a13291dd83a79215ee4f9080543eab3e8fbf75516b7aa93939f2e22");
            result.Add("el", "b237dd163717dbee6c785e63be5a265899daedd706827af45207695b30d721b4ec135f210b1784ebfbedb90c55972eb18ab9b57db357a0d274e7494f1896f2cf");
            result.Add("en-GB", "8ff9f4b0ca087aee5c8129326acdb58ecd222a0bdb1d84edae1ea9216f5808825d63c5b38f5ac7efec6add758acc86292565d1608e81998d203c4c6d50c088a3");
            result.Add("en-US", "12401b9d67f78c0428891d6f00c21ec7d704b3edb0e38f79641b0e0b421f44f284841d0d025605c247691c219db80ac26d421e0669c3adf5d4e3e42175e17e08");
            result.Add("en-ZA", "c639601545d3b9951b9eed919fea607982b2ad9452304ead781106e9167996009405a3c8a87e6a486c65d4b4af72869a8314cdb4c4d73867cf7679c66c9cc131");
            result.Add("eo", "c41c0175b0adfc4fd318cc7883776d67e2163b689b7e91fba65ec399c0f01e90ca7f8ff0bc36305aec15b9f1e35ee5b4c2f296be0f807e8d79798f5c3f239f90");
            result.Add("es-AR", "884428934582a68470d70c0e012fdd19d8d3be5e4ace83483f22a8fbc0a3be6eff6c174f6a0c0bc90fabb0da1408fdab463173b27e8b8bfb1c0a3c31756bfa11");
            result.Add("es-CL", "e0a8ad6b7ee41d40c8f183e4ad6b3444ba98ac58f2c8329ef57722d298ac33305d5f64447874177433b1ca8c8158b1694e995771f8ced31c5e79f1738c7b319a");
            result.Add("es-ES", "ded051ba45a21a2028f93afac949f3e51ee449530b3759400ca1f2ca6c942244439b81c065b194dbf09851f2ac2a39aca0380e2d808a3423406fda974ee5a141");
            result.Add("es-MX", "c14a549b57de81700372319954dd8bdb934e430058696efb31c2eef1ec2ebe106bfb2a20903bf69c2a5efdc6f8fcd2ada0ef75d3e9d4ebe1a714822c2d4c3696");
            result.Add("et", "aa5f4eec79f11dcc14cf075f1aa0f8b7f81594b17280f2a87c3bda117585b3d67e57c342f4c9527816efe6f597317eea6597fbbc9e03372b8176fc1023a448f4");
            result.Add("eu", "a09f8300953b01810fda0cf31d2ed2d0c3603a1856591330544fda4e379a7cdc8ab69d606d12f0db623acc90ad8a4e9d98524d8d7cfc8c75dd233f424e360275");
            result.Add("fa", "05eb8b14f1fc343adc4b0db067ee15829948032b2027b9b51797eb4ca70f5766cb339cb6eeda256d04ad24c9868af6a6ef84b02f496b3a1a643c5e9f416f5302");
            result.Add("ff", "494f05afd3c6b1992344f9c2453404692791ae351ac0b113ced192e1eaf2ba77fee245990f3c26b7624e67109b6c16d18b33032aa12feb7930f30b5084a60264");
            result.Add("fi", "41e04ac34a7ca47bbc0ca576d640cc785536c709f646c24b9e8006a22e6291ae86aa9d1be47b85f16a42237c2e50d894ff971d2048b6ca0225991075d95482d6");
            result.Add("fr", "3c4043978550eeedd70efa097c222c67266b4eb66effb7b12480c910808c6f3f1096c71a5893cf34f573b82f27a5198745c03407fdd4587a12344d7f9408e838");
            result.Add("fy-NL", "879d4d50053d9c8b8e63e97e5f2c66cf8503f33ca43733e3b96fd77d3faacfda55b2cc248264f6146052d73060cc459dc54f2e4e2103c58058c30b5b14aa3d96");
            result.Add("ga-IE", "3e4e3be5e02bd9484561c6c4f7dd8bac5d0b299867e75dec547fbc3326b344e5eb07e3ccb565e1897cbd822183f8d7ee83bac423f777813000d9cbbb67e76972");
            result.Add("gd", "965b54baec639b44f00fb64b312fbf371d141c9b969476516926446f276ab84631038200ea4c0c5fba9ebd1183f3371a6195486a4d4e5b98eaa0bbb9e8d55ae8");
            result.Add("gl", "421ab3a37487e78613527fb9268312c5445af4a148266e1196388dba886c0055bc64cd119264610cbe30fc2687e8083de92f33256fc5fabdd9ac3423f5b0cad6");
            result.Add("gn", "a66fe2ba41720a4d28851843f0e51bd457b72ecbafb26991b3e8bb4e2f9c06648f979c8d0445d5c13827915db00a8e2cd4cff13d8282162a05b4541ef2f56c95");
            result.Add("gu-IN", "4adbaf3d8d1685b03a57517eca8490f26f4b6fb0c8413b6873b155835ad743b174b732332bcb186df619afb389172d586c96512ba4135decb36e391bec7f5d86");
            result.Add("he", "9429d5135800c997a0ab53e4ae49c9a34920cbcdd6e2d83833d3ccf0a2daf2703129fdf81fa00f62fb5cdc942d25d1e13c1cd1a01a8ffb4fef266ddb8ce7838f");
            result.Add("hi-IN", "3c7e1a0e863c94bf16dd80db827af0f5c2da1bbbe4346a368e951a5def7167fd76d858974b4b29edc9388f420c30ae8bf3b811bac611bee5d08315cd841aab03");
            result.Add("hr", "13ce5101511294adc55a5861626fd16f29bcc7dfe21a1259dc0ed807f0d91556755a63d6cb320b4c2e613484b7f706c98c38fd1c06747f3afe1f5b8a6da859cb");
            result.Add("hsb", "1b6c8df61d21e91c8bc22eeb54407988ab9ed62ca7d6a7066dee8d0b6cac7e6dddf50ca769e1a042093077e21584c2e17cda3a4ff19e7680aa00c4b641c516e3");
            result.Add("hu", "1bec84aadb8e4354dda81e56ba541043b5e27cdda104bf8d98a1f5cefea70212e8c5d9d051f4768765464fe931eae8664df1afdaad08ac0e852651b0a719db2a");
            result.Add("hy-AM", "3af2711860d96d0534cfb4cccdd8bb01ae575db08813bc7a1613f945191e7602c2887ba707429f3a7aab3c3b765e0b96341be2161937cfe791911d96fa0c0751");
            result.Add("id", "aaf3c32c31e50ffdb80009699acdc228ebeeb16f47ddc705d150dd6af99e0836d8a7667b7d9f8d48933e3f6732b2158de801b18e97e898edd1610348bbf31d22");
            result.Add("is", "7371dfdda26548253a1e9c3815ba362ba4ed7b97bdebd3acc983fa1939c726d6b3c6c5720f1a25b838dfc53c43f2473c7d2af6d9ab1169ed40cd97417aab4e9c");
            result.Add("it", "56282300e49b17c94fc52c26af5da71188bd9970dc0452017c1462625fe545c5f6d29e9c2f033d2253dea4ff1939a11402eae78456aad350371411c96381e781");
            result.Add("ja", "13d4b8cbf6d6894c42b66c7278e32919d38c91809f06da8e98e95321010c7221c8800bde961adc0ec3c36eed678a03b7356d47ac73ff8944adb40c10591f6d72");
            result.Add("ka", "77a6d937cc14d068408f3b0c3698c9d70e108c468160f747b4bcc44cd10e0cf915fe50121cbe59515be7e3bbcfb930815614fd3d3945f49837fc9a0bdb2fb12f");
            result.Add("kab", "4a848019047f48f83b077d7e53efebfb5c94c53a1b8ff76dd4c893e7ec0d24fd65e7d02f1fd309ec4eee6ee58b8c4f3d5dc97d4f6a643627d8b0d9e5e6afa00a");
            result.Add("kk", "be5b40b9c4ac2ea54b773f59ccd64649efd432b7896fb4bd093d3afc2741ba18e328126a62bcb02d1c7934a98a6cbecbe98dd8002840d5dcfe9e360e26705e80");
            result.Add("km", "7198c348e82595beba24c3159ca7e02bebd7818d95e47ebcf75486dd9df7de0d6dd02c1b5f3d1c9eaad2cd9f31ce2d29134b9bbc4eecb68fe4300a3d5429b2c3");
            result.Add("kn", "0957a256bc44d329811d663fe2ca45cc4460f8aeaaca00143f92d0d510a8c837b0f35069ab0240632d66ea989da09c86bc2d17acb5428d68b2e14bca6993c852");
            result.Add("ko", "5fd23c68b5530ea732fff04a0859bf29c70a182221cd354864772bf5afc03e923b8ad5ac387171540ecbbd5d1b809215a84f0cced0d00f2a2b04765dd854c0da");
            result.Add("lij", "5bc03457aac2c91d1d26eece3d5b824819692fcabb50bb92d9d8c06215ed599af2f5926a6f1e9514b377a719631b79ea3206a2a3250c7a39a280900677f9065e");
            result.Add("lt", "f4b615232b193dda3e283033e06499592418b1b7345fba034976d521dbcc25bd07ee0dbe3b7c5267c18b5d22dbd6b00e0fda3ff2cef7374dab79abf3494a1acd");
            result.Add("lv", "4d82fa61e3e1262898d7d8a9d79c75fedb695b03aa1b35edfa4b37bbc6811fc30f77c078038d45e44885acccabecabe555faa190a99a6107676973fb0bbe3085");
            result.Add("mai", "3c60cbbe4cef0773ba77f45ffb4854deb7805e98bf6c20a7020fc4e9de222d05c241a4594f65b9733fdc13f41f6462ab3e86b6283129088d6c58ef506af303c1");
            result.Add("mk", "fe98a164e6688d3eeaa3dbfbe89f726181ae736aea17da59f4aeaead64897d0f0e909aa8beae20f7a5736a2b7095d2c51c317c669e3f96f5e72375c382487468");
            result.Add("ml", "14269744897cea1e20db9cb7b1bfe379c1a8adb072de6ce4abdecb604e6ee60781ee2dabc2fb1592abd574ede69af6102333a077aeb110b8f13fd46e997a4121");
            result.Add("mr", "09a138fc9a0318f9a095fb72adafff0ce4c964bdcf621667efceabeebe7d353b09b3d7c9cb56e331ae9a3139fe9a03173f8962d6ed5cbcdbd004369d4fec1013");
            result.Add("ms", "f86581a20a5d50b6e818690acdef0b4dfecff8fa93e4b46cc477de0d49adf016f0d1dd3c47ba324a54f1332fd16539d143561ba3219175f5a9a000bd4d0f59fa");
            result.Add("nb-NO", "18f40aca9e804ba9d10e4f887e760232ef105ffd56fc1c044b2fb15ac23222ebad043a0c36941caa25b9154ad318f32eba6aea85df2107a261ff7637bb32d4b3");
            result.Add("nl", "d629879e64cdad6fba51585dc889ae36003b084b1005e3a2fc18e58360e37aafe4c41d6c840768a09bccd0b6377a9c9e4d77a7438f3d115b0f6fa497f235607e");
            result.Add("nn-NO", "5be9c7ff06b074c9b83b5e43f1edff7fdb0b0fac5f6b14086215b40c3b781f0411e78df5d8b7bc5bd01908e48c2e6c5de622cdaf39e760a4602e4fc3acf158c9");
            result.Add("or", "554589ee705cd286bc368e104286b944be7283abf804f681f1e3c2a921546a695044f1041718e6a061854d849138c595305509d6b7313cc9b95f0c36a0802af6");
            result.Add("pa-IN", "f7d84390add47e73e3e6d306ac4b549221b99dda76a91e8672cfc57cf94e88b8318c1c0daceb03841eec0b8cfe746d22558d61076aa237639500b49916dee259");
            result.Add("pl", "f14581398ece1e93705c292c9b25f1dde845f7125af057f79898c363e7b9c9f69897892fa1854068ae994bc85b611074e59ff9f567aa7d6578f5aba221b7c7a5");
            result.Add("pt-BR", "6f9ae90b7ae21ffff5abd9892b1ebe3ef7c1ea0abe22bdb9719afa443030724f8a19c68b49d79f177b2257c835a0b9f7d5e975b27384f6215c5892c4276031d0");
            result.Add("pt-PT", "71f93f9709f01f6f99b0dade9ddcae6b237efbf08a483fe90c33117cf0957e5407c07f063e5bfe899c2660150833e8fb22ff9a44a0e8d57b99343930164ced9a");
            result.Add("rm", "3ce57ecad98d1706f45b5141b7c95aecac79460ba3a8165b6d2fbd15efe4874e20ff2d5cb4d9f8f30a8a1365c7a02c3b0eedff8f381ce179f16d91c2e7b55772");
            result.Add("ro", "51d6f1afebf42bed082e332b8c777858167d4a4c7b90607116015462467ccdd5527a0309ac47ee99453df0387662aff4fda8738b01622a4ff1349403fb9a569b");
            result.Add("ru", "972cfe0a67ab741f93d627bfceb20c1612b0e2cca44150f54c8fd8d564f60228f9e368ff729105baa05edd4202d56300281f1329440345173844ea53d9cd88b6");
            result.Add("si", "f55d8851fc6450fcf60f4a4726d6c0576fa7ffdd85dd031878256d06c7baf48ab96f2fda20d181e5430435f7334c381ae21a85c902cd9d7fd24f0714a8034610");
            result.Add("sk", "ce27c5c2f53ddc2150a6d02099931667514b7a847be3bd8b4f7dda5e0fc4de99aa22a11e07f303df8de18e19d765e9dbe5044f082500a7d309b6bb13b4281b37");
            result.Add("sl", "e126ebf05eadea21b57c4a56ebccd14bb78175845b80939440d7095578c7e75be38af41d357f49441511b7b4a7812010545febfef754bc612275b387844bd864");
            result.Add("son", "c820d320e68c336c9889a6c4857adb8bec00ae99084d7dfa21ca274c4f942c5c11744867eac776856c6bc475919556973faa2b57f2ce857fd8c4fd2f89f32819");
            result.Add("sq", "17853e2cc871895e9c2c28929327b07dac415f735e5b8c7036d32b765be18274c71e3ac1385b6860db20763ac28e25172e08b9f5298baa831cecf361bc966c7a");
            result.Add("sr", "a305176f7824a3bde0ce64bcce5c8fc393451052a8c20c1b0a7bc9f96793b3772e8978bc409e20e2a270bf470eb1ed5955d342c6fb5b32eb7435eb3d67b29dec");
            result.Add("sv-SE", "3685c6ad95efdbbc3a02aac48bf9984cfcd0cd4c1be06441ebb9bffead2116abf46afba6f1c83be4bc391d4aa8c9e0c9dcde3828bea0ac6503bb4f1a2b213abe");
            result.Add("ta", "bcdf32cc3ec35c282bf52139f5eb58372b2c33e2fa681f301c0de5b7f1b37a2ff98480f87709f3e61bbd166d981a7af8323dbedceb060749c5e598b1bc1b7aad");
            result.Add("te", "c6a87c6b5e8c3403c97f8e746db605edd03d71db80538d1ac06989fe4c47d81588da62803b3e417fb61a85dcf4b52603dbeef23e5f9363d29b79170a80348455");
            result.Add("th", "35e5ae0b446a6d3785e91ec6077b8b7109242d7294da6a12ca24f84c1e10f036daf4a930385e58ba9cfe6a5b336b48cf73595deda52439574c2c798cdfb0c8f9");
            result.Add("tr", "382d4baba5837c004c18f7ad46bf3f450ce6713402a8b5d779a87ad1ec08fb64fb4af861d584f002b0640af48667649bb5d10642d7e1b8ffe4628259aa4885d0");
            result.Add("uk", "9f6fca52e144d9e6e1fb1b4c1635965d317c4c8fe6c4af38e5c66ad3e510b6e8c998482412acd76c17ad8a779968e2a04c1f08b774ab6372330dd2981b44c4e8");
            result.Add("uz", "d14cdafbf28ce327198e84be4f057fd4198e2a4842ad45b3d3ac28f032abe5354f68ebd651fa09243b7be49bbce0dd4a671605294648e26283a927e6996e8b70");
            result.Add("vi", "1f500876c3990385b860ab9a1044a571131079b672d03d190101e268e02f61b69898bc2b05ef244dbea1c55b5a0b448130ae8c690b6a8e6396627427a42e5efe");
            result.Add("xh", "74cb4c7a53481d0be158cbe2ab742f6c8606877ff9970a639360d93784ad6e3f8c11a47107517e7ca6325bd0f16c7525db10ab10e3d405891ae24987000c4f41");
            result.Add("zh-CN", "9844400963354a6c13b0e953ef67ceee811f54e85af8406744d672d05e4907b832905ef3cd4584ab3264f20c81dcf0931ba9dfb9f9d8da32035fcd8e0cfc8e52");
            result.Add("zh-TW", "b647961477a57285a22e02c48d3ce00ed6bef216d6e9eea5dc23987d2ab9445e488706924e4cd20d6b6e396f7bf772d30c7680eff3d7f3907c57685b0d996d45");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "52.0.1";
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// tries to find the newest version number of Firefox
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        private string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
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
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
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
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } //class
} //namespace
