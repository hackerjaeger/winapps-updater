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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "126.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "164bf4403077ef8a6feed0f04b1a6681a859b8b98eb43871e096aa4f5e60893abf4924f7320885481c6f8c906a9d12cd950aab1d8005c123f5cda415b5111711" },
                { "af", "6b99354d2c72a272c2ce7eb33ffadd269f6532ae9ddbfa9d8ad7db8927fcce53a91c5675c70c4aad8bb050053c1e4a839a6e7617e73f5c7600d0369e97bdcb07" },
                { "an", "a839cd00ceb9303be0bce5ee4e36709f048ac3828410915573430670b0648f5108c3be0231006fb8bd0d738ff191c0d48b224c4a3af90ccdba679b6efe70aded" },
                { "ar", "6c200913f841d154f2ad664711802a3a2233cfa55e2bc7c552f1ea0d54aa2428157cec0f9670657180dceddb73cb8e054c798a7a82ae46f3ab308bfd383f4cae" },
                { "ast", "c852525e68c805e04f6c31d1bf787123354e0e82232df722592e7a41287059344b9ee2f3a3a5983d7be1141ff7ec1ff73ca70014ecb960e1df4d99dcb7ae6be7" },
                { "az", "c9d8f56b83a6b5a482ab08f4715238917b5653877b07573a3118bef6c9761b532e0e0f1ceab209712e843b7dd76259120224e3bf2062dc77d05b743294565a72" },
                { "be", "dbf87a01032fa6002f99597ef1ff73674e663412df64c9d2249eb91f8c3dd3927c711a35672251febc4e0e561cb900cbefdc6c000e85430113fadde0a720e6df" },
                { "bg", "22c7aca35ad40d331c1a73ce0a31ee8a49efe02539f83b4b2c579639baef29784720e03b50ef9a15a5a01e5e273791b739bfc32160a72de0de52f87b63133bf3" },
                { "bn", "8cd4168cc5f304a405aa8a323c39a91622c2207a2cc06ff03a25ad470b1411c9698fcff610993b3086abac6c3508a569333b6f469079ce11d7be0e44c7d8b36b" },
                { "br", "b504ea3178e00c6c9d8fca1742468db8716c27e43edd9935f7eee8e258d9dd54373628e8d9c4303dffa5f3423a3e203cf7e56ef767b20fad644955c8f813aa05" },
                { "bs", "b078f9a79457ea7d922224de68dfe1dce0a36520940f5e77d41baabc2acea9bc30002afd1cf37099fd087f61d0a6aa5eef8578b504af491b8b7ddf98f53772ff" },
                { "ca", "05cea867de5c763bad15eb6d9d85f8492164865502e997df20a685b7b284996f3b0a9acf0b1d46e3f97f2a4892188f0ed6384a0a2ef7054b4918949467110ce1" },
                { "cak", "d4cd15a7c3ecda2666d1fbc26b1fb0d5ab715bca9a5ca3f0ae25c9347aaa0d1e31eaff19a03ab2e48e242b4b8c5a973fae1fbf86f3791aea9b45df75ca024ab9" },
                { "cs", "79bca2ce3303842f3112357104ef804ee02d6b08f036d43681bd17b0c1d4a88adeb8c697dffa6824bf81641812584662ef722d7c8c2fb6ea66b88463d2cc005b" },
                { "cy", "52f28f66d43f7536d179d62c677988dc15542f71df0501683be21935a6301f29dff533a20a48af15883897e87d886d80ba6ab42c4c6f776a0b9cb7945dc09b40" },
                { "da", "806e54bcbec103af7054e6dbeeae4384ac0f7a316d1221e774c743c2e4cc8197a6b01cc987ce46895acf30248b742b41b89e75b23f9710182dc17abcdc6a91ee" },
                { "de", "b03597971cd25869553d863449deef25f5ba0226d76904cad83818e703018cb5905d4555a96cabbc5957dd271e7ff432d9a302e8e83ff122a9d3a97d70f93f1f" },
                { "dsb", "75cd0bfa08aa3ce06d55e41b146338c1a9337d9e8ef61d78ab894e54726c9d866563bda998a500f4e434aee56bd77eaa392fe6fc8b6cab0283305708c31e4052" },
                { "el", "41a2d71dc3cdedf7f72d3a575086ac9bd42e01a3936b872c22283f77fcb205b304a134e0db749fde1ef854aa8508dcfa4a7e576b97ffca486c43f77e5cddebbe" },
                { "en-CA", "fa5d35df93c9906adf5878733dab5986cd83ac885994cac6a09f6dda5752ab3f517904b540f4ee78b2ef1e9f0f8df0b4379eef4680d40ac380ab6d2bc41a718e" },
                { "en-GB", "f4798e0671e48161d65b1411f613fbb307b42cd5db18aaed0aee46ae8fe1e344944d2db277352ed2e548ab8d4028d297170f8c2919f01f41e1a4a5abb779fda4" },
                { "en-US", "9ac44ddccab291abfb46684eff087e72e389fe9336244fce11280bc369869cb1ae5f56f9147a2f7c17cd4ab8e11a9b28ecaec6f280f754d88d00d27cdb3644c8" },
                { "eo", "ad66c0f65ed9f3ab1e088ce35ff7450848a4c70e02be4a1a7a08a754bd3f4c98f6716fdb64d4d5093d137981134747b323cb6aeb18ffa8e5d58b94ccb4bb63f2" },
                { "es-AR", "f0b3fd1550ce7248710bc359c9cf61f6a8e8232929796362526e9dce0184d4183b027f52b896164b7cc169cf88568aaf10f26fbdc81fe6b7aad6f0f677326399" },
                { "es-CL", "38ebb4979b200c99d667c78f0154d5e6f431faff338e3d480633762aff62481e925ddb9547ff60d8266c1519105edb19db98a5a68088b0c2e9256e9b2bfa85f8" },
                { "es-ES", "6810348a42e28c943848de17f10b858f0d9662bf30fd16d935f7374c7ee9b4b478d0f30cc9720d95ae40e31d0ae1c200b91a0ba93471c5ea8abdaca381706670" },
                { "es-MX", "bbe1206990c6262127869961c3a0fbfdcc6b38ab5bb05f56c9cde35b189830aa9dc098004e24ce80f09d32f76f48fc0c3fa83a8c58463fd4cc85fa894c864c56" },
                { "et", "89ab2392bbfaa992dae5b1a23ea921d6fe04f7de42aa7e4be2831d1aae13cc94997a9585401dcbacd1fe9ae106c36efee40e355f8c5e80f5b1b223e4e0ed7553" },
                { "eu", "ef2ffc5b19d6cd587d72319e9eeea081c7a990ed2b8e1ac87c36270907a936524399effe0a1fff6d775ec87703c4e4c4c7a7768289fa491851f1867770d0f19c" },
                { "fa", "bab34b22429ad68509539a098eec994a871ec38e3902d088c776ea8005b19bd78a84e665c1d600edfa0d3cf39d75b9d52a9dd0a8b6bc908f665574e487d3ab9e" },
                { "ff", "27be07ebd4d7d679abd5d8294c143e4ac49a7e970aae40eef92b5b8da6552eca1a4e070dd4089140d20bd46c74af650c752f92f9b389f3037ddbfb0015f933ba" },
                { "fi", "e638def428047dac95f00a4eb683bd680a7d8fea313719016d13d40a8d1378630b96f17239f7540c73de09e73f3ac2b77083560fea9c6c0fbc58b2246d0ceb6c" },
                { "fr", "aeb21f3608d998088d4b2a0916fc0f330094a8be4a46b6665be7cdd6be82cd93ca9e5771cce2c141a4bedda3b0f961c383874c3283d251bade04acf643a1ef5d" },
                { "fur", "f28ab0364ade86726366454335cb4fe6a6ec2db19e168a2bca1503ddc15dda21874034d30b882249ab7435d0f1e51cc1287c03a7335978b9dfc765b7b7c40eb7" },
                { "fy-NL", "ec559f1b73399f484fb8b30aa5d1cfba6f6612099cebfc152995739b4c2400f1e7078a8f431cbc3f6f4f9ac7d2afdaa9d0d9c6a019cf2028d3f2ae396e1dfb37" },
                { "ga-IE", "3bb3bfb8f6a70e48bd9a86654a8f1519955f6fadd54321647174e0b1c4272bc98b29a7d31e7dd36a56865029047b91bab3988d2def58afa9cc4403e304172bef" },
                { "gd", "85edd6ca78d8908e046c4e41a56eecffa7c3abc45fd9e3551c02f77558306ebed26673f45ffd69e4d155220a2b6b0d96668c28b39138b3fea461a46257f30ae7" },
                { "gl", "5a2c7885fe6f629bb56d88acdb1fb10f366784f1be3db9c8153086bf893fcc8038232a6cb57a95ef32b8cc60cf9d6bde654f48b28102c290032e6b717f63a536" },
                { "gn", "ad4dc41a358add8a1906aa15c09160f62f215ecc3263a561065c16a592722758269ec1e4e5b40cae650f10427079683ab26226446f13c250512911178dff6a43" },
                { "gu-IN", "98c4b147510a8e628ea0c742f769ee494d32b0eeff05286dba86282366f9ff06b1ded20314bec31ca0be39024ee6f37cd6c4b7a97cf26535a7af1dc00c45b94b" },
                { "he", "88e24dd20d4ee15902919fb608bda75e382af3e0fb407a7ee8dbb509d6eb7dc14d9d2ec772e62f827ab6c1bb9820b75944fe383126c0c92536ad7afd49657ca6" },
                { "hi-IN", "3930a98e3ce01e91e3385e1a7a99f93c9e7174fc919a080d7fdb6338128ad0c726a7e7e57d85d0719fe1e8c4115d5f54092e412451ba6fb43f2f39fc58d18e32" },
                { "hr", "b5a2b29acd5e86222b6713aa44948a46faae3d1f65346f5c2f505f8561792c0799108dfa9ae688d40deb9f0989405a158162e9605ee8f9171fa6be807072421d" },
                { "hsb", "793ff954e669329a9a83337d61bc608654f7384343a2d152d622a18dd3aeafcfbebc940dede391232fedf77bd9f21246302e4ea1b68a27a187fb5123fae6ead8" },
                { "hu", "3199068b61b008fd83bba7d990e6ea3b5beba8f773cbfef442135867bee9d2fcd3b9e4231561da623f7f96e5bfac781e5bd8fcc04b7ebe465c0bbb3e6711dbb7" },
                { "hy-AM", "0b6239c4977794fb58a4232fdc458dae5f6ebcde83e4e72749929fcd9669bd96586d5332bb80d63d4fe483de741537740efe13a5109be12a07b9985599fd353f" },
                { "ia", "843ca70838218b911107274fd6ecbf4717710dd8f587a4d6e1009473d45256e30efff1a4c2929af97f1bf979dc8cfda642e79d419b712a07599beae688445cec" },
                { "id", "34dd9805895db4bde7add3da8fc0be75a77c05d25f21a5e262441bae5ae458bdc493bdf1f4e446922f495bfe956a8d2a9ba6b6485130322b9c1be92aecde7ae3" },
                { "is", "c8419d2933833721707b13e213a66859685fed1ae592a4399a01abfec06c5e5e253eb35695fe439843cfcc2bf20c361b3d5814b0490ca52d2e9834c5b8a99f63" },
                { "it", "212c19d6d76ca538589aee7868096e5280cd5a87f1d3651633898e0fd4c2365d039514678be9ce2ca3e41f53fc4867533bb7d5bab78a9da7711fbbfbfa980169" },
                { "ja", "0bffca3fa1789a251068a5bac93302550e0ec1fed10c00e5333dcb287d75c260b4db7759673773518d094f16d8f8b5ed024a0e4e05ce9813930779c20217ff56" },
                { "ka", "a33f8a8a0e5bd840b073a5c7464b14b2dd953e9f098fa4b2d1855d703a38a84dadd24c86245afb6f33dc2796e969278be1983da332e8a80283194f61a9a0555a" },
                { "kab", "a859f8a03cb81aaa75454f98ac8733f36727ff57964cee9893e739479e8e868ffa19ea6df1e6b3a8f21884f1e0c8e1cf3784b6bfde15f153a6c5c5aaee0db6ea" },
                { "kk", "9c88184dbc5ff7d1b37be76aeb5cfae3a2819ba52c4237e46d02288975e45fa887cabfdd8763510d24ee22a722f0dfcc6045abe77637ee4497717ef7c803b073" },
                { "km", "52819c921c936f850ce2a6dd113d2be04c5143bb196c211dd37f1c9906b3a0d50c7f8a0b55a53e707c9f4d8ca13fa5d1857adc6b9b851e4ebc175cee18b06f87" },
                { "kn", "5c938c743b73bda8e7f09fb2f00f8136f4e1d6014797f43a73a1896b1048638c72db96796dee224667786ed77cc36cbc4c25475406ced9b0f8e499ccd7a235a5" },
                { "ko", "2f8ddc473e27e72d11dc41f8d0cbe3fd1dd0bd326e5182f4539082e05dff115f0b8a5f4b8e00b183d4cd8d810f8c2fdcac6c25de139ce0b2aad252bce6ff6a6d" },
                { "lij", "7123058e8defecaa369be48a6d7ad0b795368ec925b286eeae31e0a2be12a29b7ae0c75401d9ec1f907ad703216f10469f0fe89524bf082c0115b7b976ab690f" },
                { "lt", "e1f085d9db061e0b4d3c6a080bda2f25a7fc4e5ac5a69c4255928bc0c1d2091fe7496c301c05d68954159030f53792a2a7bb0ddd3f3e0e2e16e948012192cca1" },
                { "lv", "2db1fb34b9ee6e757dd65402c0af87d58c653d83866660f87569e2fa397c6626ab8cb9f4d1218193f4220bc0553a7bef8da8ee812dab3296f5cf788ecbe805e7" },
                { "mk", "8e7853497e08d12638fe4988c9a089f52d03bf934f8b75b2e72b67a70f02fc216ef332ca39e76eb344e36f186301629aa5236f81334421bbe889f2e9ec24ee77" },
                { "mr", "2a881d440901c7f48f8e48cf6f5be17664e1a02a6b0c6439cae5cd5373f3a1975e1a4b4c766e766c39cb0ae695876090de8cbca2e7aab154aa0b14bec2b486ca" },
                { "ms", "872abd949df29d487dfa5da79185a62a1c3c4277ddbea90c2063dc3c3d9cc6c570ba97c98a08bbcbe50d69a07297bff900eba1ff56b6c38ddad1b6722edfbcbe" },
                { "my", "d1bc0aee53731dba9264e7349ebbdcb1f79183aa59e7546d1e608252509073ce3593b622bbd3750c92d5189c1898fe220d0ef1411d12ee0ccce4d27e294789fa" },
                { "nb-NO", "d8d8ac604844c1ed20330f7e1a3fc3a1ee0b3b9c8eb14c5f3b7eb28647fcbef6a63e0921e456e767877d23ee24fd2297d9ab19254eb24d158cd83a08f3d7d6e7" },
                { "ne-NP", "bb1239d4ce80db949aad4ffe6387aaee6f821874f79e2b611cd4012515641cf2cbdb00f367339678e5b41363d924a7e45a2c15446e6c41fe201736e08695bbda" },
                { "nl", "1efd2e630e38f24be6c502ff5b211d1ac9adf8a370886862838c449911b05ba08e72daa1373271f6e50f606b56b1b2527cd23245b7ac93ebac388633454020c7" },
                { "nn-NO", "d631d49f63d62fab87f6d9b7e6d286fc0c7c20295227a410bcaa44e55d7cdba01faec339844f0a8ab5063dcf12506747f817c9cc88b390da1f35fc7841b8db15" },
                { "oc", "cd67283e493990fc211c808a5c243dba8425bc2d83bb3f9bf780791d9d31d5d8313dcc2b31bac58cf8ee7435738f40119b764b48ee4bf8e5afba67df2ca828bd" },
                { "pa-IN", "9bb785bb87dd16fd86ade85f6641dab529af49126616a0aa5a7d808b9b19a6e2a2aae64721d543a5b780902bec880363ee34b51cf437976e655515dc4497e66c" },
                { "pl", "f759a2029b3ef529c0c2cc2b42f58d795b52b7d076260dfca1959fa52354589e03f2b4ff68c58b9bd0cbf4d62153c15ad7b3b54c59dffbf990194d1bb03293a2" },
                { "pt-BR", "d5c66f6c0a4ec9ab2226766c236950f5b35b00e75b382dc91384c284d6498ac975c1f9ba84ee2fe076f71e86dc9e04c7abae3e70d856aeba8ad8383117c530df" },
                { "pt-PT", "8c9d19da767c9d1d5bd0c3f8525a85481caa414ea19d35c2dd2e52151f0bc4dc4e171b0f09a1b280c36c6b5f64ac073795cd3268efd382ae5acc1965cc95e525" },
                { "rm", "2d65b045090d222610d829c917ee9cb15debafb40992298f6a9440124962ec14df52b9cb5fa33ab4d881e2674b4a9d235ff60e1993f4ab3a635fabce58fe4353" },
                { "ro", "8518f4d6743e8673adc3bf005880146f4b6bd737c697758ec8354e99dd29466d2a64d363c78fca129f9eebe0b627d67956bb6b6aa333488e15cab476b68fbd44" },
                { "ru", "296d301c0fc35c6013718095e3072c33962c2ee8ac061f76706fdd4542ef59a3630dab19ab7b019d89b5a29ec8801c5011bd8fa8c9dd5bcacee5eb60eaa11d9d" },
                { "sat", "096fe8fdd5de01666b722e96bdae929520e64e0318763f2c717d366dadc1b92c347517e676d461ae55d52fbd93ca95f7a8eccb1416d2b12eb3514c564cc07986" },
                { "sc", "bc37854a3bc661615438d171f085025f436be4869eac02dc63efad14b1edadbb505e5d1a6fb60a9115b26288e62ec556cd833f1bb02a71abbddd8dc3700d25fd" },
                { "sco", "c415031d7371f59e70d264836b1ca094631f45e19584b8d6864a0aa0c12106831206bd815c92bdbd2ba7bd8c3b29dd0f5a4da403f56aaccc6101fa94f5e0ae5d" },
                { "si", "d2779e63f14cdf347d149a8d65deeef6a97cbc6e2139e14eaf1452db65b13fb7ed0ba761e675945d0f76d2ff24989cd69fd35d1c70b8d309943af824a859ffc0" },
                { "sk", "10d1d87ac472252aa9fb42a1e2a8ec3c82bc19673d080346170c06b0e8a40e0cd9331da3b05a09888d25b3b11a886dc291419548279921db40f8888135f8ab7f" },
                { "sl", "b1fef4de6e13b870e9781be8d55ce3088bbde872b5631a693116851500529a6e8c5cb9a219eb843f0542cdbe2403dd431429d6204e4b2f91cb802ddef80b4a5f" },
                { "son", "0a6d6aed654df750a771d787c7c65d32e412bbd6e29786193fb2c8371e7f11f2e1e91633aa5b69d814bbea874a76c2e083e0c4a98aa60fb131795f98f0dec114" },
                { "sq", "7fe515edc3e14eb25b111433bfefcc6022acdd10750f7f518b8cddd9ec2e154e71a7ffa248dd7ec0d6c6a1272a7088604dc821eb5b85ca9bd69707729ab81321" },
                { "sr", "e78365896a8a38b94f9f04a03fe62472c893685564a559f8781f53df41922485d121de3338278adf661d7b7be17e1fa60dafd156317373b566d4bf24e65016a5" },
                { "sv-SE", "ee2653ef65644d0db35491d904ca568477c5f7688b35d62a93454cd4aa76d185a44f0e1201ad91b8a4a8ab45acb2c96dd80fd8e894d2b1162dea90f37af1eadb" },
                { "szl", "bb0014ef1e9c73a627a91158f055f5457b0d92dcc4a3f9a3971d779628411fda4ac1dd6e61352e5efac0b5b910715b9a0c8634f48c0f39e501e35dda6e63ebaa" },
                { "ta", "0ac8038567f2f49306b50fc2e5e4132fa94b6253f093aee7a16005d9c0911b151ca127db69ff9442cbefaebd8ec513dc77d2d834ac4031a74ec80478f111241a" },
                { "te", "46544ea5d84e7eec7d307bb8fb8c6f9381e7b3138177ef9e9bff5ac7f2a31fb50c6f668eb305982895c2e34e01bc1515cedc3417ef9c954a0799969ae854aaa9" },
                { "tg", "d3fbd266b45dc907864c3b8157940449b6798cb95d7ca18a02ae8f7cea490c24fdb5890f56372f1b7b4289e2c3e286815e3e7e8196180e6ed0fa42df9ace4b26" },
                { "th", "43992978d55921596685e03ce6dd1bc3e2dc72f262de3b6d29382cc1f1062dcaa1d92c819daaec27f4dcec969e4efe83a589ec6181ec5899a09fa543434c0632" },
                { "tl", "dc0a36d995f084a65886eb3f7b508255537e45d8b7e1c3049c7e99941c9a39764dee24f79cdd4552461fb4dc2249eeefafab615710b8577e5bc808b4e5ce2595" },
                { "tr", "96a40e14805573843e21364e55b227e2b2fad2d81cbc00b25c49b213874e26ea132912530543128cca93f9175a0319edc873da09afcd4a8fb9524ef0b87930ee" },
                { "trs", "905c1efbfb797c0e5530981197ae5b7aec9100c5b77bbe2ccf8e0ed118c6011df6057790ace05e138d88f97c71df176e490795200239affb44a873c9b12858bb" },
                { "uk", "51d6b200c09e8539766f97112326cb5e0046a38a5daf5fa88b11bb3d6e5776165a0ee86e483688139f5ad8ac57e924361383044a645145a5052406bdf52792f3" },
                { "ur", "c13a1f058a6370ce59f406ec4bc6f8ca2196110c83b6540ca410ecccf8e8eda471e172374766cfb181d4f49c6b46ab6fb71802037bfe89a618af6828b6858f25" },
                { "uz", "e6c6b752def9fa885740ff0019798331e29626fdd5a15d36ce85d97f9b105fb5c1c24fb994859c3a60cb31639cfe1b747cc26627e28d797bb1cab31e5e0376a4" },
                { "vi", "16a6f80871080eb0c41283d5ef559f74ebe501593054d1f64ee5f34a8d2298beb38f4f0b161955610acfbfd9e4d665c87e8b16caa778c002e6229634eb19a8d5" },
                { "xh", "49ff2a0fa6f42d7f603144c8198865a47d2cb8976e2a302661059da576647f20d5d43f0f0b0798a5a24dc04f3df7be23a615739cf1877ce5972445f54f44fd15" },
                { "zh-CN", "150b393906945a2701baa1c4dafc723ed2a9e379b17e7bbc76e49fbfc7c3bc58279ed41cbf2817dbbc8665e9fc1a02162149ed5f52fa4d80f670abbb7b7066b3" },
                { "zh-TW", "05ae45b1342431db5e4aeb9e83101f766e0bf5a7f1a94aedf874a1a362f73082accbb1f868f2ac3671369aa3bf0c80ba4f365ca91d650033873e6e4684cfd765" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b3/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "cb7e2db5db094864da1ad3798046ec54f85878d257364c89e5bee962d5e7134b2561b0a51446a12af9d341544c8ebc192803a434b073597d86707ee7011feb47" },
                { "af", "160cdd2a5ac004b2e1d1406a0c2975a0209da9caf62e0d5a2af7df55abc5f4b2e34d08cb6cd10158921801afa12304c06abfb096cdbd88ac9c4badd63eb3b061" },
                { "an", "49ed4d6c5e5d52b6e2d495149686c904ece52f5f66fe4b9b8e604ccfbde96de17e096a2308f42823bb9910abd1709d724e5e57ca5aecb1ac47c29b52accbeefb" },
                { "ar", "4ff8b48d96eb72f7f0ba97f188a871760bf5eddb8c0f62d475f4b43429eb490fe8a1d627194f619f77dcab8f3802b546719676c674e35b93850413d752726435" },
                { "ast", "c0f6bdced6503462cbdf16fa1149871a0851675a723b9dbf646b9b77f343165624de1f2541e4d17f63a42be3d676a2dee4e006eb6869d46fb5bd9e6c9a0e395f" },
                { "az", "7440b11e55370eda9ee497d3562c6cea86f7ffe9d777b33bbc9926d3a9278e679c2a64c5f25e841d6ab33048766676984687575f1b679116021658d409c4c19a" },
                { "be", "92be64176c5c5d64c1cdbafa4ca7a43874d00d0522d9c7c87e8b944058f91a4a0fa7b79a563097a691ec41396480430bbd75bc81c945f73bd6503fff4bd366f0" },
                { "bg", "079f2fa1177097f4ee826cc3969805ce1dcc1bfa911b3da140bfc04641cc73339024ae243440b16a4ae36831d996c4a15e219e60010881c39e7077d6adf6de02" },
                { "bn", "f54565e85e6e748ae8241143520656b9e7f6b8ce103caa2058648e8f0d15662315193b1b1c12f4f71e9aee2f5afec860faf53c60364fbb60cad67aead0c08b54" },
                { "br", "ee0266c0ebeae7579814776a6e86c9185da3622a3fdc95937f7161916a4d43333ed075abfaa4593a061663cea8c16d71d46a84d889a0056a76f9fa35028fcfef" },
                { "bs", "4ea7223eadb2ed1d9a167bad925ab41b8f264e2c9b324ddd1902f4c931530364a2462f74e9185303f38aa9d540d1ed7248e2f02fac44299ceb5072cb27e57d79" },
                { "ca", "2272a9e1cd98bc2540caf575bd483565a401eab1217db0e169d5ffa9223c5cb3e27d6d8427c22be0b3c3f108ffdf38ef3c25a09cfa866cec42e5e068d4a72f41" },
                { "cak", "7bc872c4bd54ad813f3965b87091a19736fd6c5765f9b286cfb5e79e80b1498b4142c86060b1c88c5370ad78bb650a86c894e71af71c6d53db7a88cfe8976cc1" },
                { "cs", "fbbdcc2c244a89b1a5e923883bae5fe8395fd72ad58417242d654099a361253c3eafda03169df2613b920fc6c76fe9db9998a476384c5b333aff990bb5d03d72" },
                { "cy", "f0d2578a56934c5474375fad7b8d994f4314799da71908cea6561b5cedbb68706c24090629bb7ee7ac7033297090f415f2b891eaf487acfb0b8c970448368e3c" },
                { "da", "001c281dabcbe140101dffb89573a760516bb887eb598db3e9ee204a2d1346fd7ce180242340857449ea56cd352c508a888ccc09fe1ee10baeea95f19556cc77" },
                { "de", "6183ec41ebb5fb0ca0819cd1afb8f282198f9175264beb41c9db67a88628504cb86fe1ffb3c4a6d985cb4a70d0bdc9a0d06f4c4df11999d3fb548d0e89645db0" },
                { "dsb", "24ab5a9abb94eb0d2bd35c3756e95b00ef66644f49b5035111db95f050dae616badf05e9b3c7837c16d4da0691950dff68f425a64ad9f3af2931b26c595ab2a7" },
                { "el", "3e294a5b8e00e3e27aede20dceb7a8679993d0c843760faf110efb69db8c75fecc9d70eb89e09fcb03481604430e708d58f04b21061f7e793a4fd0df1ed08039" },
                { "en-CA", "3b89e7a769a47b6c6ed04582ce4060b13a9eaae240e0627c18b024528430a82dbff7015614fe146f7e4bce2bbf9a1298a5c314d570ab4aaca0c903638e059b84" },
                { "en-GB", "818eae1c67df2f1cc152a9fc0af6eb23c348b5e6680e20ed7fd5818defff65be13782998d4dd0f06659145aefca6731f11fe09afad83cbccfb5cb36aa8d021df" },
                { "en-US", "c4b82ce03e9debfff5526026fa618d87d91e1f1f4e293c644350b459b46880b1bae4c951085b8db7245a8b25dd919f801e8ee4f3865e47ba3cb9598c03c7e16a" },
                { "eo", "e3d688daf1060bdf7f5f794e8df33695a35a406d787dfcdcef34b7414742aff0f8be6abf5dbadd1e8fd80b095e8ae48c463e1eaface0d625d6b6edaa050f9c45" },
                { "es-AR", "4e6f96ae25c741f51b0950dd1865d3874d540cc9bbb64aeebea5b8c61aacaac6ed687da767ea8365bddfea02c560210c8fd3b140e1d84b20763de06871b7bbdc" },
                { "es-CL", "4c6dffa9b8bba9e4a59b2493d095770b9ab17e1d909729887b0b60c91220d14d14abc6bfdba0b9839472534973a5559cb69e48a04f471c6d7731d6addc9df6e1" },
                { "es-ES", "58cb1d9e9f013f9ae9688c7cac61ee9ec9aa5bc5087a71c921820bbc6c71e83c5b6693c51b8fe8a7767ffddc0c52444a1002824933d21ce523e16fcd99a589dc" },
                { "es-MX", "da58a155f92ac324f739085882367b6d123693f968f00af366919c70099a26f8122cc13ffe05b3fe35860f5bfa7af21120e227136d70e4545223ad337bdfc5a8" },
                { "et", "2b47ec34ec11ee8252fedb02a10d6fb39634594569ff5978613e824f547c4f10209afb2e11bf9b64df9a3e400c9543e05593a4866d9884cf9b33401abb3c19b4" },
                { "eu", "5ce731ab97f1fb0c03b120595a325379d87ee6a1099283ab3fb7c6e6dd068c121f74d8bc7a47a06580449134869e71f16b7bec0e7237a8bce2829e1e1cfc4b47" },
                { "fa", "687e225953911fd0255f6427490718e072ed088ce9add2fc173ee5729d1cbbf0449f46578e5cc6928d50fd7568d9e316fab7bfccc333a5fd734ec031bad8383a" },
                { "ff", "63ef2ad4ce5f41d15c33b24bdb9093bc640a4e6d04c92dd2f11aa8c8fbbe513ca6f5b1a21701c2567848404bb4604c5cbcae4e4394c06bf1e72f0aa21977d2a5" },
                { "fi", "5be085e872b2009a8cb50bfd07d0fbb108bc884aa40066f2a2e2739278673d26dbac553a6276049b59120bf60dbab9184597eb9c8ebc43337e05f38ce4f17b59" },
                { "fr", "dee6641ee23331d5c0bc6fdbf02b054816858ab3e97639a305cd9b364b9349dadc10dbb363b806cd56f2e7722f4eb61afe373d3e20354f99cbe410f8affbcb4c" },
                { "fur", "d5e8066fe6178ccf02f709ccad1711b3d962cea4679a1697dc42d1326044c0aa9196ed974d18b77cdec63d5adcba752ade0fdd40da4f8ddfa7a6c64c116ea52e" },
                { "fy-NL", "a2d94270fea1a45728fcb4aea1ba7d2d48b5f5e4fd79190c1a134cfa91487b1f0417d223ee0f7df41d7187a8820ecb24299950d55c79293bcb21af81f878d6c8" },
                { "ga-IE", "e4dfbfd9efd9ca3787a1796499c5f96169839e748fbca5280af9d0311ac775e3bafd04c5ec551397f9f98064fe1615a1721b05acfbaa9af55a05fef7beb7a748" },
                { "gd", "d5f2375d8fd4394a8965285d2cb3df86a91725615299ebe7a0ee4d51ec8ec9eab7f0f383634ba689d3187cdccff99aa2ab63265e222ffe7b1a0547c5469f6e7d" },
                { "gl", "57f04ef6e1a4fdde24bc7320a4d989a9b5cedbdb775793d4be9c5415885a677abfc7676c6600e62ccefeed4426c3b3081f2e8e51c3ccb5bfc6842b2cce2b64d3" },
                { "gn", "627c0e8b7b6fb544f228cdd3f83f9fec6b1c6b79e3a4f9e59d2540ef4ecbb804447d2923cbb90ccb4df76314588283fe957ce237cbfbc8fa29c5a1441f4f5d46" },
                { "gu-IN", "f2549bc5964854b31f8a37a3c5585380b1fb5f2e688d03fbca20d5984b6cb8c4824f33199b7b4b1467c3cd2e851b25f25c34cec32c1822ce44f9447677dd710c" },
                { "he", "af8f0d74e0efe402f73fe4f9c9c83a951035824cb968684db93bc7df684fd7c60dd71c58d669efd1436bfe780f76be74883c5035b4afa3b7450f4cf578cc1feb" },
                { "hi-IN", "5ba7e963b243089b08d959203a1b8c38dec75e157815b414bf56c90358f6150ad63cd9475c1498cc0f31d45075e4694fa440741fd0077c79b922397d1b3370d7" },
                { "hr", "0015ead21c3472195b62976893a2966a990bf57793b574cf4bb10c9bad4b8d2b94810bf98f7cba3812f931f811678ec198d2fc791ec573195c03ad92f719294c" },
                { "hsb", "96428d210ec001e13f6108136ad91af50b0d2b32fbb63893e7f36d7f7ae8a6c4a16b949a9be85c0a82a93820364c8afcb87054ab956bb090fbe8b7787ba1341f" },
                { "hu", "7396d1d480028fd9280685377727a2f935b009b89ac5198118bb1f2b70838a1a20df3880ea7bf6a58837f3ca2db3727e43dbf48a7464b01a63c827b788c15cf1" },
                { "hy-AM", "5f6520f880536c8a4b64daebe67851afc8c265f953365338dc1de87e3b5bbd0b9047be2daa3f7a43c497be8be703b5ec3da257702524a2e4c8202f81e0ee94ab" },
                { "ia", "e30b5d3fa55bc532765a48125532aa3b45ef5048b946eeb701d29dd5c9b82cd47b7d613c11240051a470e72978559d52b8b14dfbe1c4619edaba66ccf5e63d00" },
                { "id", "9226230daf163be1925352ed2aec559c65fac8738676ef9e0a2d545df14deb30c0792dc3c6122519366bbdfcfc7b824c5ee54cbe268d47275c3d215877b3fad0" },
                { "is", "62162152511d504c86b7f4876bc16e8e8bc57e176c84b74e2f80bc2187051b4e8aed0b8b3045c0554342d674afc069bab2a380fcde3303a8831ec3068f2a351d" },
                { "it", "96ca5141bcaa2eb657d77a740e5201902626f9ae0dd0decb99b8786e42e038f405ab4bfb2bf41fb8a54456339f5608043253c476a7e788966a68a501945b298a" },
                { "ja", "d8691cb436b3754c99bcd2419196bcffbe6b661a34683d24d33951c4fe0f13ab185a74e51357b7d933775b8769910fcda873684cbaccf2d0429f172b3fc11540" },
                { "ka", "56ca88124dd047e830b7b4d90be8084a3b0effa09db1465f1800e0ddac42f335e06c942f2a7030f9492a9b298bbf32d374104dd57accbb0923ba47523be680bc" },
                { "kab", "87c3ba74938b800f567ad463964ae5590edd131f486ab6481646329eaba92f23ef4e61ccb88b58436522b71a3d8720d87822612a03f32c3871a88b8ada6539ed" },
                { "kk", "ca138d47dc4a340703b0335102ed4098775021b04364365f07ac129fe3ae5a8f389a2b226a7e7f5f7d80aee8c7c977c6b2e7c86d2516618667674bc83f788a04" },
                { "km", "18d79dadc4b0b2b27be1da4c2a3b844991c808611a3acfcfd2d5ee04857d105b07664fed63ef290a55a02aab2b15ba628bd2f831ac325729d79d88e08f70d2c4" },
                { "kn", "53506e8979626b4ccb1cee501d688bca9c027a132f4817245d9f998a1f1c22c22f23a7e57b0a84ef85475ca6ae97e52714ca221fe828cf9e725e5ea9b5ba71b5" },
                { "ko", "da9a838397c814aac6506c1888e2a56ad4f1150f65a5908e0869f47e4662d8545451c4bf3685f4de1dd1ce81faac2cf5cf43c60560e51d40d5c68b6963f9ae01" },
                { "lij", "96679016f25cf7cdeb896169074ccdb9a68352b67d04139f202014e1fe8c1c7f29d76fe934d1de40d5ffe744a5cb39d88fd05be0af75c94c199ec3df0b4178fa" },
                { "lt", "bf72fafb7fe2b1ba1389ba5b753812983b67a230ef552171c0c1e864d2fa0106853ee09361aceb5250a0ecdeb49bd8209c7a92a6e3ca9b34fc691390c2997c94" },
                { "lv", "bcd2b0b3ae31cb0ca04d9a4586ab73c501d2088d47db2f9f31bdbc287c8ca4aa14e8da23449b16a7b322ea5aefdbb3f4fd7905e37893ae80572a81f400161027" },
                { "mk", "9ed0cb48dd7b55ec0d447d3c9715ecf0076c9cb2c4165a73e9016904e4d353c7e2a583a67c47f86fde339b9291a13fa702215492bdb3a136b0463f50d8633aa2" },
                { "mr", "a31756630efc95b291c23b9599d2cf40f526c506ce88af6a4faa347ad2397900438c6592f8a1f78e696b253ce0f0e949d3dfcef42b715329a60347df4f8539a6" },
                { "ms", "8f342b0f0f5bc25dd7629a742421d7b05c99fb771fb3f0b6a113ca2864d9f00c9441d50fbd35106963cfdc6d6eedf02433a25e787f26209e28926a575572e02b" },
                { "my", "dc450a1354b2e69b02d635d641e9353c08ad0c6dc7abfe7fd4fee5bb5033a42f703b9a55d76c4a9827e622f6e3b9a600a9de0b69a5d9b162bf98ea8748becd79" },
                { "nb-NO", "136620fca9f18af76af956094f7b486ee2ee140589de4465550809fbe2964c4999b66e1f5d892c4fa989ec0000d791ad40c826da25d20eefb004ea7278cefc1a" },
                { "ne-NP", "165baa2d5f3c5cbb1c7656127d419312a56f987b4d16c149c713a8cc9480f50ac4bbb649faab573402dcdfc4607d99f1cd7da1315ea022e566597e8744117f3d" },
                { "nl", "360f4cb6a312acb6f88c4f61fc58ee023c2d9041d7c263c50a64769a5378d3ed4d8801ce94a26c08af5ed02eee6355d9b5ec9680b7d1dd0742b0c50aa772a20f" },
                { "nn-NO", "1879e0135bc8e90bdf6c16473809147d0033decc8e888bb1cf1d13ac6d7adf199579d8d92454cda7d2f19cfe3fe51aa1327a58f270b6b92dc7e601d9db06af3c" },
                { "oc", "f4a8cd378dc7096b7303568ba885ce0df25c6eedbbca3b9b51f9e7b0fbf9811f9493baca966e9e3085b4e305878f0b7814c61d2513b6257f3caf474b811340bf" },
                { "pa-IN", "b1d9c8facd2b42e976f6a6271a484db6826ab5a40fca70564e4f348411ab66855cb63fce031e9dced67f7c0c019381cc64453d04d4725edcbc9174ad42623bbb" },
                { "pl", "43cf8ec63c87cd79e643e1b84bf7169fe7b5de4614aaa29af75bbe17c6baede3e43ad66554fbfa4ff71be576408125915ae9444b21ae94115ef259d7734da933" },
                { "pt-BR", "87ae86a48cea3b3b642333c9a8e6d9776f3e15538b22ede0d0e25ad4b5d4c20825d91b05630d5c79f34572212bd6e6db60a3b5d5a2f4099b37e1868f91fb74b0" },
                { "pt-PT", "f04d75809dff522a252614a345e1f3cb34448de29ed4d429e37f0bf63a68b65822d4698cef65459b297e674f3a78035f65824327c991575c3c754096d5538623" },
                { "rm", "b07baa9da01a5260d260e803018a2f0dfe0481af799c09277b40901d1db66e650654dbf9809072b834b361556057dfb54f9f0a53d87cc2aa2f8c95cd67000a53" },
                { "ro", "bdfc5347ce253482c930d333105aaf9f9ffcc1a94f902e5af76a4803501bd216d09ab9c79568b037c0f7d0371b468b8bd69b5f38ed72faa3f06bc4a6cbae8c52" },
                { "ru", "329f912a5817731bebee5518e10f55b7a30ca6b45537b017d678ad76672dedd7b5d5b140dba3343ab9d192a3311478bc19d0276b22d4aa56767ed47e8dbe6c2a" },
                { "sat", "538dc018b02ed0ee4d97d6def77c6e2b487e53ef0463a202cb4581eed44b3a888b524f146ac69d87cc655a9511505009e605ed5f3a68a88f9df9ba0c2c3a75d9" },
                { "sc", "5b60c6f47c4973351b9ff56b7582ac406cf87b949143073403a7f815efd16094142a284546d69b284174e43dd4f2105317480fd03cc545b1161b20bc5a8b8f1c" },
                { "sco", "6aa9a37ee4e85acb9d6d3e5989308b4fea50ab63198ea33ac6bb114e66b92b9b545b91b226fcefa3e11dc70ba30c11457b8dfb766c9007b96c078ae750c16a50" },
                { "si", "c6d50414a620c57ba10094fd058dc4bd4d2f3d8710f9cf25c7f5c43c26e6e81d1cee716dba059971d6f8d43a82578bd6dfd7ef0ae3c759b4746d02fb540aaf62" },
                { "sk", "25a80a9ef2094b1cff4f41079791baf1272171cb1fb8579bb45d8f0cc5d8ff55e2609b89f62ffe6f06f11e9a48da90d435e9e218eb207d5140fc4983420ad028" },
                { "sl", "ad6e12c6376267f5586f8bdcb56d585243c615be7f491f42a3815bb1ee9dd333d9cb2761cc1078aaf4924403aa42f213e216f549d77dbe5b3992ab81a7e7f519" },
                { "son", "6f9e2d04933f2c54feca4b84bbd9a91b8cf46c01785cef23f0281bde6b87554ff0ef1cff942524a76a00c713a3f3ab16024da87ac25a83aa5923e428e5d7b15a" },
                { "sq", "e246f384f3adcd959fa0f24d6a4785389e09697a8552322097c65b684f4409e67295d3d4f4e2e10d8e07ba518418e484fee2f0a1074d56136b20e532a7c35ebe" },
                { "sr", "e1bfffdb3bb037dee43bf71149bdfe263ebdfba3b3f0b2613cdf6e24627b5b52deeb92ffb3343fd9f8705118bf8f250eda64cbcfa1f1a907d0d1ae3797782bbf" },
                { "sv-SE", "02c532d636b3a24685f4cc85f8e06ef791d285024c3cc1ffaf1231caa4c07635fb72cec895bfe39d07c1a36aa796d19697a54dec10cc333c92e6fc73a67f7508" },
                { "szl", "b4e9d750771ab8258118531c5f629d7f323392d8a768e2b743572580096be19799edf1bf468f44dfa7f36cb59428361f69f79091bef6ddf6aa9f008a7b2cef99" },
                { "ta", "311d5739d682c46ec59c01abb2ab03f3c99ba0067eeef4a202b6dc23fd5890f1bbf859b6ce8325377eaea8a60804127b8779a08e58fff4dd61d5d60174609358" },
                { "te", "c87f73aa7c4f0fcdc196fdad8357c07cc3f308e55ccae249606d83e9ecd62cd9d737b8df86545d0e9a3f924d8499f577e1d10c34933285f611c7610fe5dcaa9b" },
                { "tg", "55698b6ef1a468dbfdb08ba6205a54a3cf102e5268c83bd1c6ae36791960beb9bc19c5cd6cd1b9ed60f381123582304cf48b93b229c1bf288736d54cfb2e30dc" },
                { "th", "fdd7088e5bb5a11341a4f4df877cd009915ae5e300a4ec4dba9b200b84b57acb1c1bc6ed41ef9fe5d778c282fcae50f2bea3492b1cb368edb677f91dbc81a8a4" },
                { "tl", "b5139e7074dc68519ff4560485c196a0331bef49b202497ee0439eeca3965f128b4a8d01a331457d3e5b0936d850b78fa97ab7294360ee99620fba5e5bbd32df" },
                { "tr", "5234d41ae63924c9fb5da5e361b53d8672985a1d96ab15c0dfe774b62c66b9bf649b8f8021fe72644a00937e532d12d206129e497de9cea55789d39501086f0a" },
                { "trs", "b680ab3d810ad841c54b6b7151137710bda3669ea6ff94638d07375972ea37f5874c6a51fb43c0947fae6b96e1d259634825f6fd4413d117a2098d9c300d75aa" },
                { "uk", "25928c5d5dc190226f9fcc6b6db7d01e3c4fd88070639c36cfdda0e97775591f3404d03f8377a9c11dd8b012fb6b66aa781197cc70714461be7c2b4521bff99c" },
                { "ur", "9368c97d0092b5241f14f89f63dbdda651d30cdc4873f2e83d42fbb774f206182801a747ebdc6a4e44921ebce9354d67824af5038842bf05491755fcd511b23e" },
                { "uz", "3bd06d2cc31567d6dedcba02eb67ac258c0eb9789045fd041c061421b15571bfed0b91f0c755c065d25b5538a25826ab3db0db2bc1f0195d988c42208e433b85" },
                { "vi", "a681f3f62b8b62610a6e860134349d479cd99528a9fe576cbccca12f220d6f84441d330e536598c94968e0543205ee2adc538aab72b2d4efe88b0459921ffcca" },
                { "xh", "da4189b41fec3ff33b490b8c36e8633b037adbef0e9268d22b231e5d78a055130fcc7271a5ea8c25d1c80b84b3995519aedabe48bcb583fdd27ead9016c3305b" },
                { "zh-CN", "de5d1290de5288b66e58650b152a10b0792cdff141e45a9b5d6c4b4d3e20f0013ad59509e48e7ce3d209e93b36de042825f6ebc83cf8f6a219aebdae4b9f99ea" },
                { "zh-TW", "dce363343113e376327260d4b6546caff00685fa22ab7afe7121e9074cec77d193101f399a216461767e88000811d4ffc58e77a5e99a239dc4d7f491e6f880e2" }
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
