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
        private const string currentVersion = "76.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/76.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "106c4012f6932145bdc2cb0827d4da804ff9a34ca65638227857fa5f786c2c9d09f7cccc246cf6e48fc20ad0604b930eb34df9cc777b025f6b0a89f1970d0db0");
            result.Add("af", "5211ec3c33d69ab5dbb4a7d6f7eb169b279041301bd013318fb80292377125281637e0b31a18eb70f3e45e3ce37847ba152274b6147fbac23a15cd8e3104ff26");
            result.Add("an", "69639ff3d70c736d0f70ed36b45e03cce4eab9a4ecff987176677349ce7df42cb81df2ebb5b3dfdb9b507977131ac29c651eec5f087caf0b8ad114cdf3e9724f");
            result.Add("ar", "6404b807b4e0d1831b71c53966663f34300536bec8f1ff38076f5323e599f6b37b52a175a6c6930eddfc09b7c9e958b8188ec0d90871676b1b61f50315379f98");
            result.Add("ast", "e16838052ea6554edd1a2dce87c22c4467ced3cb791d7e56ad1327e9628ec4928ace639bb4f90f8dfbb21e624caec9b9fa429b257723b6349ac4e76b66b3c71b");
            result.Add("az", "0734259e0a1e3aa81c8a38dcd8abc10c8a880d9ce7bae4dc2663458ac2ea398ddee865e8c24e33a38e509f3635b0601e58070d29331fa99bd2da78e4613498f3");
            result.Add("be", "b4d5b6c9613ad858bf6b962623975a9025fd2243bad903557b56131a59a4d6480b550ef3573ad0c00e4947ab152fac1ca4a5b75b9a27a30c26680244d809f3cf");
            result.Add("bg", "2aeb9f0370870a3eb8f21bba1fbc870cbff0c1b8caefb12c07cd22b4641f34ac5c1bba60bfaf436e18e1f844928d1ed6a45f694bb31cfc563a51b7a97d154e8f");
            result.Add("bn", "aa3d14756e28d3f72d4886fa01b6b20349960dcce38bf1ca968b47a667a3bfeca477f0f224ed54f5ca06740248ca4e231e0453fbae64676dce28688b5172aacd");
            result.Add("br", "68ef81311a852d5b2aacf59ac64f7f150874a020bacb504b59dbc397a8555cbc72b9c18f0c3bdc6ee44caf5779aaa90e5c20865f4d11db75c3c40b99a43c7fdd");
            result.Add("bs", "1857e1132951fbea623877a732fc96ea7c3ca66854a7360f7edc646089352fee328be1723cfdb9a73fd2d344bb73193eb3485a52bfe18bc86d2fca74fabe8f8e");
            result.Add("ca", "323e60e9e6bef14b317cf57ee2c873723f3ed130c2b841205963a6945f3629eb39342f7e9c0ce1e9ca07fe964f0f4810f3be44c8f0f9b6d8460fe6c873ae672f");
            result.Add("cak", "981384baba5a700b7ceb707ad125d484825c318e8d80d516f35d845f9cfa06011a495c526206f7f3ec49936b0e15d8b4be05b7e6fdaa7ee978cf960facf1cefc");
            result.Add("cs", "63f3981cdc02c158e8709e0c06b0107a0786e698c7a471602174c62de4210bb431cc24ad3b6c7b298c32b46294b933ae77b3d51c4fbcfab50c7030c2ea6c5ad9");
            result.Add("cy", "6479c72458ef5b45cdcf8d58914f57f5c6f439aa40cd4086a96f65d2d03b673bbb8a5f54440e36ce9034abf1dcc58cea2a9bb998e71484cbd626537f3f5e261a");
            result.Add("da", "cb4ac253c6f681f69f4f8a35267d3d24f033336c78feb82bde000d0540b0e53fd7a3860f7b1232cbbf384cfc4a83830f23639829b781def2c8c4e71dc57f3c1e");
            result.Add("de", "6dc7f019317cee4233f3afcb4d1f650ecab85d36802c7e8b1b98729acf7cb2f9a03b9e8fe486d7633c10f6ff13f6eaae316ad020cf91c4043220797ea407f4fc");
            result.Add("dsb", "3004ec8b4c9131c3d552ae73990c6170b47d1f5bd41d8a67d4db46ac1a5b0819a5cfd2137c51dc7b54d72ed3e3775087206bb80bbd5967917909a3cec95a54ee");
            result.Add("el", "1692ce57a3999852b821a7f0c3dd7a0e2143b9fb4a706dd15e2100eb74690289f85cd538cb58e8ff3ed2a437f0600ac1b1ffe6cb4094bdff074d145cfa96e48e");
            result.Add("en-CA", "ea592627eba41a311ad9a2777845344639f9e72d425a8a5534c16d8b1bf8e6557252f275d17086cff0f386e8fa6bc1817fed7efa8f5bc491a5fd2f9fd609ffb4");
            result.Add("en-GB", "21676c1216427f1408c0524c20601270987a9778a68ba0ad80758dda1282729775a0572c06a35176a08c175aef952086ba194ed4553d00483d80ebc1f77808c8");
            result.Add("en-US", "dcc12a4092cd3d7cc9be1f3ecdc103425f0a5ce17876dd77e2500618689771a7ad2c4481f3e65edfb818bc3fb68142aadd74f9305a28cf1ace30856048460b45");
            result.Add("eo", "c4460b082688ca4bf1dc7a6b3204d9d9a2c356f873fadb1e3ee500f9e64bccf68174d547cc81667d0f2b57dbebf973b99f494efd1082196a1fb8abd0771eab48");
            result.Add("es-AR", "c69f4a31c125d80de009e0096e5c1d4b9c81575514bf2b3035a71ada5baff5c57a17de340c31e2a46f1769475d166fe4e194ce307cabcc29559b573a8d421cec");
            result.Add("es-CL", "2dd255275adda8e4c56882095eb85317a8eb7d5b988af4965beac0d0418ff21b384927b57e4bde5a6a02502f8f6850c9f2fc5b1382e54df6fdf93b5949d9954f");
            result.Add("es-ES", "277c80c1ce01ba3c8f1b4681091aff25394035dacaf4b9f82e53fc96fb7a4dc479190153d99e70be3de8ed532fcb0b0ba07498330b786b94d558c819a249a6b4");
            result.Add("es-MX", "caceee6647822d6d3d6ed27b51c3e04e46b2bbf43fe4cdae0068e6b15090c465dcfc8f3ea3af35104f30ca6d15f238e6a5f5e6314345f936494f5bfdd2c6aa56");
            result.Add("et", "4c04bc02a6bffb2e6eecb811de3aa8282f35a37f0f726fdcc5909c47b282a4cea9efa6baa4e487e8551e97cb2e11737e5382c9edf3caf5a45e5a5c397e9c1c26");
            result.Add("eu", "45fb9d0e64ab23fa44d476f7a34a24dfd5453ed5022341c8881b23e7bb88e29424883b02e4d4469d4f214f4d7a21425d12aac545ce131410759e40d6f4c24f01");
            result.Add("fa", "f0a1496ddafde6bb5ac4bf2893d7a7d73e39f09dac17691c90f9a436e6dc3702a316e14c0a6f45177a92b6b6f3e70d05a4fe7fe585a70d872b9475163637c091");
            result.Add("ff", "87d23e9c16901e8433d64138e829343ad4790027cc8b85e721f146af10ba33f4c0b06283ede2eec52d37c9ddb0fd6fb19c6df1728f55c6ff1f0cbbb21040ee14");
            result.Add("fi", "b57a3810010ec6f11f5075851c7f4a4038353b603b84aa7aa33062cd76be0bf5b976d6b6939408071a319374419b3f09ae07a53cdcf945495ed72ba8b7108fbe");
            result.Add("fr", "d05a5132f250ea78309cf77797448c9af430be874666606eab4e9cbd0a278ae48a395bb6445713d3180a3de42bec73e98cd91b589963d1a9da7009a6dbf05349");
            result.Add("fy-NL", "cabfd13d3fb8547c14516c5c4af3fe170596bb19c398f80216cf207994240d91ed332c55cf62c275fe78bc7006c2b5936ce82b4226dcbffba363f23ed5e8f584");
            result.Add("ga-IE", "2d532370e9aba101dbc4f712e48acdf00e0447c50ed450742b7204fe57094ff581247c831f13a861d80740f30728f7e51e4129f499bd294e3e83d843e9b50222");
            result.Add("gd", "fb5d514364bcfc7bcbb622e6cf5525de20b19e6f96b2170584d7e000c03bc8399406e1aa7c2aa7eb9bfec1d3e5c4071c6634768db20a1428ef10bdaaed5d8187");
            result.Add("gl", "6fe2af7a7214033967f045f777411448c0f11a2324d7563ff0fa19f72ea0870ab92879bccb2a8a485be49318782fb75264c8391d7c3a21d4f927e9b58dc232d4");
            result.Add("gn", "fa865f4d558d130d49054ed4500279697e69caaef792637472166a0387cd0c497ee776b593da665dceb88af31c11d4f4b8a3f08ec6e5a1d009edd6964799aeec");
            result.Add("gu-IN", "35b5ec8452a07d1fd04909d5e9d52aec29836fc427d72e6b037da2ce3d3bae3af7698d3384d31890b616e62db69336b0381b0877ff2cebf20f3260610e67e1fd");
            result.Add("he", "1152758c3fcabae396e45165d7769b054dd07af988f1f397c429333f3f62a0ae606b98eee71b78e958cd7417e9f013d952ad8d861f54b2824bfea906cecbcbd0");
            result.Add("hi-IN", "911de34e0c0df53c6e36a928bd30347317c1705f1d412ba10b1d6f5f6294c09ffafb85f51861e16930bd36172f66fff20520483dae5b9fdf6b0fad8db3c44851");
            result.Add("hr", "810ced10f36efb345af2d2c8745504feb82147ef0054338f3042a121ac995912e1c140d55cb6f3fe167827299231151001f94423585a0bfdb1bbce3f752ed23f");
            result.Add("hsb", "c45d7b78814862c532076a059e8222cfe59ee015ab35e5419da260df057b288e8b4d8b985e1d3bfd32c638d3c0cfda075b4ae7326a8d07bda05456e997f7d3ad");
            result.Add("hu", "d93f5af96d2e501900e5a0e0ff765f5c1d721a42940c90d32e2adf845dcd707930cc61df530643f81ae3420bd876a92d7f254168923641dbd6f2102a3eb10633");
            result.Add("hy-AM", "b358ddab8dfb57d28101584258662f0cf2e572540933a369f2f0746ed860a77d95261add40efe1c4bcf900ac42ad715f6dfa9cc4c3ebde883c597ab6d5eae0c5");
            result.Add("ia", "49d767038d9274ba36c2b47f89781d555dc916e6e512bcd1af8d52fdfcdf3f344775a05c6c7de44ebf82a1f97b46ae7272b596b55afbdb2f1711334bc7221c6e");
            result.Add("id", "ae2890a26bea6eefcd7b392c443dd4ce4d6affaa5d7d65c5ddcb5fbf159158ed832cb894ad95e3514b514c4c1e0745cc556e91a80323689fc89fda2b0edeac03");
            result.Add("is", "fa7dbe321f5aa6f7e81e741a76d43188a4ed2e7f273f310a2cd319e47ad6b3b808f392fc7a8ded7e77e07f6cb6bc294a9062a4cc53f6c3de6156584fbdb77c78");
            result.Add("it", "352c31c09106848e0449f7ca680a1e3da6e55b65d5957b84fbea65eb00cc21d6c1544388acf10861a5406483f274a3d5ebd6b217769d623cb4abc35937561be3");
            result.Add("ja", "ef5c8f150d46d8e4e9a3c724d3114083bd82b4d8e7d49ea8a068d62a002fc169c47c27c54c0536360d1b98eacc5f94ef1cce880acb61bd7655a088800a86910e");
            result.Add("ka", "c485f4e819537234793d25177c489e98696c95471859642859f5a88435e7defcb2fdafb5a3a2983442b2b70a3e983f3fb5da8470193850e6bbc8a61b86b088f0");
            result.Add("kab", "55e1af18f0c575a1688cb51a1d0d64d485ae7fd6919307fb2c5e6f8392bb927ba8bed804a335602c13b524d3100d4cdabc3ca85169056f0101366fdcd64e4b3d");
            result.Add("kk", "3cb8de6ba0100eb7384d7c4a437f6ce0c1334376d97c2c6498a3e77d320c79948e43de13181a7729b0e4ac494d7308acf6f22a4ada30112c5d3abcb5a71590cc");
            result.Add("km", "9e555cce5044da284814aefce583a82f91b2f0a4b5b00cf172b681ea1e9e34c2336734f86f6d3f1e668b2cc4587282c6fd74a60d5b5d36b467f7e9f3fbe6e573");
            result.Add("kn", "fc6b112d58810983e2f49ea89f6b89817c2d9fe89a34608492c05dc3ed8e8662dcf92462aabebe9af83f0216ec5f6b65023af1649c7a0f880d3711aba639d840");
            result.Add("ko", "f8f90e3e1fcdd3fe834bf458f7b315f7e61f034cfcdf92207a39a36ec32eea2f68c5f3f88c7b98f44b9329efe4f4bb20a09f523372d8e87d807ab3336b91f427");
            result.Add("lij", "7d3494f7571c774a56e7bd22b9c6076e23de09b894f94c661cc6eaf0c84507ec9e1371905c401dd16fb2a3c7b9685a2d788885a38fedeae8d0d1d21d07bb000c");
            result.Add("lt", "e02d7cd2c8e85472254f784bf8decc18a117c6bab949b4bbf9e321980a5504f61819a64e598c67c2a35047edbe6049e402e8d8e6119393a30d3c5ddcb7d04bf2");
            result.Add("lv", "1f7b9d2ef0c1b8b674c8f6d23c0ae44547b91e8412147dde9dba19fe801332ca68cdba634960b767152170912358899efe4e77290711488d5137b732aeb9840c");
            result.Add("mk", "20741518093173cab6354e09d2d72b216ffd82dda7be6d1e9a291de1e0eab297ef75fdabfbcc9dbd327513818dd5f16aba52d49132882f9b7b5ab1065c429cd6");
            result.Add("mr", "0dad18907d583681e07593099f3397bbd225087746a1df74e89356c7abec437770718f1f3ed5f2f74e03d4862b30da9088dc2a3b184bc7c5b24db1823dd10811");
            result.Add("ms", "2aa966b6071e62b70aa1ef772bb5d617502af086e9a13a6748908a9a0539314f4ba9e96736880d08266b6dd909caca85ecfb3c28bc5e48980a6b61f09db4e85e");
            result.Add("my", "61a33dbf04091d62d16a72d5c9fff384868e993a3123235a1ac3f29fbe96b62b7cc0e88e1b09c05b1213c01d36c1a2afb554bee80dfcb9430403d754658c7369");
            result.Add("nb-NO", "694d7b3718b536c23a3b0a7b258782d541b26c28c35dbd35b8d05d9beadb5625ad34b56a82edee93c8f48a8095a63b77913706c5c801409b43c906e0aa32dfe8");
            result.Add("ne-NP", "965da8836a2179e5327dde6402a7356568ae5f219a6e81fb1df6e2929855c618e0b6d11bd329bdbb2ee37db7ed94cbaf99af7e1ca2dce1ed27116996c5e68e8f");
            result.Add("nl", "272df1eee6c82ce0c85d8b106449617173569d3de3604ec7ff18234ec0a77ad309928bb915f1faebf84623d4911a9ab45a68dbef149a5b28dddd1525ab6502aa");
            result.Add("nn-NO", "5f6585c108b8c61d3b00fec48dfc74b456d21ec588d261c7d339d62cc3ddd3402214ed6f4045c60b278c0bfc7b63a83ea0c2f5e856405da71274ce2cb51779a3");
            result.Add("oc", "3e76fcea4ff4b04011d3e114b612d197466c786c4df375f0c194d69eaa08bb40ca521cd5831d46ade7c032013083af92324556bb1ef0b319c33c1e5279ce22df");
            result.Add("pa-IN", "13f404a2a898b8c20048a20a708065753740d3d639efd4691120b969a96d629efc7cc40adadab96bcf2467e8ce206b8e9cd8abd671cf78d6ec2c693dd9aacc2e");
            result.Add("pl", "75bf84f8db34be55a1b5be46649db944f247a27091fbdf78b307848177db8ef0b3e40676f2f9d7f6db58f1abba15c184611d5db840ff8b581157aef6ef56637e");
            result.Add("pt-BR", "7045139f3a9942c6b2da4b0f42e1a380ae2739cef6fc9ca6e03adcb8505c02a6b5a7d74baceb90c2758904ef996c7abadf667c307bb8715a69b9224eb16dd408");
            result.Add("pt-PT", "0f37cf0293fdd2a555786b4dd1074397f131f29f56fd1163a8ad0326e9be61ef8442301f7a0bc7dc982ef50e258691062c0379a45e113d90382274a6cadb4f5c");
            result.Add("rm", "ec389bb854f30e2e387c5c97dfd902853c2b15f9f3e882fef787788f94d48f1ea58622db90fefc4ce59c49efc78882bbbf56b39b7a131c4f0b021c0ac56c3c01");
            result.Add("ro", "f73ace867d686b63d8b02e137f2f58acb78f99138eeb7858eb52d241ebe2fbc3bfd4d4a1a317ec35e2cde2a1c9c9291710f7e833792e60c0db412927a3f427ee");
            result.Add("ru", "91f33e3256629eb50fc57a57ae5e092addfd68cfe02db02026a9020c0c92710883195b036ee2e2030a46b34186696f8520f6e872e878a015b3c7045f8da1a530");
            result.Add("si", "b63ff09401130b0ec6ece4d0a552b89534f8097dc6a0f10dea8b5975b168a7e331ed660887dc842621f5fc0a5dc444c5fedde24a0da4dc4a567e46afe563b575");
            result.Add("sk", "c6887fb80d44d8f72cb2383ae6636b25ccd36d5cea8b7e35563bdede24ad91ca54e06c2c7e3b4061510d9f1cd9ef38ba741aa3d8ae79f30d75be12e01ba5d39d");
            result.Add("sl", "1c59667d5aa310ca19a30fb3afd6466bdc6c8ca1b6108606c491fc0d513790d9eaa321369ddc943efd44e8dac5955848a2d61ce9dd20a1cfa992ebe48c29955b");
            result.Add("son", "5f9e0f1fa8bca1c2142a1afb4bd15a06c6a8ff9f19bb9b88a08b4491ae0d5beb1fd11ac1f38a292d064b6935a247a628d5f6a37ac7aafc2a7dbcca3ad6cad7c6");
            result.Add("sq", "c312775f63b3a9fa3d1a669010a00a629064d6cc6595d226344fa812ca094ffdc10dc36d6f80addf3c5e89f7b43e48d490ae81cffede8072437908f663b9c1bf");
            result.Add("sr", "01fb1cd299b3525b247bd5d8555613525d71d2daaaebe38476353169e28f58abddeab9e773b394ed41730179e870831113abc68db3c476f2bbd67ecb9bffdefb");
            result.Add("sv-SE", "1585c3b984d62c65490c24ceea89f79a34e01b3f360e48bdbab0432eee310474831c50d21dccb15bfcc9174b14342b9c651a7e4b7406dc891cf2b88e987c43a9");
            result.Add("ta", "db51e81f56aa4b15c14f364d1c5c0b5b8515b4f535984dfe9ccc876d7e9f7c219a110f57192fd2d1a3f1b2f7cfecf2dfcef04cfa2918dd992e99051cfc3ad63a");
            result.Add("te", "74e8d9cba5dc9040ff2f2123220b37d39efa47ed953a1701aaee0677395172ec713a38a87dd8b999fdbd3490fee74878194d55c5e8a28caf625c217b7b030e56");
            result.Add("th", "36507c684e1bceeec4ccb89fcdb6d6b2f7d058979f957185905c405a8c3223d220da68b42d57b4183925930d7113be8213716b95b094f62cee16fa0a5d609b06");
            result.Add("tl", "9a18e4e98221e76baacf4dda2c7dfac24ca82d941820db1377448dda2742c1dcaeace9cb46de70915d70f23d5a938a14ba91ee0983c2959f35a38fb761f18284");
            result.Add("tr", "34f154f30853f83ac9fb29da64858946e903d5bf433006711d8840400ebd6abc937d12548c765e30632ce584d3e83bb9868ca3aa0ae26c576e907d1f8d63060b");
            result.Add("trs", "880830e868d58ecb840265e8e806719bb50e6294478968aaad433e8ca21654b2cb58cbdabe978961685b892a191277283a0b09e12b3fed518bb8197b962a7a44");
            result.Add("uk", "4b2151a83f3ebe042da37da87ea25364f894707639fb319df5a9cd585ce12b4a50c170e41a86a3be010e30286d0758755f765f260ea6e665b1e92395078187bb");
            result.Add("ur", "bee7623f60797c75aa3e34455f2d0b9dc7efb655021e70528012401b5ed35ad18dc7b64d60806c868412206fee882954efb4028caefdc3b0b76e47bfa056e379");
            result.Add("uz", "d01e95f8490ff080ab51f0bc7ea7acf5875d5c56f22dccdaf385a87f556111291704da244c9be823352741aa5eb7753dd3a2ec93d81adf016e550587221e2bbf");
            result.Add("vi", "ce4dca9051365f5a32f8f1e57f7ea36913413a290104caa4a16162255cb316b02d2f475dc4cf493a946b2b720399a0c98aba82eb3bb4b74b0a27c18f0ac15ef5");
            result.Add("xh", "3eed8732654f41d34fbe155483d0d3db6e1720d33d9807620541bcd086570f64efedbc081061e622634927f4946185db078e66c2148b5d3912cad921f8fc7efa");
            result.Add("zh-CN", "859d87b838e51fe288048afaa32335177982f0d27d5bc3897cfe08c95e3a818bd8cc66b595f28df74dbe51fa024962256cd6d7d73112fe9e29e0aa3621059b01");
            result.Add("zh-TW", "ef7ee55589fef79cef5491daf6494a80c8031814ac74622bc24b6d7673579a4766f471c9902a79c75c71be9a7ad612346a7ca6e368047cab9f0e4faa6948a309");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/76.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "f2537ca43732d20ac1ba9e761d2b26e5201ee1e423e3ddb728871703af2e6a63da8829657f3d6276e91d1246dec36220d971f3a42421d7c6d99a36d7cbd01f99");
            result.Add("af", "1cb49a50e4f40af13625e5c7c00db6f0670b32c2f39c585e0ad7faa3ad202a74a3fd4ca56bf0a6c3e37f97e56c1c789557eaae72e0082030f293ace1694d7a83");
            result.Add("an", "56b460bb34e2117e1a953563a7e3c6c30578a40e64bb424fbfa69326d9ffd3c741af8f06a9db64e6d0b609a32374bf1cd1821844aea91bfc051eed2e41864c96");
            result.Add("ar", "7604d28a9efcd71396c8737eb74f4f84e15c0e3795502601037005e6f2cc36314ca427398324e269ab9ccfa82cc5c44aa4f1dbe610e4d1b661f7b0eac8710571");
            result.Add("ast", "99086e92cae4b0179ede81e163a4dddc3d234bda467cf54b4e7b9fe3b133bd1633462f0c9e5acb889ae295fb1ed6c01eede8aa0f5b94549146ec9a0f5cccc2d6");
            result.Add("az", "400271ec2d2eb25e63ca368732ccdcedac08f87bd859df27ee5d565c4b5ae1fd212882606f08ebcdd9da2d7b8de45a3d4a6383d2702153d039f503171e2b181a");
            result.Add("be", "227179550186f8ce7b79e137f4a37e0b064423b13559fd47c76de545e15bb1ff5dd0351bd47026446cca30a4c3b5d51589323529f2b8bc64cda72726c2ede78d");
            result.Add("bg", "a2bb7cd190929ab1edbb47b7d213d02f40784e0c422167698ce1251fd52220edbae8c16a886e86a3dc478b19df976b175fa356aa1302bb50ffd97ff8e621eafe");
            result.Add("bn", "002ea3afacf90a7ce9436fc4acaea60a49002b8851175f6756c29da48cce259b60e95f9928caf7e7d971ff6f1b7c3a20f7f0458e8d58553f8b93b06a27a623c7");
            result.Add("br", "a37b34c07c679d8e8025714f0a3d64be36a1d58875f9b0856952a3817a597026d2734eff2c9e6c15b5be0e47dd37bdd74702fd14c881af6bb3065ade67828d09");
            result.Add("bs", "5ecce75eaf0301eb2d4c2bd9b10c1163292d12137ad2515614cacfa36313110b9fa416313011776fd424f0b81ba6e4921b99af8e2e2f2c4ca8c2d0d197789795");
            result.Add("ca", "2942255ce71edc0555da00fb0455e1c588b213fb058fddeeaaf50e1d42264a93f511204e7c3f6d7e7485b210266b0f19b012cd57c5fb7c7f883c7d1bed9ae0cd");
            result.Add("cak", "999fa39e5f9a721471f6b3cf16efca931446560c94f4390c54d12c77f9725862fb1625918d06ec6042ad60d0f1ac7cf49dc6ca68f45c9e9ca1aba4d4e18bc453");
            result.Add("cs", "8d17fc54eed9bd278f9832e34d3a9be82781bda5fc336673cdb75392c4fee3ee8f2ef137f36f9dba4fc59c446246fae585c9a761cb756f22b08efb7fdfb777b9");
            result.Add("cy", "aeaa2c7373365bc2897f8a668291370b1c1433143b8549527ac0a23ef837c3f760e40c0aae071b4ed6466d30d483194c9efbbc77d149f940b1bcf681724326a7");
            result.Add("da", "e79da1e1324b1e58cc9394ea76d4b5439820acb297b0404758b1bb121ff047cd23c5aed699b05cec3076603852eeaf6300bc39b252f40b973c2042206f0fa45a");
            result.Add("de", "ebb6946b7002be54c47d5540a7295901e32f167aecf83d1c11738ca81554d5f6b19e9bead7ff6b36f70c372986deb82d5b187742085fccda6b4d9adddb14ddb5");
            result.Add("dsb", "dbb44752bf5aebb146d231f265961d3a93c5951c152363ad24cd5d41a97b127912ece58d1448dce40df34fb15553f29db616767d0c9c0f50554c1a139b424c5d");
            result.Add("el", "e4b39e9c1081c1cee8d34e49c154f8917b1f1b695f17dcc9060124f5b35fb53619ade7e0dbbf39c618bf3c158a9178ec7221821bfa712696c76d371463ac8c7f");
            result.Add("en-CA", "6a99bb6a5f1f3e40d8522dc74b4150368d9d4c82431072226ad589a7b1d1edad7fbe1053fff556b4bd9da1a8c4ecd8f58bf42427ebd75cb4d17219f492f89fd8");
            result.Add("en-GB", "ce8d3011a096ad34b01b00c12ab8a8317de72cb26f3f7122ad8085dd3978458b6b4d1a211772e330a28a672880da0089608300b25de0d9dbebeccf4c313d726d");
            result.Add("en-US", "f3af5bcae61ac3ea722abc90f4b560c5628319669060b64184169b0b55c9410182739eae03bdfb8e5c61524c8b4d2cdb2893956e8bdfb2c9681b14a4c9d2d3b1");
            result.Add("eo", "01484f57f1475a11b29f232a36c7508cafa1ad6509fb0c30ec5ea09d16abc72210093a8bb03d6ae70d8827809c513fb1c46433fa694a3dc00e39c9c85e2bb12a");
            result.Add("es-AR", "d074b20b0c202d78ebf3e541cc044583c8103722d6fbe4b4e080932009c80fc9cdb7ea25d57312933cf17967c26ca5b15782dcd3f91fa3ed7d2987afcb8e4984");
            result.Add("es-CL", "c8589639fd64a33b0b23196dbab993ff1060f2f517ea06360ca5747e3bacee222d8da94896bad8d1ad2ae3264b7d120fd3806b4735f2e530d66294717546beed");
            result.Add("es-ES", "eb9d0bcf6a69b90715d7da533253e922006092a7a8f1e9d9ca7a4f2d56184ee45e014a9c75fd608829aad8b4739d32e049f4377c93e71a2c18665e0274cc42c3");
            result.Add("es-MX", "97f1d3a2ddea01eec4b6429f8b74363311a74dadf1927874bbe50044e82835e016eb5f5cb90fe9ff9c6b96103a10c8d402ba9e68b9dbf857058e7f6de08a2364");
            result.Add("et", "3ab6593415da9d771027feada913d6010f0c45c2b9da2a7da2225cf5830276bbe7afbad787809f346221e267b14eb70a75b0cdd35333321702ba0b63e4ad4748");
            result.Add("eu", "84c5651582d69b1595215e84cc08fd7b439ea4513cacd6e244da437901f21977cbade6c881a168beb9c64d81bf85a26ca92c35215652cd82ce91ddb80bdd583b");
            result.Add("fa", "258eadb4f50efb514165376ec4136262e9785575cfe49141a45ddfde9fd2cf399cb6c4dba80fe81b9bd28c35e7fc3b3fcca9ef0d4f17b7e08cf2ec656e0228c7");
            result.Add("ff", "faef13858bf01d7e8b90c137f8d6392222d417cdf3250562b5b516097bfddb80c0e3c37ceff726ed6ba75682c5833a57bad8af565e7d0922fec7dee0f88ecc22");
            result.Add("fi", "2c20b287567219ec5cf6fc5ec726363431837c7007be50729dff575b33660c3481c141f751eae2ee6bb50a9bdbdd3a1e456e68b7de8ba92ef28164af13609903");
            result.Add("fr", "5d77b6fee354ddd876c9c1158d7c1eaff26d2c29816892231d692b28d7ede1cfd8d1bd9ec1fe172cc84bf3ad4aaba4a67e7e8f808776f657eb22aa7d78508075");
            result.Add("fy-NL", "a98ee6483a596c2cb7a2e1c6e648d06169c267be10a28b033e22b4db31beccbd820c8d7a98f9231004423a2b5fe2dd18b2be079e84cd1dffbb3c54cdfe07310f");
            result.Add("ga-IE", "9407e332c288dd1ce78c68cba3d310daffbfb76041472729bd51a951657075f041d6158ecb93a990a21f0204b92f155b410eda1d7703ea63ff7874c671ca7486");
            result.Add("gd", "0da92051a7e3cc41582a0a4e1432af10a042e926a332ac26000397953918ae29d1ddd26a3e7e3b4ce285e8ae2136055de704b6a364d0212554d968249d520e3b");
            result.Add("gl", "65a3064d41c2dddc329cd598eb37c1a9c54a1bc2fd80e8d2b17e7084c9c39fca1338ed8f7069ab4fb19528134ee7182492e8243aa2cb4143c7a13ebe1eb21473");
            result.Add("gn", "a28beff47fd7aad821e4012ca32d40bcf2493ea5f091a30cbd42e0886ee42456c5bd218b194449801f398b5081795ecf51a440a4a2cfa4ba034b2768476b4753");
            result.Add("gu-IN", "52b0d38423d7877e0437ded9f0d80320cebd020b5b8830713b86dde818176b9a750d7d6bd1a3471dc58f7acd52af7d78c0cb3d07e7644139e5d99766b32318a9");
            result.Add("he", "38071c4eb16abdfede985c7d256735a37af29dfbf53d76f885aeb0cbf9b5e9bb0dfa1aaf9f6d6d8bf2c91e44be724376b9459ec2d39c96d7cb5e36cb0555e356");
            result.Add("hi-IN", "3f1a38fa73cebb9ac82d0d81d913eb665d8cf6bd0d2827dd6e86dce941cce7d3aa1ec6061c7c5f45a37bd9defe081bdd797ddb22e2dcd75a8df399b602b4faab");
            result.Add("hr", "a22c20e9fafc2f1b0078055430c63ce8e353e06767f034a52d789f7e9d2c5617699aeeb404d9317a3a017f52185aaeacf0eedb0ba3eefb872dbdca50ded29b8f");
            result.Add("hsb", "6545adf2290bb7e73cb8681bb6e662b81552617d97300f33da78d0da35365c1ec6b77340d9e22caffed265d184a2ff0947c3f6791f669a3256f49a5efe6a2713");
            result.Add("hu", "14e1c92cf04ac602376a07876c6662746a5b77fd758718cbb1068922a77c6b9574f9d58b727754c825ce09353a9b8fb35bdce87f3abac27339ba88033043962d");
            result.Add("hy-AM", "5cad853311b11f2654115872a8f30d671b072144eccfa83c67a9f432608dab9337ba5ff99716f830100de2855f04b03689e6e45879cf48a67dc4b6b0f04acf27");
            result.Add("ia", "bc3b13ebf08c57ce043a854528507e45354c9e95069eec83727225f0017fa0f1ec14d9e732dbb1230811245a53be308feb498f3b0942b367367f0705479a625e");
            result.Add("id", "afb97ed548b19d2ad62aed19ab278247fbee37dec744741072decdc5e7acb2185443fb1bfc8bca4baf56100196f26e2d699b8f1f936d66aa3c9a3632d95dbb92");
            result.Add("is", "44e5565f54b5332f9a93050ecdaf5cae6773009a074015fef6eee39577751faa50b12766aac5d43bc34d3f7c574312daa1f0474b8154863848f052c259c70581");
            result.Add("it", "53ec99f6aa53e1151ffa2998a4621cbc43f7537083f86ed99231488f16fb8ef336e6078be38201ff97e2239f9ac7815df80d50a6ae61c7876d3b7d3745d509c2");
            result.Add("ja", "a60960678f133798713de872d7779e55ac9bf2e6575b272f930c9b334b8480e64d32ddc8913d76b1303c77f471fbce5099b0eab1f748564df876966640be4086");
            result.Add("ka", "e0241980ac154ba56679b0a92bcf639a837d0c3b047e6a1d88615f9cc7a750fe4a29a6b8a8b938803201d5873ae7699246ccce99b8b1993e73d9ea0c65356f7a");
            result.Add("kab", "500531fa174c1caf31424e81957e4093a32fd8b3318a0781dca4795c72b93c81968fbef206cc6cdf5edfc52d82bfd95f7ee15b89ee7c4be455542fc41dde0459");
            result.Add("kk", "f986d190928c40b3c4c7009b44a85e763c8b1af7235c1df62225b90eaf499c5ff2e9115d53c916c68324a5d4888f595872a0e8a8a488639b35af5529b65bfde7");
            result.Add("km", "02e60c050868de745665e28555585bed5927f56906937cf95c48995d2e02264f68dd7fd0511adcb03427be93c86300e94ea752aeedd4df96dfea71cdb076b637");
            result.Add("kn", "c33a54049c61bc30bd3eee0798aee02d4feb1df1b9d949c59e8b7a35766049f302a95903e82264907124901ddaa8ed8e5e65a01fa00a12a2f85f40061b26be06");
            result.Add("ko", "20c21bc793ab5207adc0dbac494d949691393a497b47f4e24dfc40ab50b1570c96356e8239bc10d1647a68113e8d530d0c8244627f8253da2cbb1762fc3fb3a3");
            result.Add("lij", "b656dae0deb2805ebffb44a3ca18ee248cb77bf1798487c1a4a7a5f5c28926389d5b9abdd9d0c6e7db8b1639f499f677cb88cd303f487acdd7fa2e4bf4b5b42e");
            result.Add("lt", "16ab8d25d813c40676c3d63c84a2cc01772b12c1221887a83154f0a190dbef263d9b54abbef7d6e75b87c52e73c26e8594eabc05d3c652c51f33e0a6a9bf8321");
            result.Add("lv", "6ed5eaee0a30b1b9082a412d1130add93643cb6723c3a43bf7e41db0f6ec9c243156007d3b3afe3e8ab4389ee8dbea71f683f0cf3af459059a27fce07b3cd14c");
            result.Add("mk", "4a96cb9ba03f8334a11a39704c200d032cf97995fc68031ad8781cf684640f6ef2a4c27ceec2a84598a7e8249e3accdd220ae4c8ceaa79d8b5ec2a65226ddb0f");
            result.Add("mr", "d4ca012c4987dbedeede9d60563a669610886147f8b7196b2296d00f224674325494b52b355982ae998e0576af87ff2c5586d2aaa1b2680c40c2cfab1a8038e9");
            result.Add("ms", "00a026b6619326c801e1ccb511ff094cce39e7fe30d66fe271fd80ac110745fd68e1d448995a77ed8262dabe9461b3f18af1eaf229c136bb780877956b17a3c2");
            result.Add("my", "a5c8ba0b375dcd20f7d27c560ba1a55f7c9c3925accc899eb27a40205ce0ff3e5f5e39b012a5019aaa63e8d35aff0f21f37250bc4441d3121d4c86e126c8108f");
            result.Add("nb-NO", "816e6d344485419096628334d6502d71246fbd18a54fb3cb77e8a69bd94eaec375e52db49ae2e2cd0e9908ac33fb2db13389e1c504972bb8efce635dd85cb029");
            result.Add("ne-NP", "3c6be353f16e6552c660dc66bb4fcf925a593816b65fe63a134af6389bd3f25f0b318d62c9f9d9129dbc76562a514d318f188b3cc0ed037fb9fcee77810e783a");
            result.Add("nl", "22425e766116f64b28f74fcaab5284cd4c906a50d92b08f1d606184457622af1f470057f097287a56a73caca21b91f088ba69c3db194ef139c8730ce705148ee");
            result.Add("nn-NO", "30b4436ee4eca9ba8b3d6b7085faf814cb7eb84bdb4c86d4b818d6df47ef4bdad1b56536195f3a2a4b8b935f818aacfb88afd50e9c47103e7d06bd927ca946f0");
            result.Add("oc", "4a4b968ec85cfa378c23b63c01d73641d3ff91ce3e9ae02eb0d5737c8745984491a40c391da7122ca6358e01dbacf9d874c69e5851b081f0f4a301aced549410");
            result.Add("pa-IN", "80f2b6414136b54e2f7be42be476bbf340ddc110435e342af3a1fa02c80be4da34ef7e91313580ddc578b4da6bb5f374cec5f440b0a2f6f202648bc4ce9a87d8");
            result.Add("pl", "334b4c986b7d4f548c053d1b243188524d8b7b7cc0314d645aac7d35e1329dd9c4f6ecb2577129ff1d5e22c96426ffd1b5425a9e5d5f374c9dc9122028c8ec12");
            result.Add("pt-BR", "55fb2f878e2b7c3188f8aebfbf8a06dea7d49ffb1ff5ee9be3fcba0fac185dbbda0a4c014bdc63182560a78be5d77d9561247ddb0328149b200b46fdf85849a2");
            result.Add("pt-PT", "641d2f7cccdd91612804898853c35034e061a4053ab34fb557f54130369f7840c410f968d716c2ece0f1deb27e4e5ffff8abb318616776a2b71dd4c8b21c639d");
            result.Add("rm", "adbae477da2d48808271835871b292e0580a738b4c831fac65d2cb17ebdd9021eb08178e4cf1f887995b9ea62d445d31c9ba52b1719e80ecc53588653e535acc");
            result.Add("ro", "e3105c6213938de3dcf623558e02fa8919888c192a25af1163737fbcfd1fd1ee37b7c8ac9408d443541f7c7176e5ee8d43ace0566ff19944ccda8cd73bba182e");
            result.Add("ru", "a844708295b702153bb06dac9419278659e9f55d7774bf18b39bceb1787be51d47fe0b14e4d6b1c78679682d67dba87310e9c5f806aa6efe87f273f13bd32fa9");
            result.Add("si", "2c1b1727ae0b4987d50fa7b59d5b90c79ae7e3f68fbc6fa86747713c45fa431c9a978f62398db0604980bcb8cb31cb085e70eaf00f7c461d7a817f3195c35fbe");
            result.Add("sk", "a07cd4973b1779d41a417d3137059d7e484fa2d13bef5a9d0b3d1a29718010765225e51fb3caf024f0ad7ae223728883481cb60a995846c8af7a7fe935adf80c");
            result.Add("sl", "dd0f4e2a11537c9dd1a1c96925aa14680cf1b95fe36081dac0a7219f683f8f9d5ca0175779f3e7b0710f59e61137d777a69f34e2fa1d5230d617ce2223da11e0");
            result.Add("son", "3304d77242d6382a16b08e0e624c4a2a2d085552f5bfa17b32764313bf2fc936ed107891656d87cced30b3b6cb05fae482c30cc209c7ccc0db4a19cd4928fc65");
            result.Add("sq", "284c58f1127854327de1364c77f12038af3451f74280b198668417172881349240be4499a7da81847ec0f4805a7a555abe0fa25a402a37ae8ef49137164d391c");
            result.Add("sr", "bc11ec700a6d47023cd4f6b04ea588eaa55408534e5e35d557f44c7793c2f7af2db1ecc293f7056cbc705ad4b8aced9981f8899f4d111a16f32238a47ec4dd12");
            result.Add("sv-SE", "f25f13a7c2088c410d404dbf2c4230e620e97cd336c58fc5028ae25e16ce3b0a4fd671969e1c47f9789b4a109fb9d5314f8d1c4016d065cc40218490e3859a8e");
            result.Add("ta", "6f3a94c74a19c032c6aac0b8a1cc65298be374658c593161685ca969518b65b9c2979d42e2f0cf05a67b6fd4ec78ce9b4599b9cbab8fa06e2fc5cfca7980b71f");
            result.Add("te", "6859b320c4516ec6500caaef1abbf17fa977b30929481a929b23afdde7fe3e95491ca8b8f7e4c2e4480e1419b4ce3989c35eec3e0593cf73dcdd94c0f8ec3e25");
            result.Add("th", "5490a2d3d494110a88f835cb455cb927a404bdaa6fac570ac74fb1af4fc00db8658d4f676a2e3d01ce9125d2d07fc1c98f3f265995aceb2e26f923c2ff8fb328");
            result.Add("tl", "ffb3e268cc86e937cd3a42b5e2d493de1bb7e536b544d9e32b66da974a9a943d3e9d9b68779828f500dfff1cf198d13414cafc9299a8d846cd5ee4b6240f04c7");
            result.Add("tr", "42936dfe2356ea0d23107245ddc02fefd758735254d973c3d25aa6e2aad36ac7f3978a879ebf545a6df094e39d0489910d1e6bdf7ff63cf9d152b8f2cc77f2ea");
            result.Add("trs", "e040d93ee8d803c6e1bfa0c67f4ef6bd8bed1097d8bf02c3fc48ebd4bd76d6d2e2b1a5eed5721a98b279d686672601458820c72edaadcbb4ab366125f4b78693");
            result.Add("uk", "5916c06b1dc6d66029c12c277db94e1498090a5fab95b2fe5d97343659aa1247812120f9651a79a170c63d282bb4545203e39ec0c9c8765e3255d43ae87eb0ad");
            result.Add("ur", "abafef6b9202d4457f1ba27b48aa3888f3687d0a999a4a963337d4f4be817c6ca2364000c72e5dd25602da09f29ca8c58187d099d36cf73a7ae2a814c8c4e436");
            result.Add("uz", "af13c25d9101a78fbdb4b0e8877ef547d3d0a156f934ab15d26c00f3df7411bfde8e0ed64b67b1b76bd144112dbc6f3319b28f8f3a11b3b1dc4fbcc272a733ce");
            result.Add("vi", "42f611f9aa8df19e1383a62f419135d3a7fa01f1148e31aff93cac95b5ba1aef9b72a7fa311ea1a87a2fa179cbf2b07a4762a4a7c2b008d70a7bdd886d4b2de2");
            result.Add("xh", "5f721a180e6eb36d62bda8b0864bc1bfb8e3e172cabb6e58c2619cdbcb148b048a9fb9842ec8d5399f3b0f03eab33f18bb2bd71974ddb107a5069b21814d9672");
            result.Add("zh-CN", "8862d521ebf39cc948d00840e7dfa2fb6522e8e01bdefea5e6e2cbca20006251aed4a8a3c5040426a8e3fbe998b195c46bbc24ae8e8f586d0a06dfb69fccdd64");
            result.Add("zh-TW", "b10c95b3f5fca278d28aedc105f549e7e2e89447243321bc968b7cf27a2993037850cf306fa8ecf0db550dc5c8bd7c6be3881f7b46f699cf0ab5e0edc645f9c8");

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
