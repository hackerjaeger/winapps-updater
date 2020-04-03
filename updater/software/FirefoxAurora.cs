﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020  Dirk Stolle

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
using System.Net;
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
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "75.0b11";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
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
            // https://ftp.mozilla.org/pub/devedition/releases/75.0b11/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "6c8b90abb0308f4c3d68feac00499311cbea794ae9c7fae6ece22ef0f5257466d614311d8f1bbe1ad86615f19dd93b23b430c607c73e17a8f343a469efac1e28");
            result.Add("af", "88308b334d1816132fa917c2cd347a1e0ac69150ae759c075379cac3a267976859bd2897cabaa70d3dd311e644f167e2ea6fe5e4c9b3c30be0e70a8813f84aff");
            result.Add("an", "5e2c5611909fcdb8cb992713a86649f157947d1c3b1076d695fa1063f4762b4ed7bc30fc886792e93fc4266178505a07c56f37f5946394c134ebb03b49304bef");
            result.Add("ar", "aaf0fed017b4fcbee02401712a9404daaf59afbfb5367a8d6b41ef87e1e99d3ffd6887012b9c46c7917b201efac4fadcafb11eb01fd75d576a9a3dd0b4868215");
            result.Add("ast", "9c54353f4cff47d460bdf32ecff3f9765eade5c1434fa7a78a31b5775f5354eaa9a2dc78d580b9ea67802ee913acbd1ac5d1debbe11d61c147fd46e416f003f2");
            result.Add("az", "0b830852e8359b63e612d6ba69e21eb347ee8918da2789e68d203912e83420e85d189877063d7cee543b5431d54c1479af0e91931fc9bfaf46a075d67196d306");
            result.Add("be", "c9a9e88a275cd897900254ed0188160c47c72ffe81bc5daffcff258e1b3252193d0798b0255aa960b11defcf55b419fe6b8bbd15c70a78fa02f15a0c5b5cb098");
            result.Add("bg", "ede04d2d634ab85cf43395378efce2fadd97f06e693fe2f3e31e137492f2e83c53daf928ce80d7b9f0971ed62bf55fe7a72995553c0274dbbc9de5f9133bfd6c");
            result.Add("bn", "962582add1bc5703d56302ea038b143af028249fb1b065b310dd75bb2081071de374befe114078daba96f010094cd1dd0ab5b5f9a701922242780cefd06af34d");
            result.Add("br", "849946e8581ef41b23e375a5cd1523201fa399c6d911f3193d3859c66156d8c3b0d14fd614b613b5277487935bc4f1bc1c1b7e6bd7f90b193850648556f5161d");
            result.Add("bs", "e0cb446f8b63ea7eb2c3ae70700c2e7b79cd5a0d8f5e01ef98b4414c862b4a1b1246d70d090df457a97f4ffce428fdc53acb86a1d3702d888cb88bbd6b906771");
            result.Add("ca", "9ef06be738d7a1eaf3732700e781b7f8da00ec18c7880d04dc1b3f8349f699785d5900d0a3ff194b0ee8d0ac40778ab13a438c187e405affbc2f36720aa2edbe");
            result.Add("cak", "621f539f0075990c42d1e38e3a3cdb1b1b5954bd9e6c9bcda06622ce01b7bb119617b0edc5be8e8d0cc644070b5bf4f018892fe740d47b256328d68c47ba8d00");
            result.Add("cs", "dfd623935be120179dd9251810f1e77182665e6c7744b9e3e0b7945209530e46aecc9c55fd0464e5663596245a741281a2ed6324fde000a64098d9be4b4a0f4e");
            result.Add("cy", "eee5d1b5018c285da8b7e2ea646ab54f337f2aba91fa89d487d2664c14a65af47507d1b22126ec27ea6e72d96ee979f7268be1d348f7ddb9e44b5cd2920d6123");
            result.Add("da", "43edf1fe444c9693394b082534865f53c26bde66fbc45c8d7f5fe00be2bad4b8c4f425abe8440c8000aa9aff4a84c9f97ccfb5a03468cb917b449c7071937d90");
            result.Add("de", "943a1f025aaa74ba8538711755d25b35d6dff304538f2628e9ad4ab17118139776bcef8a9e19032b2ea2ec05119fd94c8eb7c4c0eef430f7efd45172aea87ecf");
            result.Add("dsb", "94bc543b83e3cf012431d3022912b5bc56dfce5ddffd4687d1456b7cef36c5a208cae7f67c2600007404ef708bb74059532e9dc7aebec99652bf05f76ee6708b");
            result.Add("el", "2cd212344074c8c54b44ac4e1a129f22fb0fa1a39e0e540d3e5be512892221c6e16e3a07c9f0e578dce978443bd2f06976a9e8c809168768b754e83cd448602b");
            result.Add("en-CA", "3047b11819492bcec0434d77e43d375aa178b810db59beaebdd388b0f544bc5f45f69513df6922b9fd4d028725401c60fb9fd90606f25be6f3e9d4cef37355de");
            result.Add("en-GB", "83b61ee44e806cd1171a9fb0041ac9ad0bfc50ee2a6512d7a93dc81a4d8d5eb7bb7caff64a000a36449fabc8f43cfa267dedf6046a3306a1027c9e2a05b064e7");
            result.Add("en-US", "bae99e386675b3eee5f667b18fe9aa08871548e8f3872c758c5e19fa2c6c20913c5e01b7d494b1fc81ecc4b4ca7ae0f9c2fbe5698bccca8837d26b756974ffe4");
            result.Add("eo", "8f1ad68995350e570348c4dad1c8f95b9eabf0dbc5d9e25b558f4167057b6648b1f12724bd0573d8d72677f3e0dba4e85a1c4eeb74074b7b215a5cff8323a529");
            result.Add("es-AR", "6eecbdd1cb0130a85fcc16350d328cc6c2d96f9860f0180cf873c7d750c0b604b8d70db37e76fd0ec7768639f145dd213bbe2bc919dfdc521d5319a450af4f67");
            result.Add("es-CL", "b73ec6759569458a1df9be3849761aa5e7f86b7665fb62329db983f45388b52d2252e7a4e700767eb0d824590c216d16180281915f88c9938dd3cb2beb5ebedd");
            result.Add("es-ES", "141f94f09eb85b4719be21cb3dc99c258eb6c55466c81f3ee2b2b3e9c2bc27e05f865a40c494b18ae6e200d88f38fcfc6405708466895fd33303033084fa5ca0");
            result.Add("es-MX", "4e191977e504f72a47e567fd2c712c20ff1c700162aeb928d1d0b1caa47785c19e842bbdf103bd0b61a697939c986804556bd0a28d0b55f0b7ec52122fd41ff6");
            result.Add("et", "cee030cb803b4d1bbbb2fff673b65f714a87eb0854b5889276cdbc5acfb29a605d525c99d800eab2d80a6cdbcdeddce2e35f390520dc64ff41d9a060f77cabf5");
            result.Add("eu", "3d13ceadcf9724e92200e95b2d8d4120b0651697af55ae62db3f50f5ce95d9224138e222b98a6a0707cba11358abcfaca132716f3f8885ad36446ec79d0cbaab");
            result.Add("fa", "670f1d46f32bca021f412f5703d00db3c90ab202d4cf95423b10e1ca1901ac75b1e2db690debb90b373db6183fc2af67926a04287160e3be5cde399117235cd4");
            result.Add("ff", "df641bb73f8f54320e199bb7f73cb07235d3a2fd5649c5ee497177790fab6e5f75e514d84751fa7915ee7846086be1bfabf678775a1dad9d560e9a723631ef1c");
            result.Add("fi", "3564867ef29ef4a8eda11e9ded0b2cfbd201db852ee6ff350c2dd6a90d69c6015b2856d302757c339cf1e8f968683b195d151d07206a08a9adafaac97c5d96f6");
            result.Add("fr", "27895360ceacf4ae28dd93f4ea8f6a0bff6ddcce7b26046071b95e8e19a3d2d20391d15b6744b0a137233f86e1bec1b05d9756828f6c6914551569bc70240394");
            result.Add("fy-NL", "3fcab565fe0e5da0100562d712e953801ad6a7075ca8c56ed557ce774d907363f220433aaabcb084518cc81dae0b8ec054625a908622012af4ef62f501932d3d");
            result.Add("ga-IE", "cd387f8732abb55fd43c29f603c6a858b7330c21eea91e28dde1d382671863e0afde8de657fa91e7a5bc167c19888a027022e077120871e3b568fe648361dbb3");
            result.Add("gd", "7ca2eeb3847edbf120e452874c5f37934efbb963fa7bf0b73f05139f93f23288d20a6b1e2817605e61fdaaed90dc89aba555de235495434a528fa678039c8103");
            result.Add("gl", "d7b95799686201912abc688f0799de404ceb4f4b9918ab5f8e06564ae232553790dafd86f702522738ef9a868dd60fb4659391d331b2393631cf58f6641e8fe2");
            result.Add("gn", "e005b5d0feb7fde745ab9160727b3fac21d5c1bd93949339f547e9efca47bc70ca732bb89439b22bc2306af939400166872759f33c6ed3660b87d71e07e8c78a");
            result.Add("gu-IN", "fd1d7fb8634b1250e87fef09f82734acf6742abacee38e8bbf0cd5f0e66ea53c86a78e2644cae88b6b417782102c095c756a2a3d7288ab3ba0fb33e25fde0dad");
            result.Add("he", "02e9eded039dc2c854776a53f2b0322a447e2b576cc055e273f5f73cef2f200edb318d4761f038a199c3706b6f914c257ec8f09b3ff19f6492e3f4e55089296b");
            result.Add("hi-IN", "b1870a9be97511fda44467910963a3ea68e79a8c15ef7d4d516f898e09d45b4d9a591d37a8011ee92e3afcbece811432658854c3909ed8901f0e46728ee2014b");
            result.Add("hr", "a435607e74d77c832512d5673c9995f90c0a32502c9188047597b322b7e24a277a4cac24abfa32fe3016f2af3c3644e457e0c2e84de15678b70d4ac35e374bd2");
            result.Add("hsb", "1c7bcde386c1fb15f0da483d0ac0c2346ae689b3fdc4beb0bc2ba1fc15579bd4127e6e12620f6264ce1750817419046b5b4320663f077c4431514abac3ed460a");
            result.Add("hu", "46e0802fd53af8437a5231d125b280cb9bb9cbbf244e52f74c971e9eff469966dd2cc64b627c2dac3d4b3affb768548773b0ba1244740763ff68ce04cd9b73ed");
            result.Add("hy-AM", "1a7c255fb59f6277b47db4c5752418187bcee11c63efe87af519c27cfe990c32b67e4a933c10604a4702db18cea1242a51922d46551d1d3610a2fecbb011a9c5");
            result.Add("ia", "f0bd43d85662c0756b06d19f547ccb4cd55338bace6f69905dcbf1a25e1e0c154824b6c4bb8e37e53ddda63196ebe5cb701992bd7295c26f1eb3fcf052dc62af");
            result.Add("id", "52385004a55f8a11d8cf250b2d430e49fd781dceeb8db907adad5eea31086dec5415d0402d5f96cb4188865444fbfdba0b8794e4084496f515741cdbbabcf6c3");
            result.Add("is", "6d38d75147651f5b5c26bcbf33fbdd74ca7c3068d2fed5a684c1a2726ee261ec2944921b54675bb4923bcd321f7a53c5816ca622ba5ee56a5f46cb4ff301a177");
            result.Add("it", "740dd85d4e7a9fc7f0f5ac12178fa287cfc3049727ebab0ae94e9b11730fc3cfaa8c87664df79bc948e8e5f46cc656de4d0196fc13f53b4b2ca114060b1dd2c7");
            result.Add("ja", "559b73530de3d109b9790438779e5bea657107fe8c16deadc35c6930984cb4b1b24328f8beadff52886514c76c31bf8d0bdad55557349f411248be2e2dab8473");
            result.Add("ka", "881e65b8b7005e306eb548b52fa0fb4790c2d42cbfcea71a220eb644ebb4dff08de7aae16802e4953577c9cca5f6317b80bdb6fd53f0bf39d96bcfa586d423ce");
            result.Add("kab", "ad63456fecba077867d6fd3b0bd1a3f60237b09e06eae72a4f1c2cdc0c5c89909ca0456a28bbe12ef2103ddc70909de330a7d1fe04e293fd538eab41a9ec4b15");
            result.Add("kk", "59a67d16cb9bb080c27a08a23b26017a50cade5ac361c672d951949685f52c3f3fa9c76dd0dbba17a80bfbb3e4e3c54ac159b4f82d281c4508c7136310048e6f");
            result.Add("km", "98c4f068e7482e21968db7a0a402ed3a9794708400e08caed247c8f99efce282aa3ef6d586464185b0548d496e9f90d82fc3d7b519f78f44710cd60cc9cb39f5");
            result.Add("kn", "a18ede6ae1fa5932d6e9c9c7093475ff81e5682176fc98d6fc016d0c88710499613a936b3c5abb6c6549f5cd920b6ab46fb60c32842e07accf106c7bac729565");
            result.Add("ko", "82a9188893a265ca99cada1d5701f532a734117eeab25426d211620e36ab7960844ab02beb8abfe2a3be0bb932d7d0eb310e02da813d41cab847a3af3e510b3c");
            result.Add("lij", "3978d87a9db3159b52263a6b838e976c39320cd0aed9616f80155f9c066b30cb58bff5f9b0b67c7a2c664c29eb6cbf9b596f9b6c1ef4ed44d4214dd2af890f86");
            result.Add("lt", "5fa12f5797fe708ac69b49e00b3a803db7d7557b908224ec54e3cc8364e171d66193fb8d99177593399a55fde735e370cb034c19a78f112f4f04ccd68d3c1137");
            result.Add("lv", "fcc7a4961611a3e20961d516613c1f27a43403d03d36e972206c9bd4ea126652822fedc89cb79ef09c1761a681d39c15bda12949eb5dd38e96f329fb41f5592d");
            result.Add("mk", "8c67c647bb266e07d2406e6b0aad364847451cd926763ab8f5d38b9b1916fbd08a2c52f0a1e1d3d5f51cdfaff4a6d22b942fb4c86b2b11556a427f32a6b2e035");
            result.Add("mr", "92ab2bcdada42455f45788dc5e62c4f8a5856c99ae90003e6b148e95b0724d7b693ac3ac3a42dd938c438e49015abdf1d9a4e884af418695f114a922fbe8d764");
            result.Add("ms", "c44174d1fea16f70967f3ff9a180d2786f72ba218157e49a88d4fe90e6050d327d93aca07c1b11d6148addbdb1584d802df1429407ada5309ae8803f8111772a");
            result.Add("my", "def96cdb932b092ac12730a3365e329bf6223c61a8f440a7ae01d33cb91766667c9880d65608939d10c270af0de0699244699e2e87d9cf3dc9abf389d15668ae");
            result.Add("nb-NO", "ecf9bfef9c098902a52ff062135d49d734e908bae31c437fb0018f41b88c13ec7245f041e9fd48afefd8f0e11781189d5c9e9e7b54ded16fae9db19a2bf5c1f9");
            result.Add("ne-NP", "138170201f557801537f203d1e4fc1ae7a9a79dd6a60f0d070931743409f900e9e08901f27522f851e9ad03435b4f2427d78bdd3fd2e06f9442fed9af0e9ac5a");
            result.Add("nl", "c270ab550ae9a7bbc55a5a55d8b9b9520bec82dac6d567dc2f1a8918ba39747a845e796ea81eac1b3c77d277d228b6fa00de689f6d21e3264b84f58807e5bace");
            result.Add("nn-NO", "8bae62296170898f71740fc28e273a022fc3ce91dadfcf21175102597eca58bbdd2ba445043298b5d79c2298b7ea1283ea719f19000721a27b81e5e0dc38ad21");
            result.Add("oc", "dd80eaabe3f652c2197c6922981eb796f411e644ed9ee6ccaa3f5fcccbc48a30fcefa058d2a0663d7da557cc2e85578308cbce314a66fca66c6af9bfb7fbd4a8");
            result.Add("pa-IN", "2ccba07bd85c8bdfcaeb721b4170a16ff70eb7e7a68fb0ccf5b3ec72299aa015f1d4473c13a89818f8a69d1ac641e6dd2bac28690626400f6a1e9087556ab7fe");
            result.Add("pl", "f72d7c9a9f74e2451e29ea2d924aeaa82648fcfa92e2281f96ec826f4fc826ea8875af2ac08526a721da761b4e376be3df106dbc732fcd25f66edc6e8acd762d");
            result.Add("pt-BR", "7b7e82d91084dd86f99b20e11ee4aff9ecff7402a56c9e25b96dd590b444a91b0cd257f3e02edb12868c18403ed18dd334a0af739e8376819b4f8e7a77c67ca7");
            result.Add("pt-PT", "c1a41ae429af0045caa98f86c135a5e06088e874003ba53c711feaa34c5e03465cb2d20fe1bad1becc47f4cd9dbe27701cb65c8505767f87873132068b131c0a");
            result.Add("rm", "6f47215fe7b25d6eb8816248f3f7ade282d3f8ad8bb4e1a7052918da6b8cdd3bcd08bd5d3518b56cd796af06c8d5fb568559de4cdca01890b4404e85c38bcd0b");
            result.Add("ro", "22f068ad49b8fdf697d779e98e7d25f9dcebb38ad2923034a48aee3cc32dad64687be290dd1d092b09b7d869dcebeead03ae813d3a9dd955c6a3fb96c894e4f4");
            result.Add("ru", "a476a21c097d5bb0afeb01bdb205a2d10f7a23411ee03c99803ef3d55bc21faf17a3099568252b9ce5f38a99ff63cd2b4066fa3c1d4ef5bd62c098016c82736b");
            result.Add("si", "41f2d30487b51763b9b39801b6b41087fa9f8fa9fd980a02a3ce9546f3b0dceedcb399ecf447891afa560c2e2b7bccd129ca2edb5e6d19034c6f7f16c390c387");
            result.Add("sk", "6ceaf930e907c607a7b59d5e07d7eb7969785b44f5f6e7fd5fddb8c48687f5395fabd0b4bcf610ea36991e6ddffb9c4bf4b857a4a79bc10915dbd3c4877c3bd2");
            result.Add("sl", "002ff1aa4850f315b57dd4c3caa70a342344e4371db0dd6d4ae166269a3680ab900f17ef62d28e8c668ec842d30b710bdabc1dbb12695dd7b55c4f8c5b01b564");
            result.Add("son", "68a386aeaa987b988036af1010aa33c0747edcc33d4061e9c987ff757a8b9892682965b84fb2f6069a9ec2b214b64b692232734568325b8bc6c3d7f3279c8cf2");
            result.Add("sq", "70e2eaac671c02785432f33cfe17b1dac3ffdcd01478a0348a20859d009d2701548875bf28deecb69069eec6af5cd99fc19a2916ccd6fea171edd564c24e76d8");
            result.Add("sr", "38d3342ef43b8e4d832240f96865a4d60e1fb090b289b9a073f6fe45e8c4a4e691ef45d2180e8d26a2bcd4801063ba22456c1ac299606f0de4e2a85b99a632c7");
            result.Add("sv-SE", "4e4be7e9f08056e6e776c7df0180c4db86dac848d275baae4d9322714b290d8b056eeba63bc594679c0724209a0950d7d8827a72dccae5a43d83d2fecbfbd36f");
            result.Add("ta", "e3a9d7d8588eccae95f7c1e202a63ae9cb15552f2d7226537feebb82005052397279e33c20579facce8e0a821cc3c783013b3160d6a95336cb2f467056fa8f86");
            result.Add("te", "7c672bb6cbfd73d49c2cc81230d0cefa17a7ae7dc8ab2c9dcb44dfc8293a3619d659b7cc8b6dfc02329e9a80235ec295ed3581bee18725718fb63bbd9559bbcf");
            result.Add("th", "7c0592e9af47cd181a658f764ae4f7933fefd3af33dea18db11e19a19869f3e2cdbffb4f1ca5d078df968a3cd30d22414628453e42f8f62f389e5c6bdfe3cde7");
            result.Add("tl", "c82e7f802b7b6f2e3a50d8875e81aeb332d7506ba36fb20162d56e58821dabfd7fd75c155735ee80a55f7154a9bc54dc2f16d3a9a283307e8e7221437c3da7e2");
            result.Add("tr", "1a8064a02ff378d72540c3788df37ef1162ac77f0c8407b3579aad748c0b09f2872c5b0212e796e8124b06cef4ad0d192c134a8419033dc4b1f54a262f897e59");
            result.Add("trs", "eec6157dc89ed3202f8ccbc001ffa1857585a8e1f5761f97a4f0555a715e0664b35419c8a2587b67a67c91dd6eb2d19bd93e391343ad3f44919b2593692e8f96");
            result.Add("uk", "5dc70077a017e4a657c7c5af725a5af7254e2dc6a92c15855aad41803ed9c8d2b884ddfb3df4ad5a33b6654f1a60498cbed67b3b49a842b38a076467bc6a9d30");
            result.Add("ur", "89706e7670d728a0d9181fd985eee7bf718c5a6293f6230d149885e8b75a52020233b9c7372d296b19eb50f1be501e8f5f8402a7db57adc3510aa8633045ce46");
            result.Add("uz", "0a17db6f8ba772a18e1de38812c38471f47b7b1582ad903cfbc27c851f633ea2d7a20f968d48cceb4c34d20889a46523b7d310670011ffb18410038303313cb6");
            result.Add("vi", "67f0b039c627683ab9f72f0f1e77830b56a2df394b72ebfbae965fef7d68d07f022437bd1945877aa5f5b0aef68205ffa1b158bdaff3cf319ab04245b921f0a2");
            result.Add("xh", "fb7f6b306cc911ffa56d2530af4c9fdffacce81a1314b077e629e2e31189503d958ff77946adc5e53d17b582f220fba2cf44551e0d49dfd9010ad9a4c3139ebd");
            result.Add("zh-CN", "8962a599158a8df32949684c5a1b96232666398c506e409307381aa55ff5f0a725d0887fac849240741d485041f1b80cdf979c856749eb430af0f9f14966a7d2");
            result.Add("zh-TW", "6d689e6685b4f8694699a88c5817da95a92ddd3e1097005b375122cbd7e1cb288916db6509809a5f7ea0dcb164c4d8fbb9d9325e90f04a76ee3df897f115e2dc");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/75.0b11/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "1b2a9ab460b546f7815b6fe0f908f2c7d05c6e95428e7aa20ca8bd9a02f6d7903b953217ba81e14416ff3d1f3eb2fde7d3c29d0235a7fdb15df49741ae1598c2");
            result.Add("af", "e189141136d9b774e5f06919e54c6740f74f13a5d6d718618ae9c98a714939af72d17061ec181b255342a79ac70bc467324a87f623abebbb7068b90a61ba7219");
            result.Add("an", "0c579c781689fcb1874248c486b3cf6cda3d07d2779dfdc255cb2eac6f56d8c6f31a5f9b814bc4dea1ab5553b6ac742be462dff6f0b9ccba0251c7a2ec465497");
            result.Add("ar", "c4581a98b879cb6800548d64f69c67bfc1764d9eb2a7d71609e184bd7fb23b3c5c008d4db6f465200b0341ef5bd39394c1c75e5d4cbb4f3db8cd54df7fc49e97");
            result.Add("ast", "bf75ecba48841b4a52342741887dd8f9a6f8992460d73d340c2ccbadc7b21469e1ce341722d7b9ffa99d39c89125552e458d824d69fee478a7235bdfe05c916f");
            result.Add("az", "72cc40457dc87eadaf8db663e08dcaae80ee9590e57e18905f26244fe1fc1e57bd7f13ad5218d9b35af80d4cf0dd58301c0f29cd910292ca11ea0956624e1751");
            result.Add("be", "cd5bb6499e482f8472bfb84374e28445fe03fad82a9c5ca795e2d06b9d847692f07bf7a46acd1060745c90cd5092236ca896ded64fbec763e6966a31f4156225");
            result.Add("bg", "6a46732f0dacfe703c2defc41f4dc8a3f3d01bfdff95b25154b8f35895a19a2e1c5fe361dbc3e33b846b7a9fce137f93155db17a0088b8026af0eed367fcf058");
            result.Add("bn", "988d9f15920556975180ab294210c6f6452b1d800c6746f46ee6a21a0a46c09f337adf1e28ac32aaed54ae838bd3a59b80c9e4f6928afe599ce2cb14f6a6b49d");
            result.Add("br", "1ee2ef11a6fb5d932d42cdda217574e5c7374aa85d9bd5409be4e796e85e6e506995e5ab2b1a3568f9882dbe0e0c001e2cb6507cf5639de5c5454ff6b9848141");
            result.Add("bs", "63ccf67fe009a0404a2c9b44400b17ab09943e7bc7e72ea3e8691d7e4519066e48dbeee8cab9ed452d6d5ab1391458e8ca43b4d1cffdd95dc978186370b55e05");
            result.Add("ca", "481cce9598ffba3041ee55063a7195d45b22ebab9ade423d67c25112079f047e4147c4504d4858ac422ddb852031b0051aac7ff8510ec6df17077438e96df8c7");
            result.Add("cak", "b63de7929578b93cbc619c041410cfa8db6eb194c2e26894576e717a4e25284a7fb06f08b1ed4ec1ab7eae783ec0c28caba6ea39b19d798c3cbe901524dd1d2e");
            result.Add("cs", "69c576e887802df52facf2a2069c4d903a5ef58e0344214f74b5cf294046281ff284b842b6e88ad59ff21f4cf558668d8a4c6da14d4363900624ad780bda4502");
            result.Add("cy", "49f0b8d4ea7a5f2e9371eee26f38a2bfadf53cf038ded539cbdcaf94a1cec60868db030d057969338c56934396dc69a716b4fdc64f90ac14875ed995e7336804");
            result.Add("da", "ceb73bd153fe617aeb7d1ff6c18bc0102d477acb8b04428dfd114adf613f776f878dacfa391943c52932a9b01572af5f8cf0bf5f79e3c3776d6faedaecff63b3");
            result.Add("de", "31587875cf34763ec41874c0d22387e8dcfe572b71ee82bdf5b5d1511085ce917202cd0d5c171079b73c6d21255438c6ef634a8b9dbf4f20a83930aed68b1993");
            result.Add("dsb", "47aa22176a55c91ee4e3446c7b881286f936c7529fa3be9084fe7ef2ba2ea9ad7f9b26db383ae72af851b49cc14e2785b62fc3ab683e32353629b398592b2ef1");
            result.Add("el", "bde033611197c150bd0147b6d6f4d29d3fe8fbb2f364c6b299a9719d937796c6bf7811f40cc274db35757e9bd0c542cc514e30cb93851a16bf679cb7f95c2cb2");
            result.Add("en-CA", "775347851345c86557aaa2028df56676b4d929fd81665a69626638536442e76a37f67a68c1794c683764ff8bbce7cc63590ff392f2965e73b698c40ebf7f1786");
            result.Add("en-GB", "810dee882ae07eef3c1b6d2d53637bf23bfc00908272bf1ac0ea812421859dd893c54569994f2b4a94014e6d494a785d734ad243f4e14d6caeb7f4831ade8b63");
            result.Add("en-US", "b06a9e383cfd9fc1b1e39b15c780dcc2ccee19142106d56bb3354fa5168dc41102a5d73f90f02d6471df58e4293f87253ced022a97af117c34b68e422c01f188");
            result.Add("eo", "aa4603eeb2bd705c2480e07500fdb326521c09b8604e1462bb63b056c020cb3de18acebece73915c51a851415b32baa711336ecda49c03077e464aa3e294adde");
            result.Add("es-AR", "e6b40777059416c5e0fa5e26dcc91d7fcd4c82ff2e09f2c2bb4ec87b8cf9eef2636dc9405a3a1f4120054d220f6d319fea79a0254150c5efa22af15a6f08c3e5");
            result.Add("es-CL", "6a6eac527980e2b007e56d8e01ef1de62ec695d509ffc06c1c67b9c7f709e48ab2c059af2618771c4e8bb69705b64724a2a5d514048f54e3fab45a41daa33b24");
            result.Add("es-ES", "f28d518b1f42166096801b4ba52837b7a347fa379bc7f6883cec9d7b9de8103f27160d462b5a5e53ad50945fc79d38cb75b8c66e155a8a36359cedb4ae071503");
            result.Add("es-MX", "e9f429aff00efbe2cce0f305652f08066a5e35e2b3e4373df65091f6af0800d156028debd89a6267cc8214104139e95a37d2c03543b00bf692baa8e59f3a4214");
            result.Add("et", "5dce051897050bdb0907f27fffc6020512c113876716d849bab4e36077e421ed8d82ffe8f2c907e0d96c052fc1e389afeb1f358e81cfbdade5c107f0fd8cef97");
            result.Add("eu", "96d469bb0beafbe0bc7c58ed3b519e30b8dcf59fb643dc00c62c2ebe915d15d1339deb82c50e3818c774e3be7e4565b7d4e55fd78d3d87a01d531eecc237681a");
            result.Add("fa", "cca0ea968bce47b9455575a3f84e4f3739bcf70c4218a57ff03c63c0f81688dc0f2b27bee3af9a85a57058853adb5d5b6dc17525d7fcca5c5b045e88b1077047");
            result.Add("ff", "a19be366e3a24a41337bc1cef1df963285cb19fc9a5c8ddb257a2654bd6ad0f874fc2be2f217b85bda57e221de3b360d3e5d91bd8917ab325f63524e9089dc1c");
            result.Add("fi", "e51a9549a175ea459504498bde1198e75acea699e1fa01fb36e387075e2984ae0e6852841584a9fc7aad9a5ee7afd98ee5ab111adab3b82d8990d3967c94819c");
            result.Add("fr", "a1c9631f5281c4bc356430c199d6f07b1ed33136943e6fa94502b440eb9f7349e210ecdf6e244c50f0a3cbd9c442a01dccac2e779d92d512c4b0666f3f837137");
            result.Add("fy-NL", "7dfd871d56bb55b93bc09dc8f034ebd232fee52f13d699f293b742d6678bedfb1c49d441c09b428d733e9503c5973eddc724d5f9a646ca4486691c686aadb4d9");
            result.Add("ga-IE", "47d0d054c75c2aa45f1d9b3185c88ca7b1116e0e7503457ad9608b7e7b8c04ddd1d7a7293805f9878061f51f9b8c2ac73925f58be794efe474f6a942e4ca5ec6");
            result.Add("gd", "2139012099f4633f369ce7c998ce88e13ecfc9ab279e2100cc4ceab0350067dc7b1ee05a14ef16d63601bca077ed10cee2867e8be57adebc226f70cc0539c0cb");
            result.Add("gl", "3fb443bafbbecaaf51ed614bd9cbd3177c30df74f5616e55810f837c8fb67b3a55be365ed313fa44bce0a29891571935f2effa26925068de71b5ac2201ae8f54");
            result.Add("gn", "5a09636d3f64eb9b1674eeb636db1369b1ac2816e621afe0c41756ee18de46189a7bb9759607173861d0d408e7da664dc933affcab869163b05faa86049b996c");
            result.Add("gu-IN", "b264adb0f06b293463555e00484a931db6ed93e554531e79ca894a2d7366fb70da675b2d1b2cc504a3064e77cf17f5f8153c32d93306b4c8023be5f19bed0b09");
            result.Add("he", "ef9419b9da3c88de1dadb210072211c576370763f92eac5a5afb742d1d1b52e422f2dd3c082d0a7f06b52c5e1a5cead62e5dc4e8fdd121c978e1f47f89243e7e");
            result.Add("hi-IN", "faa4898b2eeec585ca9070e6ef0f17a0cae8a8101c0eb175652c05d4a29708ce925013ed3b1ce3a84380d2c1b36ca1f1e6e845dcd0963f8a30f449e39321766b");
            result.Add("hr", "b8a44a0d40b9ded24525b6a58c770d53dd9e5cfef99464ae634669c21d2f99ce771551938a4cbc41feb5029809fe18572e71b2f9e00808cc5c4bd5198d483422");
            result.Add("hsb", "a25d47b3bf7d59f6c7ca40917073766e1dc40f28ce363be65c3187f661b2c6d4f2b1aa4011c1ce5603d9e5ad310273eda1b8bbcf99c867215483613718b57565");
            result.Add("hu", "551e9a241d4460470d00b3c5f519650477a3504382351aed899745f70b133f871a746e71690f83dfab99df6997c44c8528477e47d40ddac90a11e17a7466a35b");
            result.Add("hy-AM", "753c09edfd17ba529e1818d03cc7d3d0a82a928dc434ab016cc48de4ecb8a37174a5e31b9a027637008271e7c9f1a328df3cb79b3c5866d23418e1850731dd16");
            result.Add("ia", "70fddeeecc87915e391695ccdb12330c9e1baf396cf5b11dc7ff1e2db9ced9d4f07da78aec1d05effa19c7f9debec6d104df901aa70b9e9d0f103c629e7e11ad");
            result.Add("id", "313aefb8db670b3e15c97d930118fa742342ac56243f742758f3d68b9122f9e8967d4a4023b17c3403f20316cc443f43b477d37450769ef08d40d8ca688a6461");
            result.Add("is", "4508c4ec94b458cb022ebfc26740da071c88430e51da44b7a01361b3ae18d3bde429968e265e4e00d153b7d981d4f343de3b9cc0e23415890f9ddc6ab222b165");
            result.Add("it", "1b62cbfc83ba0f37ba946c943f77612c79401b99d4d5b48d3ed2d87beeb064b0e37815a4d204b0793f09ae28659228913170cdee163f483b70afe602c1fd975e");
            result.Add("ja", "d9be9ed9a2ccb3add3c39e1f7ae82bf847e374d99b3438b26e08f3a6f38b1040d41fd7bff499d157e150d9db8f71677260d41eae1553c9f3897f3f5dcb51473f");
            result.Add("ka", "59edbac3e30b00179e5943d74c79da63a5e073d1cec578bffe1c0fec8df1c8eabe54491f2f3c0f2784976ee4cb48790483a0f28789adb98b9f65be7a075dd1a5");
            result.Add("kab", "29abcc6b713e156b07da00ae6017eee6e4a522338906fbcd1c8793092c58b2f52af0b3135e152d62095491c42c4522257da45e55c143fb8bdb32f8ee1fcf39bf");
            result.Add("kk", "e55420b71a00aacca29852adc13fec521572858d1efd0bb0a5c23fbba4488355e1679f31886250ba0813f222bacc75c7a1aeff582a01d3dbdf8367bdf7fa3733");
            result.Add("km", "fe3bf25a0f33c454c9d5fd40aadefb70e06b7863b52a7ebdf2790abf570f4172c8b60f9d9862eddaed3e7a65adc27430f80738466308ef39f3d6626dc9048994");
            result.Add("kn", "eda0b4b59e6a13e69504d0be7f24c7e730eb27edaf7af87c522fafd95c01a693dbd437074af5eae2002c82d2a04097d1cba61f07e89901a787b8e659894d2f82");
            result.Add("ko", "c76331f40d8c1a000331f73e2b19c6a5c7f8210de2d8946dc57d7c900efb8207a7725cc75f9c9a21c0c84a91bde55feacde3978abacc0be09bbabd1d5f80c729");
            result.Add("lij", "44f93d88e197e6ac450fe2b7b5fe2ba5ddbec393713a5e60aa48603508d6953a855061f0badb798e33831e0b7ca1b21407cf244c965f6c8669fb2442ca7d120f");
            result.Add("lt", "d1e47449bf7af7567ed197532f668f7d6e1836a693a302a6571450b36d3ac0d1bf4f1f5e6d290674f5b9ec14fee3cbd659f5c715e6f7c4e59ab459f843277bd2");
            result.Add("lv", "73611e8d279a5b9d5e6341e4d7bf23001cf136babd911abdde0b45716817dfc6776b497059e037a60f986cc580d070c66c03bcd3c0b27b2b54757bb6d535ba07");
            result.Add("mk", "f4083b6fffa2b28bb36556cd425bdb1e253ec12da6961d4b9e8132c62f3cd2a8bee95a8ddda60a302e291c78981f6edc9e6c624078521f246500782712d98edf");
            result.Add("mr", "48600579897e67fb4462c70abf887f41f148a0854f186444c0af5f166d7ac8ce632491861c660ab96894a35bfe3394988bf7c2eb7a25089544f730b93f0148c0");
            result.Add("ms", "c49c83cb3a841c33ff35dc13bd761f0f661661bd603119f0c45df1521fadce7f76cdfbf163d5d64bdc18522ea1d10b9d1719db614ce04f4ac8e70bc9441f0d3e");
            result.Add("my", "111ad9bfa6d7575d00321d3be7fa23079529a0ddc2aba22858c3fda36990832a2bc3f1e16bb5c186fe60be6b21650a2d3f675dc3e464e9c1703642817bf2b989");
            result.Add("nb-NO", "52f1aac400b65004bbeb5dc9d4acd371b51eb81a226e24cb072612a7f254ba1ec281c6a244dd04f30dab5c3a0b87277948c05279db7466eb03b39f086eb6fee2");
            result.Add("ne-NP", "d6a7029400401c6d8034825b6878e4c5b8ced6c5ac7450f8d114114f5f0db59d31949f1b1aa4bf1133f670e9a1ede05155ab886c439ee054b4512b872555df69");
            result.Add("nl", "5dc7e6c58466a7ed76831dbf1203b1cae2dc6d0d66ffa39e11bc77ac35f3128a5753971aadc340f25eccf3df9d2550fd20dc25b42913a50facb027452587d760");
            result.Add("nn-NO", "bebb0014c10a2028bc03583585ec9d15a6257669c793389ddea3fc4dffdca7390a98fcb474230e06c77eac1dadd20d787f68ca573e78c0594723b2dc0dcb2a30");
            result.Add("oc", "8cad9afe38c1bb8575b6d1a0b1f0298e22a219ad526e3eb6e1e095b4f7c559ecf098d004e517e78d43a6f569ed2a1e68096cfe252ab3d81a43c1bbbf860e2e7f");
            result.Add("pa-IN", "6c64d245da4707089eb59c38b5d79f0a564f42d6692a12393a194ae98baf7cd655c2de87c0175d28e7f8b4724d83567b0bdeb35549fbc03dbef20042a783da5d");
            result.Add("pl", "8f1754639249581fe7cffc0ca16c190e9b4746f6159281d39d16fb2e8a0c907cf36e7ebfe7c2456b26ed24dc9e8287e97cfaf9292a1ee529dab8827c166fd614");
            result.Add("pt-BR", "ad2d7afb9317aa167a24a07334674aabf208f5cd8be7bc0f17b5fd59f77d6d8470837db00849c86906780910d2f546f9d15a2c77a70a5424ff671a57963b1acb");
            result.Add("pt-PT", "372a624944b9effaf3179dd632c37496a8db31a82f4bdf58e17d5db8e3b43433c092781e3df62e9a205b10c332166d5ed8474ab2306c13fce2c1a204a027e182");
            result.Add("rm", "465cddfcb59e282c7c17fb3b8ebdad9265d8875c0b126555fcc9f166637431333be60494cfb3b413e4cd0d2a4ae3cb47fb8cf2c31b7d2f7966f0bfd0a482b6b5");
            result.Add("ro", "a4a587b666b4620894935d2964f539d103920d02e032144d8d87001696b6a2393caa38521b030d20605c8d5feace0665ae24bfa70eb209322c2547e7e74eff6b");
            result.Add("ru", "80eb853e15acd7a4d7f8081d20694abed0fe823ba5cf93704caa8c015cc5120939dda85dbabbb4f1bc83790af9bb9bccdb0f2e7619caf9b19b04315cce35b942");
            result.Add("si", "dd3c0e9a45dc51201ced77b8b590370767a0e979f69ef4050eda7843d64ae99071043fd754b7ccca22f8c97ac34e1eb73987cebafcf88568bf0e87ce2c34ed60");
            result.Add("sk", "ffbbe6008778b95d21f5aa704625a0f2ae7513f9303ca89f386d4117630c8b371570fdd4775fd51dd1c6308c853d1e052a402f75e972691bc3f8ad58588b7b88");
            result.Add("sl", "78df843f28aa37f0cb91c7fe579d83d45cfe940232e443861901a783784f511180dd80ea93b6a32b74ae62933e866d1f26d2780050e96e725a13222b2249545a");
            result.Add("son", "2b048213776db50389c3cf2ca2bba6bd0267bc692cb38a9fd804a8319eba68ae8d4dbbd78cc9093b280993e00c8368781432436f42e2008d21e53b24b12968d7");
            result.Add("sq", "34de964bbe0a5ba8c0155d6d7ab8354572b81c298155ac40f148c6fa6ba48219122316ad5d50ae49df089d76183dfe0e5760a9f9d42772958ecbf8a9b1b16782");
            result.Add("sr", "ee993ea936863f08a5512193515e0bf3f3431a3d470171b262c0be8c95f04186b69dd3d3d9f6046e789cea328739cf8bd6451d7f34054f1cbfb7dd22f6ed01c8");
            result.Add("sv-SE", "7dfb61267d7aeb6a0b3041b482036577460bef6f5b87fcc04a6b9cd931ae3f22b4f1d502d7ae54bd12e61f3480aab110e165a59a4215cf892b878db5e78f53c6");
            result.Add("ta", "1cdfdd805d7c7fdc980283c2822a37ffe74c33419969702fda7f8cbe3b494eb5f152b3031c80065e863b8b3bda081ca1e0c43852f3f6c2c9bba9d3d0c2e7a9a1");
            result.Add("te", "874bbe28b5e4316493d34c66082d6aa7da36ecfa6d4ab3d272a1b29bf6ecba5d4b07ba7d4cc7aeb6f5401878231881d29986f2b761b0cce856712dec3a5ce372");
            result.Add("th", "8c7a10a8be8b5168651d7f53fa9ae97ba69ef866796ccd7cdd898b9e7d137667d46884278c3ec16353cbcf28fa55fe8cdf2cdd0313c2d52f211e2b57b37a1fc7");
            result.Add("tl", "e1bde11584fae2f8adca66aafece3ccf761f04a55559e77a7558531e0ddbb20f1c4c2b5b8a88e0035c4fe117654248d52c2dac78e1d1ea72a313b01fb3e1248b");
            result.Add("tr", "f6ab2972107fd1fb98008eea295c677f2b10993982a757239a3b99e2c73b49acd19662b42a39211d7a6254026c253782d7c3351c839db0f37b7ecbbf943719e5");
            result.Add("trs", "3df61c5ab2f90d506c2dcaf77352031b927114310147cc713fd63450c5155a98c10b15cc2f488d9716d158a617cd6fd65005fb2f4b71b89e53dc7e38750b8922");
            result.Add("uk", "6cdc90491c9ae428464a36d925831a02efca494b3465f9d32500c6d27b693946853bed4212e845bb8734302b77c2a5dbe825ec37126ecc4f874f0dae2838ce03");
            result.Add("ur", "b5a8c5c1726ca9635d04221f2095ae19034eb617737f9159ec9e5136192c47f6831bd4800b93a167661327e5892400a3f7da9b4c18c240ad7332e8c953d104c5");
            result.Add("uz", "442fb79e6f3f0733b44b2c0cc5e70ec50349f727e5c958b34cdd71f874cd95e5e1e76b1654c5f3acb1ab33529d38d154e3fea642dbab4c208c3facd79f01ff6f");
            result.Add("vi", "33628b0a336fcdfaba5018eac7ed19e9ef2a91e7a02d4722703e7f68d1a1a50cae62efd7b0d681031b692cfd7f4cede72a936cdfb105fe3ca130f34ac31ee8b1");
            result.Add("xh", "493553ac4b01d91253593faa679dd46ba1e113a2d148a472f50925f3db648e81032187b185d4b85467e9dcfbe2532d2152a256834d662187a2214e60d9b55aa9");
            result.Add("zh-CN", "ebabd289b325923dd87632173209bf2f0e27700e011578dfe844d18529ae14a30db25656951dfe26f5ee90eef90548452b8e0cee618883b11601c1cbc514f546");
            result.Add("zh-TW", "f77393e3fbd7a64c07fab8cfabc17d4839e02a29aeb375aaa14e388be679d979a2dc92901924b2f375d1258348b0fe95b2abd2d20117e65a82f68939c8a34de6");

            return result;
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
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

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be update while it is running.
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
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;


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
