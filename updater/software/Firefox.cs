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
            // https://ftp.mozilla.org/pub/firefox/releases/110.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "f3b143fb223864f5b575c43787f9bcbd3f4e420dfa56885fe792845e2c749479caacbb23c85a635b44aa7fb6b8e394d398664f2fcaa058a10166af3fc0fd0576" },
                { "af", "681bdef24e01489227f8c83bdd7405c623eb10470b5c35affcaf5df714f6d85e3b0fd6706e46f4dc2f0bf1ad3dc6d5453a1bab55e39780cc4bb3bb907c54e164" },
                { "an", "245e94e9f45c983642a473e95b8c781c04d680f04a1a47c6a6efe65b443b684983d71bedd4000c82c8163080aeea704f813ebfcd51aabbd32fac4ed5f5b4b20e" },
                { "ar", "23488ce977702d362c1c621b888e7185ee75e85bb2d7fe4f05ba8f2091724e4925f05aa727c73d00da53c7c677cf539c6f0db4e0056a34202baa5831ee77d32f" },
                { "ast", "b4bdda46c0552183703b35cc8b645480ba22de3ccec23d637177a731d46f8ef010fc88c52561ad2adb3e0386490bd7cc64c1bd879be8d10a724d885794b9c82b" },
                { "az", "c9871219f2d1efacd9bf9ff3337fecd10866b5f92741a7f447781a25e3bddb360ad9c9ab3e47d14b9970c3efa6e12f197c89805e9a4f5f393299adb9107774a8" },
                { "be", "fb7d0232e4f50c7d17ac5eeee1c35aa6f03936db335ecebcc41d1a8dcd66d76359d03845f7f4be564cb3d2d9d9923cfb8b8821b9b856d381e26d122fcbd624b1" },
                { "bg", "beaeedfc348bf75075872b24dd7e61dbb44efd4f2fdbae7c39bcf0da47efa2b3412e4b38a8cc30987ac2936a7a0509a10db4dbd84be64f4b95ee67ab500c9d0a" },
                { "bn", "173f0f7520492b17fa87facee3deec2ea25ff60dbbcca15d308a366945613a425dc4e99bdfc16b07cf111792c8372976cdde2b6b30edfcaf8634cdd1ea13ced7" },
                { "br", "951dab6c5f0281f5f454ce0a5d7b75effab7f69032c8e2f00395c19e3871776b66da0ece5f30c913f02a049da78322265c12bfec49ae21b07d34cdb49aad429b" },
                { "bs", "9924a79c14dba766a6c1a0cb0c585d51e77062f97b4b99b5b71827ad69b824f84fe567c1b71a1f07ba123f58e10a2dddc0631d1797e3db09e569430f31f95358" },
                { "ca", "e5644d065206a1d47a379ce29a29a203e77121446c3f6c26de29a8d00f97fac41db2ea6af67e32ad8e4fffcc0ccc2352f3ed883de7e46ae7b4444370f0a02d18" },
                { "cak", "5b16a7c924b82811a61d0203b2d40508778e75f92bb11c957fff2ce273c265fe9cc0c0a9adead5c8f2accfbe9bcd6a98425c783b977b991155657bdf7de307ad" },
                { "cs", "697589670e1238cebf4cdbb259db5fef5fa6456070798883183c6adb5a8f1a24dbc01fa41f9fffb1cddc343a43028787dce068ed8ee366d602e083719de17b8f" },
                { "cy", "814f65c9c0f562c77d7c47a3c507a0fee18d548be4612df31e177573b93562603e41912188897e22a2a1d75ccdc7c75886db868fe383b7064d3a65030f8f14a5" },
                { "da", "333d8392a1bd78764df65f25f26999edcc8dcdf8db86456e3bcc6aea3e56abc181a63aef46e66362d354436f458f110b8208d2a1bc935cb5fea67d0abd27080d" },
                { "de", "ccd80db6cc47816db530e2f35bbf82a5d2f75999f42bec50f1ce08a4c5bd2fdf77722936d96b336a1630e6dac825dbf74616d32a8c54f7987286514bdb7aab2d" },
                { "dsb", "c899d21c6361db66de1b0c0eb2486b770667fe25d410467380eec0beadb30db1d3ff7e75f8225a5a51bd8c610bb48a62357fe2922999b81df16ed841fd98929c" },
                { "el", "c2131a9af499a67298100be983cc32448d5578ebe70032f3c2d7301c8ce62d3758df138df3502b5833e63ff757186afba77b2deaa35b5cacc3a7e899f9106e11" },
                { "en-CA", "5af2c4350f48c09a9a97506615e4afbe3b5451c097a1e0ab14b57653d4bb0a6cff541a669db275a5b50d2c9773f9e3eb70984efdb122ced862072ac9083ca2bb" },
                { "en-GB", "76a501441fca307a062b667afe2822bde69f5dd4faf4765eefa96f825ee18398d24c0366a25c735235f49e330745619d11587f851e5e307f4b03f0336321e555" },
                { "en-US", "c6a6e0bbc0fe566729c30ba7805e1adb1bad2035b962799781cf87b0ba22063192d4030455c8d307db32479b4886f33c05765df067404b1e59a9c1f7a5229381" },
                { "eo", "88841fb3b942c5d4a04a3f8d19b7fbd6eb9664fa9f21cb756d3ca8a3cbcac8e1f5be844d24be4446349e7da6d8c246d2a8bf592d432de2ecb31baad3cd82e61f" },
                { "es-AR", "4e62efb69f0a30353bf19a8bbec50810e43a532ab42129dcfb260edc37e83bc135679e31768c690c67b92545ee7606ed430772ce31ea025d51b6e346f068323b" },
                { "es-CL", "751ff9e5083ce351da59c3455d8a9f5832108adf69d6d78eff45b1759c1191b9e94d765890fbad7c7c11ce2a9c5cf104dead46798672e73883572057433b65c3" },
                { "es-ES", "75f8a681b184b586269fca5036a3fafdf0780af322f0cf0b2f944d352b5652205f8fdbb2bcd018f5fe5599943d8b1ef587592b3538369485ebf6bbcee615469a" },
                { "es-MX", "ff414d8e84d7ac313374e068ea106c8568b4a2ea5d27f8aca93081bb175bc2368372d20d43c12d8e48415a831f8919a36f35938aa34ffdbd46970c128a2099cb" },
                { "et", "d344ea10149cf4a61236048488f28498524d7d63a64e28bd69643e7ad02dd8dc91448c4ac2430c10bc46afe608d5d7efbc0f4ce34fd38fac3277ffe0ed91c9d5" },
                { "eu", "e639f6384ccfa1531c4fdcac52ce3d70f5f03eaa92112a4c4e0a6df8d921c74c025b2c9ff24088b180a5ee69b2025a7fa7ac78ad46069635393f028b0a17cc74" },
                { "fa", "f3d2c37f4102af503c70be2c380dab6c9bc3bbc2584c5a74f62ff6dfcc380c7c16a3513c6ebed86c6e95607b4cb4b9d2ba027afc26c7d50d321e759d471b20e9" },
                { "ff", "c02d0c8f3213bdd23edab2f7148864f5f94e3e4cf8e29f8e2e4d1479cfb8e9f38b17a42f920a37e363377ed2120ef41300db44d499acca750d29f6ef40d01600" },
                { "fi", "2adaf44f0bb0290e02627b3b9d338fddf45e32978bbcdea5035307e03afed9a99491ff9a380603d58db1feb428fee6dc77d79804c3b717ffe47d71eb8e2b330c" },
                { "fr", "ba00ae251d2cc2ffce84b86f84e949625e94ecd7a7e1dd1ff7ccf18e6ed3b445c7af9383efe1d18f685257692f13ad94f8f3acd44ed0df43f67fb292192fb07c" },
                { "fy-NL", "9c4943a5d3a41e336df6ed6a5d1bca53137f315b9e50e890e22595ebfb1ceb504bf50e0122b24d1b66cd4f868cc94cde38f2f0de835299b948b756c7494c5259" },
                { "ga-IE", "230c4eb8e0a99ee284ba624d43e0f3e909cc6b839cfeb82acb9de2a0218474fe08d9cb9588823acfd472759c0de9c599d562635c22ecbb3de2bdb84df0cf33f1" },
                { "gd", "653a7a8ed51afcbeb27ae49d0a1b137c43084ef1d7ffb15b8284a43ec5c0fec0e4796fc4ffb217636f4b2f8ff760a12cdc83a1fd3153864a6a2f39b98d43b29f" },
                { "gl", "f216b7c75703ac2664d2e1c33a8d02693244e1e47a27538042ec4a995c31478d0cef5f8d92f763662acebd2ced5cb611c003f7d1c8c3c2c0867ccfd90a8627c7" },
                { "gn", "89ab42a179f92d46f1b1f1bfe741ecb342353923fbaa513205c73c4e18e8a4cf1fdc2ca18f08950a30629c2ee2d0cb16cdd3650a42d888f225310cbdbe9e5688" },
                { "gu-IN", "3aaec2770ce33014802fcc71e5bc07ad7b32b5904726990169858f6351cbc410886fd43f28bfd96badd2a546d78361a7476a75e618110aa032ba8a075737fd86" },
                { "he", "9244e124396f00858a92a1fb631ab26b1fc12badca4ee89621b0ede5135de3e976684acfc8dd5c0239f8afa13a91388b5afa5cc9122c0e59d4af1aba8c2a1d58" },
                { "hi-IN", "1191ebfc534cac87f862eff8fd56abb9e111e7e2a06b4f7e8b1bcbacb735da10727db72268d428b2bbdd4e63d03f4c64354f3ed15742019e12fe2b0c7a08e837" },
                { "hr", "7cd4f947fee3913de0e07c7485d90c32ba902d558340c118d3fdf3f4c83550e1010092db979a6de295b8032114b7f7aa93e18e23fd82c4a9bfdc42098db01074" },
                { "hsb", "31137d7e14f593920eab6b87aa70557b7e65be61d9a39987929a3142db2bc834253f6bd61dbb53118705025f31cbd327c9b16f08324517be7a4b042279568ba1" },
                { "hu", "c362000d92283a18ab3da60e1326676357623c38f407599ca2e7cc4a6ca08cfdb9c6b9622b959e2a27a6e92517bccabce9c8ea2e335fcf7d0f89842ac35761f5" },
                { "hy-AM", "a464189913ff5330c95c006eef7b5e7cfee63810009d8ffe6c95980699e059026ced766e713e75e6cb24e8828dec4f2f901eea60d35313a3fc06b129daefa108" },
                { "ia", "e07b0c88d60688535155922f782c124c9daf65b5d27641971962e0cb651b24b03bc884d5d721f533e03cfb768a30dc1a4b9583f0e456d4843b34bae270413996" },
                { "id", "334b56ca686995eea73f62414878ecab587f9ed5bb1bc4832588c18e5cea0b6f948c628dbdffd9d5a36463f2ab59ff3a856ab58e56984e80e132a3d97a6383e0" },
                { "is", "7df9296669e120c45137e1173ec06dea8b95d91e6c4199f3e56ef792dfdde2057f0224355cdfcffc94488fe1d97e801aebc84540bcce749b6085cb80f739af03" },
                { "it", "1c3dc83aea728bd44cbc5393bfd4ed02cb7c48f18e7e9291af9d3fb54e2fe5117901284c4199af21a33aaae1d88d0b6ec90b327856f805a2072ece6ec24d5bea" },
                { "ja", "11d82371cff9c3f8292a4f8b29e107e24a3e03d9561f13a468f4e34eab63cee5507b878df629e345c3bfd2146154828b17d057dcba4dde85e67bcbb93cccf874" },
                { "ka", "46ab482378eb835abdd4fff28d669f56e52c21833c06dd18f9eed293a8df6cf3d95b8dbce88bbb0575c3be6808ada0ef1c884a1f9188099bddb5f1e3dee7d5e1" },
                { "kab", "818a774a0763849e7fa0968ff0fa92bdc2b74b217a2929e2cc7a2086d52ae471dd0d8d396ad5e826908ae041f2d5917d10b21dd9bce7846bf27aa8b44c27b06f" },
                { "kk", "df4759d5ab106bf203341bc8f5a836e25fc93c87e9225f34dc38b212951216d9931d40f8618f0f83e4ad266a4354f641cc4615e28501f342d44bc7fca5ae6bb6" },
                { "km", "d25b627b0c19b9b7af32e42b016c5bd07f384d85ac89dacd4c78c73ef2a928454213f6da19b0d2f391d51cafd73d8c9fb162f6ed2607599b9ad310e797739004" },
                { "kn", "e4be467bd2632f422ba33186c71df3cf18f030ee1bdf0c3232b7448f31859c2b5d6a1091f167f6096ab06c34bf91f1d5fb6dbbc7f752109b97818ea1459c69c3" },
                { "ko", "5e536f135c0826ba1ca6e57834851c6c73d56fee1fa16c5375d14c9158fd640830a50e79a33dad3d9180685bac28f9097a3737c10d4d3a6f9c341d31564b76fa" },
                { "lij", "d5478a760e73dd0d6e0ac4dbebad96e990f016392323e6552a1f070f05a64c93e92232e5985f5c168076a9938f7f4e09c4a1661632d170ce4cf9a17a604c3027" },
                { "lt", "b3605dbcfdc2706f2aa5b3b3053a9de988a659955516b61758c4c9eccf83edc819959995c9889cea35553429ac7b6d8fcb0840efe980898cc47106d39d8af7b7" },
                { "lv", "a4a2f0caece2cf7280e12a55a1ab83944c5182cca1705ab5ee490ca6654b235ce4f9db3791a8bd9052c6b0988fde98e22c304eb5f49a1c33379ce5f5c92fad2a" },
                { "mk", "71a84fdca7966c66837aeb0fdd32b2d06ab7ddb14c84cb4bafc614adf0a42a0f33df798ace3120a930ed3edb6f90f30b4a3deed8835bff469a69734d89d3797e" },
                { "mr", "bc39c7302b78c8b66fe0e052d0bf2c39414b745c1dda8bb02ce82117bed4a274b7f0acf410dc179393f6050ca0b3e664cb9ad34ef3e76b77023a24c6ad7eec33" },
                { "ms", "bf33b05501c7c2c88f8672ef1080d08c8b968b971a99e89f339678583ec4e09948f19873837270ff2fe6f57a22b784fd3c61fbc3156b09c0616143f932e721a8" },
                { "my", "4380a41929de0805d9c14e3bc83d0f075cd62dabd16c4b7f92ecaa7b95d34779972c7220faff81f6c4131c334d1dba49bd108e1dbe0dc4aaa26ba8842ab6ccb3" },
                { "nb-NO", "076eabd9161dba14a28e52f4b95746aad7fd9414b0aadf7e3fb2afb1a02e109da9ebaa7686785de4bb1b6884fae09e9f769e907ad4d3ed28cd059669c87d2fad" },
                { "ne-NP", "8e2d40d60b89b756333932da17f511be69496534a4c5b6c802f1e3a7b0dd73de8c6094845006046d8a96ca352262b50db61cbfa8c0fef52fbec68596c14fbc91" },
                { "nl", "065716075640a760a42e7fba9348c7e848409e2b45a76f7e97d45d114400142c95b6cc0687d96f3ded5c793f69c483d54c1deba7f01ec959ab00342209585a2d" },
                { "nn-NO", "cb5ebcc0d107b124dce2f54b2217d931cc117bb90d3616c1730479d270d0ea025215df7f78a33d12cf16c309b71d32429bd0cf24f7c962759dfd9caf4247548e" },
                { "oc", "bdf94c49dbcfb2a1a183021f854affca1e37245d74cf94dfe419e2f25ed5d00ceee08dd6860ab54cd8c3c21f0ed513fec2f9a6321bedeee1226e8a98ebedb488" },
                { "pa-IN", "744b0e5b1cabc62d08451d0a46641cee74e272e84468e9a5aecd4ca83bff6deb93887206fff30b6996f299d9668bcf4d5c669cc1b0d244f3f85ab961f3cf05fd" },
                { "pl", "84c2808ddbca5b529a8dc736c30117d365558ed15c352d624ce5b93362101188db0987fbef4db7fcf923bceb2482dfaf1352b7166f5c8e85e7dba1ba85f04762" },
                { "pt-BR", "1156c3a54ad4ffe1ca0b0ca8f31b9ec122b4db78c09dfb175fc92ce0459641b308dd6b50fd314ab3479ebcb5ad70845229a96bd85fdaef2964eaeea5e2c4a685" },
                { "pt-PT", "62d027c6c6dc6d150b56ac95985e23b40e6d72dc8d0562914edad4d0089586cb7a4d76bec0f639fd3d589a40333a467a4c6f14cd29317c7aaca572d2989f159c" },
                { "rm", "36c9bc745ef701e6b0540c609e60dc8243531a801294ad8e70034ad66768cb0ea0b6262014e3e4ccf237ec2859e4aca267a171aa47cd7a2a055598be03039428" },
                { "ro", "79c9924814e0632c6f3011bbbaa45fa6dfe2a8ef5c85dfd0c4db69a0a0a6677f3d56388a4ed411a85fa4486b2923b5a4e999a7d46516a15a2ddc2fa2541cd6fc" },
                { "ru", "8b8f1788df13d51bb5b2d63f7b83127a8a03566cc8a86ecc9c389bf384064fa47a628352d980bc8a3907d1171e5254faa8e68f5660bbaad7965a128956b82474" },
                { "sco", "bc3e3571d42628b104055eb0b7e4ba4279335360cc7f3a687d68176b0baf6d1ddee08e273380e2ffaf75d0e0481dcf56bf2279f1ec1473860a8aed783cc4a633" },
                { "si", "19fd58e434c2942764c59de06df198de7ba40ccc2dee1b69a36585134805a64577c2844bd6923832c68b0d5c618b42422105aece926efb51ae283e1ebc8c691f" },
                { "sk", "746018220771ac48ec6631ca091a03eaec7d3e69d59500213146fb88ee1b892327971e1de3ee0df2f60f9f2ebc8cc47bedd9b5a8c14273f7ef0195a836763cd0" },
                { "sl", "7caba3f4f9733c7b6cdac39eb3ae73f049159e1b58eb463cd355c0e4131c2bae318e730e680b6d4ae5478b379af0bc4528bf0d9da047299801de524b6c329d81" },
                { "son", "d30f91ce77074e266d5dbaea2b1caacf66a900cf17a2164bcfdf1458f5294320821e51e55b4b84add61c34e49ef37f5b5c45b93f61603a0bbc8ec06d57f6f3b0" },
                { "sq", "2dcc9a1452b7e17cadb9e8e133db882cde631a714f1cc10e84cb432b56448bcbcb1f4b1e5e1b124e377ab609c1d3caa40226e0fa7115473e70ca996bb8abbc4a" },
                { "sr", "c1d5ab537360ab871768b38d7bc4802d2c8b0e19869b6531f773406d9569404566e9e34b1e67ba3f125983e3973835d768fd0d2d42da774b044aef7f619294fc" },
                { "sv-SE", "1d953e587998e8c1a1fb883f058c527e9c896e0e611af013c715174d8db550f8b0d8110ad178815cbe3aa2f1538bf7795d53650e739d413250c6f2cd1a7f074e" },
                { "szl", "b3d7022d503cfcea0a8963008ce5bc62b0e605403300eef8d3f4c45d2e8ef26ed3294f74530071763e93ee67d199fedbd43baa3e50c386c6053629f5d8618472" },
                { "ta", "e8c7f90c976b9e5c3730811b478739a4cb443160f9ddd7ef20e97500ed8a9aa9cf92c9459cb4ec5d5af3419536f7ef598ab62307b4df6272f3b338b3574c5c3f" },
                { "te", "dd1ae1507911354194eec526b34fc5bf4691e90d286249be299dbfaf0eb504d5b69edc94ebdaa3b405dce56e522b7cc3437febf239a51132f3fcc466c4631e3b" },
                { "th", "d130a0f1c7c91e39cde2b4255f27cc46ea21e1bad14286d12cf8906316e3c9c99c9d62f26e87594f9361904552e3f4cbe82fc19aa3b30e8264c56a7245c1d7c9" },
                { "tl", "5017bd21457a7d26e1ccfcac43a09e5a01e727931b13f1b36b21acdc0a6bd8287f79b8fd31d9c7be5d94db4465ac4f09b11346297e9bb7381c19eb7b47a55ff6" },
                { "tr", "0c437e09e027b15464b3d5435ff8976eb8bd14b754a443d2f2aaa7879806194a90b55d93995f70d4ef5ff193b05491488351c06d6ab5d9582a04582ebdebfb24" },
                { "trs", "972598fd2c34ba28ac28dc1f7d4930eab32811b01ab590aab953648b70bdf29d8e9b54d7295c42cf691e5ec44209a3bddcd9b2722798a47c43927d73f4a98a83" },
                { "uk", "b50be21aa561204913b2580564272829e662a3642b074cf4567ecbf767fd3085651a6b7c071bffdb36eb703f28fbfbdd8126d31ae9babd87aa963ebd02fc1b53" },
                { "ur", "29ba2e2759ba4266404078317b3419524ab60fffa630f1b1c270938629394ef8cdabcb142cb9d267b05d9397e27a15ad02ce314074ef1d2c55cb058b879d4645" },
                { "uz", "d67aa0180d67a607a05b5699fe08fab74dfb7f520a9d92501dcb7ba3175b578fffc1e2fbe9ab92aece343978805a66f249030244e40a3e47a4980f2cc338b86c" },
                { "vi", "ea3dc49399c7a0524df0fedfa10f323506d61375c72343695486aa4bf2b82f4e506675f2c1703b248b43470df28c48e34bcd42123e24dfe9a97822d41fc3e800" },
                { "xh", "22a827afa7b0aa2bf960fa1e07861d83795418a45bc133b5b767d59bae9ae93aafcbcd8c145ed1db88bbb1bed9c5f199294547fce7926f204a772cc9c421f4f1" },
                { "zh-CN", "c59acd1c0241249356206ac13261bf5bdd26a09efacc2ba2f6872b968a48df93ed533f4fa6214430d708ad63e5705534427311ee2905a001427190f3d885b77a" },
                { "zh-TW", "b039a1c3882d94e0d623960b49f43c47fcc2d75e563dea7eae151cb3e2dd8b00b460acdf68246001c6b84a71d6a235fb0ee7ed980545f611a10c875b0f359fbb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/110.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "accd399161515e42a278fff426063d9505320e0c15fdd2e13158c99bcf7b335cc173414fc7f5dcdbb384957785b303eaffe7ff2ae8b25879dbc9d6ff860083c8" },
                { "af", "8dd9387ddaa23842f23f5c2698dbce6eef75321137f43b264446b5ee5dfb7dda841db11776169e58bed529c6e8712106db3596f4a88245fb9c6c7a08e9645616" },
                { "an", "467a202e48ab653acd74fa14d099aa30c443815d3ab276597bbe65c1226a3883ce4afc4479cf98330b566f71777558dcc45d19a6a4ea2228865760bdc4de44c8" },
                { "ar", "25617748978a463810d264de3710f4bb58f5e6365e9cecd5135387712f12c09c7ec3f5db38745a5b8c7beb8d4dac5163b2dec462ba7c712aca97439545b7d813" },
                { "ast", "3dbe04e67215e3ad9d482eb7af9abbfb650e4d4cfa53c36572cf97275f4f736d38804f0d6c19b403cb51f284f0e0f9cd79b03f6a5530ab47d859a9d9522ff6e6" },
                { "az", "9e9d08eb0f93f4b8af6e0590983c3ed523d03f8194aa003a4e3f91a1aaa8d2357057d298668c1cdf400cebc39ed5fafe66897548b3a1f3c80f927093e9019d70" },
                { "be", "dc29ec55c05f259bd4e7c4e6b1963513a61e562aef587d7b7a75fdc6f4fad0d6a64654c33103546fddd3bee24630f5b7c61a245558c62cb272a6bbeb9bf46081" },
                { "bg", "a2f802c4ea0f8bcaaadb185219c97be090991e6dba38d9da2d6b9c41e5af63b74aa3ee388e29e40dea1ab1082ea266a3f7c92d7184346efe34fd3fd304671aae" },
                { "bn", "a2491c72590eb87a757263a20f02ef60269dccb1f9f3aaac329fd10a911d20c2af8a8d4798cd9bd3bd244d9c999c4a8d05600b9d69db23709db61c437a6692ac" },
                { "br", "95776276e80ebd4289ff15b9e107152c1984e7a215f3ed1f48f8d363a1c1204f97f7d244c25e87e67f23fab231f85456c115f77cbe843e6f6bade0bcd358dcc3" },
                { "bs", "f8e04d7d3546b9a3e53aed0df792d714ddd817a6858edbbeed7768c7c4adee35bcea326db1f87b24423a71ade620a8a2fa67e82885fe5411359a9385d67be1db" },
                { "ca", "6dc50d066814bbeb71f806164fce42ffc80e8f22f6bfaa3682112dd7360adb8d222fe407cabe6218e674032178ea69797748eeb641785fb3c3f2761bf7f320ea" },
                { "cak", "b43e3914a2e2303f2b679af6e11b8983a3db4fe78aba4130a2a7c026e0c322bcbd25bc118b196cb13ae1bda6cc2613d21ca05d8bebc1c64d8ef902c144a1f02a" },
                { "cs", "9e60484598d0dad421c99f2121edea4d8eda8d35ac8b9968f6cfbbccce6cb1f79094db6cbfe3cab8da60744649f4916d1655cb776f45126a80a7fa7c48d6ab56" },
                { "cy", "7948961c4a30f500f2a10d22ba2f69f2882e396225a5d5d99547bee9fb3d8d6b3fada89f7978b999dc0d063dfe42c21a6990ae18cc64164e2c180310610676cd" },
                { "da", "ebedd243ce7f2bc7f210c09ce7e6cdd81d519947839f6a3a5c7b3b6b58116bca854728ee1a99d47545016bf25a552aeda21d56cb4519d0755b8647f0a72af2dc" },
                { "de", "82359558b164408c7a883f58a79df6cb63cbeb82a2153418543cc08094499a68649447f2e04583c0ca97c496f5be1152f7395a5232ef87fa174ca072478e23cd" },
                { "dsb", "da55588d2a0b092c635e3f1951621d758ee2ec05692ba479baeef93497e130b0cbfd13cf58f029f403a800c12194ed56185ccb24a00802550813191c1df83577" },
                { "el", "0e23b4efc1cbddbc914c48dd2633d17bb445af7f2cc5bc9228acdce62c9b69f21bd7af1abbede44b34ac8ffefc0d214b9db4ab8376736d5ea4f87d926e76fbf8" },
                { "en-CA", "7b79da084eeb47d231c9d9558cdd073248f71db450b96c4385ba902be52321ace6f2a689a12b3c7261f1da3282048afab75a3d84bf90fd8a0d0d3b5fe5db64e7" },
                { "en-GB", "da10f5d40639bbeb997afeecfd960b920c6717f50a27ad0645a1132cf4e2919ec67d11bf2de245edf6f8e47f396df1a3564d46297b9b5b61249f162af76c5a9f" },
                { "en-US", "15f84a3de7810ca5be35d4deb60badd99518a25f8db27253241ebba14912ad9dd1ac896198193654ac51972a0c4841277dbd0d532284271f1fb5dd8706d60497" },
                { "eo", "e34d4a4f50a70630e438a1d9cd27288a73989a0281a4ce07469594fcea1712e8e70d681c197137f385af6040ad08177343bda3c36a58b3a44367a08d2d503e36" },
                { "es-AR", "e59c38901ce16d4d96d55cb67174307dacdb8060c1043a7525471d922a64cd1cb25dc20b6c033f469195a825e7ebf52ddfc78dd34e16d8c53b92c26a9ef66bf6" },
                { "es-CL", "202b6aa83cf5ae5f4c505942b3f512b5286218c31cd26844c8a773ed08eb7c44d89bcfde7c42fd6b8ff51b42c8bea823e918273b353175f02930ac2175710b05" },
                { "es-ES", "35a377ac16f347b655e11e524e496b6ad21749632cdd1edfea26fbb56f71925a1da45e4ff57821b142551cf2f147d6735fe2e6e73f26829fbb7f48a7c155a3ba" },
                { "es-MX", "be4fddbac2b0c0e2b893496b37905a0cba12385b989a15f036e6ed5b467eafe0d6e9e00e42237168d8c523e421296519c6ef5d95dcd4526f73a2a5682e1116d8" },
                { "et", "3c57b645e4e1012d1d34afce1c7fc9c69cc4e7b4671c8412a4a18bc542cec40534b4ab23fc6a4f717346d1d73f9ec467b7bc281d95e0a117f3673154e625882e" },
                { "eu", "f44e56c0047f6d28e2ea713fcd38cb0df941fb28752e5354cdde51b9a9ab8a777acfdb3b7051ca0833a0df85d449aeed31ccbb6be44d2dcb078c04cf31b9af03" },
                { "fa", "826684101e08bb92edac884abe70d84674ff5f48606ee5b4de8ecbe4da9a983c1b26cf385632b900922cc1fc35b7b37aa4ac1556379b01bd50182bc75ffed1cf" },
                { "ff", "f0738ee70f4e17c23a78a353ca997b9457263283e126df682176073cd809cf79c6f397b3ebf22642502b16604ca6372958f96d8fa0075d5985af09a80bd723b5" },
                { "fi", "4c51311b16871eea756a414042ca9abb8032a0f14c251c2573aad382bbc9a01ae859d72883c8294f7ca949bba7f33a298f0c0ff1f85e8c89754efe391d1f0399" },
                { "fr", "3b3b6aecca3b3ba61b0a0beac49eaec20b1505e95f5f88c2c1fc663f39d4a07f88a4b1a49054883dd2acb8cded46fbafd78964aef888c75f4370aba103dda45d" },
                { "fy-NL", "b96b9f1193a4f0e5e6fe71b813f96df9bf8aff54d58e2ead28d2a3c108ce97be50696e092a3d1a4278e6b2a90d4a65956f71c02623a9fb78c095798e9bb7197a" },
                { "ga-IE", "3b5c3a6afd85b6c335c0585316fd84ec4569ba31ddfc1d206cb2d6b95134bbd6cacd4eb25372a854cfc9dcf7884be719bddd006a133684a858fcea218182d1c6" },
                { "gd", "399599847919d1110f22ae94a3535ade32889bc9ed940bd788c750ba606b0628e0ef8c9f8c8581d19c5f45b48fcdfd6c6cd28f9731031f3ade3230ce79d6da81" },
                { "gl", "c15eac5eb36f7416dcd9a5b38d8a52b7d47bdcbfa6cd0f8e148246d7663824e2e796943b66bc696c891564b33ba1d4760b81ac580d7fe364ed35114f75c997fc" },
                { "gn", "03133b796ddc672b61d8342c112d238ff76ac7a3a7bf26fa792529c8e681b16975f7fb0bf5d649ca7c0bb2849663fc616c427dce3c57646639e9fbde1a98f065" },
                { "gu-IN", "5851d49e43c4c221a67fb2df098e6acb04ea3b472cc2cd68fe094d5cc9fa4cc7525bd8abc3b20f837b42b686693619296cfb716d034a04583d442689feecf58c" },
                { "he", "55d129123f62cc34aa8e868148837bbba3cd142298b5c60a501d71159aea24ae9243b2f2bd2afa8fc7dbeea88e65121aac488bb6053c0aa4fdd8bec939b0fa4e" },
                { "hi-IN", "d9177d8865aec2fff2831188a99b8bf3114c9d7e66a01159c3415e9a86cc7d8881da7686c30b94a34f2721cf04cbcdabeabd0e3c45655b2ed107eddf22d1f2ab" },
                { "hr", "ba33bce7471f47a245f62235b219cf18f7348d7ad02c79bf8a82cfa148fa1d8b7f986942997590e2235362a9419e1c2a782c4f5c09b63704c787f6bb662bbdc9" },
                { "hsb", "3e86b9f88be3ee20fb9fc325463540a16de46f0d73b5e240407d2c217590d456418b8e7896125dd181f17562ed82c6815c3b1b31e8d3ae01783f7ca8d6163889" },
                { "hu", "7b9921587f7435f7ede6f2622b103c10ce43fdcbe902a5d81ee73f9593b12f8ac17a5f4c8dec90c560e0e5ab686fbf24ca8619675b16ad477885cd6b609d7af1" },
                { "hy-AM", "fe3b1be5b6d9bdbe0185cd4dffb0b14eb29920259a638daad537b66f1e5c0435c30aeeefbf40142ae4d3c626660dd8ed5b52d202f1b547bed65f2f5bd29860df" },
                { "ia", "3a9c981e211d2be9b32e714e0e7e9a88e80a6e5d4562c04c5728eb6a41a0d6e1c24ff02710179d7fd54ab670147bc1d8ce130bedfd0c73c7e1cf0496a26e359e" },
                { "id", "41bfdf42b63c1492cfda1c2112c91a4b19acd18fe007b08ec51185c6e02cc4dea02094461b1c6a84e0bee8dd187019c85b1f1d0115efd455214991489703e5b5" },
                { "is", "8e14d3b2fc3bfa1796006d2f4d5a67def8ad1ec44d6c31836d52a8e66f356d8972ed0e0b5bc9d80ad5c9d5cc78ae1cf78a7773971818bc0e7fc509073ba8cd7c" },
                { "it", "4e7f2d1a257cbe3dc75700f785debc6ac7629ec69624735e6308b8e16fe3344cc1d7eee55bb87ff8425a00fa44c769ea5484d4da629daaa7847c3325cce1bf1d" },
                { "ja", "6c8e4844bbd3117596d1f32bb8c2035185b95cfafa00cb286a25fd8119ce29df8aa7ccc2b70a06598cf9ac47127afb331b48e483790c40f7ec2cafb0fc878321" },
                { "ka", "1a79d3f95967cd49fa46481285761443b6fc8b3d0bc6b78471ab52b2cbb9080ffe965cc05fabb627364d5e2d3a2eb2e55e36a8700d1bf50682fae92ec8cb5d16" },
                { "kab", "ffe323c243d6a6648ce1f445d315a178310e24cdf69885377e9ca4e7eae9d6d18106cc0d97f8c7c7985500c3af19cdf8ecd264be33592c4768c7b69955b2578a" },
                { "kk", "cd97cd6567843c5af3184eb6045978d807b964622164a719f305e2f7c25968b5b3a090c394c27839b0b09d632280ed1fa3e489fd905268c11afaffed2b25e203" },
                { "km", "8af941d8ea14ccb7c8d9a2b24e29a6eef415e49a2ca5b78dd9636829110707e4644aa3eafaaf44dbafdcb66cbfbb7e1dba592bbc94c277e80d0ae72b3dd83e30" },
                { "kn", "944d4601f69d0f20afe496345fb259be504d5281d2e4cf3d5b2bb220aff4ad3d79aa2c302c049e32300ca4fd9a844797ab68261ad0ce27a91a35cd2ee43608e0" },
                { "ko", "e684383d04111c97b4e70a02aa381d37b5995394890d59051d41380d20858130f13403c1a765fe9d7b5938e215c6caae742fd34dff6d456cebb2b2c419f44ca3" },
                { "lij", "49777fc57b15146c6fab660629ba46a34f5f004855f9ee74fac4f57bbade4b6f394c5a5fd3d44b5d7d232c1f8fda193122ac9e17dd39c194da635c104a49aced" },
                { "lt", "8591f33929645201c79c0d5be0e4a44ee96b0a2a357bc80867b6429cdda8cc2fe37fce501098a4d6cbf2914ecfa66a6508c1b45fce9ae3585d5ce97c8e227493" },
                { "lv", "2c2a557c531f4b8d77d64296345e27ba271bbca0c3c2d580ed19fb967a110ea577fb603ae8ec29cb6e74c2ee266f1174d9b99a08e5e788aa4f87692eaa96dbad" },
                { "mk", "1a171f0b27df739afc4fc49cea2af06a20686fcbc7d12bd34234c9b6cc3d7065feeeb1d3a8d1fc2c129deaa94597f9feeea78bf08b1a2b067aa6cc5fa19ef500" },
                { "mr", "5ed52db404c690f9ed989a62da7186c1b5c8c05e45a65672ba899ef0fdf7ff788983256d10f813b05876e71c99517168d0560c3537e4a0384085d2f6022506bc" },
                { "ms", "715c1dbcee06a818443957d3b38fcadfae50852f1fc22befa9c9bc69b22c22f2cd239a7b1f097addf6d2f1dd05dbc10bf4bf0eae2bb56057c16cace391d8a7f9" },
                { "my", "cdc2260f079444870066783ba7560556919f4d5c80e45ac8089c300475596185b1fc323d982512b256f3468fc88ad643e7f99039f50aeb769e38c94ecd38869a" },
                { "nb-NO", "3a2ac03305848af624e07a1f1e15f2fcb11dbc83d2f262ab477d9c624e8ff596dfc50df885fc3fab0222a41c7c5e14986ef9fd6c855a2292e76b3141f3e452da" },
                { "ne-NP", "379d832b89365ef9c537f2e19d7314634204dca8b971923b326d91599ce83f650612a3fa3cadd55a57fa19e52e37ab238ede992ca97c76c5d77617c3ca9ce2b4" },
                { "nl", "14f75a4248e177d4641416830cf3d807e39620a20c8fc3e18ccfc32c18e98d8dc7c3a86e4a22c1cf3a765e65b2dd37b2210a3a5c0e04964a67dbd2c1850472fc" },
                { "nn-NO", "85faf86ab465d7fa507acfdbd35081539206a8f627ddedeb99c6d3b40a0f193ce55e03b2aa869fdda3b3aae00fb3c9ac7c88c59449d49fe4bed126de4ada0a19" },
                { "oc", "2a85c4517987487381cc98a20aa38f094de1a2c1decde8883ec9dc2877f21a4df23631ce3e52de6dbd521e24f47c7e6f60cc96a89727ecd32284a367bf7a099c" },
                { "pa-IN", "76f34678dc084ae0f853604777415c871ed5753f1a4fedb207ae922aa4f287a9a2a5d62831e84ac24166958ca5fbddd45fd921dfd872fafdc994a767936135c8" },
                { "pl", "66a7b74603007a7f8e7c49e14dad7c4af5c46919cbb39f7044bf8d3bc562ba955324443bb617ebe0157712a99ded745d887e463190c466a9d7fa861595cfd9da" },
                { "pt-BR", "365cb6b4c373c5e0f967beb7e25ef709e432272b995757c729e240c386158470bdbf212f0ace4f429ee4c3196ad6c7bd8179d85035644564ea2040442036af33" },
                { "pt-PT", "53eb83ea9548f505128132ba79dfa8fa50a814ca40b53a479d7848bd38423ac1536d9bf9514848ad499a423ab20c4759b33979c1b8f46a5fff12d2633bc9976c" },
                { "rm", "b0a6dae2d0ba9f90bb1690ea76b061b53e38b205c61f85a5f675e98e2156f29959a6e2798a5bad6217a28cdfc3df0189350996592a6b6a6e0ff0747e67bad441" },
                { "ro", "84dbfb98c0f8722591148d747af18eb4d05b6b77652f4f293a8e5edf6887c493f455440de630102bbaaad52d4db671d214294cad93b100d5fcd6e0219d609dbc" },
                { "ru", "e0a72de1714ee1786549d5a87b8786b40e1333a3d078b2737c9eb24241bf5fc07525ced7d1fac7b3d0c6ad86fb170441cb7d9e6843b5784b3e03f4b702e7c04a" },
                { "sco", "4d320a8a1bc389ae2da69e53c82b60e43599ede6d2672491a15a53362ba57f1df09795c281dacb18f56af756f49726b07b15d883e8a006875305ceeff6dfcfb2" },
                { "si", "d4e9854a22cc49a073d829bc9b6c3000ac13f11c6c4d8d6a2883502f58f87be6ae6d205af22b8fc0383fa0a94177b8e84dd42df35129fba4f29134ca4ef7ce84" },
                { "sk", "627893a9cc7b6a1ef59d5db979e532c094fac944d903810087de1dfb30e450ea2e08d3dab216ba12c81dd7317b3523a09091407acf3ea85774b0d2f73bb5508a" },
                { "sl", "075abc7923952d384d7b1ee3b5daa128686252e161cfc6c5f8f02b0fc0c10177a69308d95aa1246dfb4e4e8816a963d94a20edee644f1e2891387c2c07c9bb2a" },
                { "son", "e77c4d3bd09a180a518d077700fe8eb1d0edd08494bc1b8d27192861e91418a1fdfc8ee672d083680759a830d3f217bc9e5e43e8ddd0150280d2b255ec4e64d8" },
                { "sq", "0c7dc534c73db380b72196517845c3c5a322c1ed50474f04d4c210a4df0d21fc57c27966854d0e00378918818253570cf71b618558292853164d4a3aa53bd0bc" },
                { "sr", "5051d2a963f86204271351b5c69fdb2eaeb4c337497e3ae06e9cdeb8418ebe285fb04831054c4d351b8a249950dd148937532296032e61b69f041a1306194590" },
                { "sv-SE", "39f18a509d479b2b3dd6cb4b9650d6d7d08f00cacd81d7b8af40f96cbe457c8cb47cd487e4603e3603bdf2e03aa7dd348af6f557d392b94fa4bcad0ef6daf028" },
                { "szl", "2cff69b2b6b242885ea28d4dd6bdd3c577e67d1e7479370bbcccaeab43e87e9d1e58116701d420cebce151081732c5ecf99d3a386eab2cd2dc923eaf02cce76d" },
                { "ta", "1ef38d3b0336e259e83da12c42c87d7094b9a4986a0487bf2ac23d145f901bdd7b775e7de03e7afca283845c5cb77f1d3f50b3c796e87c5d04f768fa37195069" },
                { "te", "4a31954a9cfea844525d9cacb74c5aedca2c48c39d56cac8f703b61723a63e0fffc42542db2f3ccf0fec3861c8c0b18ffa85b37d36ed271140259cb9bdf4385b" },
                { "th", "c32ee6796998d2303ea95dcce3b32f1f45477e50488fe38b9f6069f73d3efc22d8e26bb26aa873c4ea3326bcb205cf3848a9ca7a300dead2fdc78f241e6c8293" },
                { "tl", "5e2bd817a6578545e29a2a79d1bcd9115436c0eff68852f77f556ed04dfec19d7de1770f69472528506ab20749b3c737a62c4782372ceff002073d3b2fea5377" },
                { "tr", "3a22698effcc1cdb268798eb101759bd7606a11083a5d3c9f690f443211d9c1be399ec8586f28015873579ea38012dbb30fe907f487a16159e605716d6c9d573" },
                { "trs", "db5bf4dc6e22a209f4ecc1b00a1bdb3e899f1f155bc6bdd0ada91ac72e45f32aebd2d6b773f7b03022f839cc87c9f995aeb4f3b7bc3267ef6f025cc292deb8ce" },
                { "uk", "7bded9b05799694a5f968078ec238b38c9238d492185396926b3a5969b2b69dc3bdab44a5037529ef752c624fefff53ec253fe16d2ea05a38e0740a98d7c8aae" },
                { "ur", "7b5346be2749c2deb9de8d52f59a13a9dfdb41c716ed6c5507f3dbe0f191c75d9a24ae18c32c9e16a294ebd0a3a990f115db7a75ba5db4289dc03c85d9b50eed" },
                { "uz", "7a2c5ff2ffa743013a8ab31f4e44406535ec635b775299655050b8ff817fe1367c96a84bd8295ed359c369832d3105ef1595263a188efe12f5e474bd7803b5da" },
                { "vi", "9f85b224f100e71c5e652359980caf5048291f498d1abe675a5713cdae0bf454e6b2d32f1aa89c5fa8184617d7af0aa354517d688cfb7e9beb25677636e55954" },
                { "xh", "148e40c79eb3832e985fcde20d4548ded7eb66e084064b74cebeeefea0d5f14596c85e0fac218d6a2aef2bad56e047444e0147ab5a78bda7c1af6c0b7ea40f7c" },
                { "zh-CN", "f9b75917dd696b9cff91c715ba085af7568cfdd1aec929bb73a357a7496af0325996155d82387b5f53a4cf2c55c3d7c9bb3cf76639554348b04677023dd49aa5" },
                { "zh-TW", "1c6b990ff191b9721cd8017b1274410dccdb9bb81a6b69d951605a67dce0b7252f02eed9532f1d28612c84207b7d44ad913c4192d7ab456ca2a13ddd4ec1a69d" }
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
            const string knownVersion = "110.0";
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
