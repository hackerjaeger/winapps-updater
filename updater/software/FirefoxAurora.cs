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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "82.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/82.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "f9f69ccdfc7988d871f7a3ca28166481e0f177c45aeb7b1ca7996f79e99cd13e0d5621b36afe17aa62298d4715e4591a87a5f6f1b1318e0d27791f82913a90bb");
            result.Add("af", "90a82e5182e1acae2f56a82b96e62f52dc51828413ec35d9a198be74e06ea825f0487a8b3ccf91bd1c04aa13d35e1992e25a656febd979723b94a9c4797b8da2");
            result.Add("an", "f6cb6d26f0f7e8c74b774ac3f9c0ea0433718129ced7709b6d4b6957ad622c142573cb727deed4605ba99d1bca9092cbd4bd5eccfcc355d4444491a4190691ba");
            result.Add("ar", "9078f266f8ed66565db7c32bc80acdfa727a49a15877d5b85b4641d23aba9d731c741f628aa0786965651f908070d26f5c4a316e47ad560eed1c8e1d3ea3912b");
            result.Add("ast", "c1b25fb0b7ef68a5421cc88e90ef6de9478ae231532daf4ceb70932c12f7e9a8314c8e399e0833ef442d22bd951b39940d73fb549d5e4cff45266383c9f476a7");
            result.Add("az", "9da2ad1c4226cbbd34197acb90aa199dbe75ca46a9d1a69a31729b7de6fc26825d7d0ea9aa96ab00dd4e6d573ec4629dd101919d890756b15e51577eb654b921");
            result.Add("be", "395ce88112ce7bb6d35e0a28ee70ad166d6f580b62881f996b1c308cb2adeb11bad8ccb40f6340036cc9d17a2834846050d71dce0e9e52b58c959e2f64d0796d");
            result.Add("bg", "d385a66845f80740d1253f0c090f58da5902ea62dd7ad8506e46f8923a29ff480faf8cb80b6906f273f7109034e1aab6f50806b4f7daa9e572728903ac09c3c3");
            result.Add("bn", "fac07062d05193357e5c203c4a25086e0e74be3035cd49614f071eb4f54368e4b6f03837e6a925a6d9b881eb58a2aaac0cebafe021d94a695d11745f0ce94b75");
            result.Add("br", "6329c6ef07a410a1dcb2a233c47ceb761915f11369f151dc719e07f8c6e5d2a73583b91263a92f2235fa1b2d0063fe2f7f7a14a5c8ee551caa1b66359206c5e5");
            result.Add("bs", "cac9b05fefd9ebc8db4441a4fa12b8993df93bb64a1e09df3217a78054790065a1954d6e769edc1902c4de2988691d800a6b84f08e1c0b52ad9c308114275262");
            result.Add("ca", "be9da79091da8cc1e0b1b42381426aadaa7ad7313f62832d9c93c7206acabe435b5f88d2570f9d6bf076ea31d0e4c59e8cfba841042f2c24ba66f66af570b3dc");
            result.Add("cak", "c091ea155f11cd59835bda3540cde29d11658a294a4828d53ab4d1d65d3a063699578b1f390159b699a0992eb7828d98a48fce566ca93062278d53ef0efaa5c6");
            result.Add("cs", "cf3f97497e5c5e46aea3a1672f7e3aff5bc511a57dc87c2f086015843f70bab1ef466da3414b36a3ef8ac49aef4f8c455725d9cdb4d3f26f5ff82e19357f7afe");
            result.Add("cy", "29b93cb8955315f705fff0a10d4d7e6c4df3361f7a885dd880b448e2b9845271e5cc0cd5093cde54f8157ad4bbbdf2635a67c209efc911891bbdcda661ce3a21");
            result.Add("da", "eb34699d5f007bdd536ae0d12bc25a8e16f6cd34669cbed436e7027fe30b0a3c5deaee2203e06d680b01cbea0e3454f4f8ccaa784b76f8dc0af66e5427ecc10a");
            result.Add("de", "715f53c758cc6985f1adb5c089b8bcca2f9808a925fa8febbebc22774a2dbb0c26a24eb04678bc67a3f33d1b9e44f1b720e97505c3666751346903a62431dffb");
            result.Add("dsb", "431e412bd0dadf13ccda77e7436e5e3db88298f6209a5b8c97d2b1000f78a05925d54aee955ea4040b300c0dde61ce962f4e65b1c03ef04f60aeeedbc935f933");
            result.Add("el", "beb0c1b5906ddb9c867883340c527ce2ca4536f90a2a9f4d266895de42b2a4b89df4f71d7e6421700dcb4149b6976a1443a5ca65f03bba19740cd28e2906ed25");
            result.Add("en-CA", "6c93ddf8bbcd211a48690ab05e669947efe3e7c148b8af2ad758610677ac0ae0a74bbbf0afd5b0aefe5adfa1fe7e5bb539ad17cf660ae34d030371d8ace03355");
            result.Add("en-GB", "a6416a72ce2a0e206a5b92f82e0f8ef5dee0a713fd9a878cc5f5b99e39531051a2173ed319864d0a6c698f19356e59a2a30b3e7e22b722ffb7b7044d06dd33f6");
            result.Add("en-US", "2e4c6aac3c0cbe9f13b2f33647d019d23e5233d2b27f43202126765939b6a913bfaa160bb94b8bc4f454938b5df67bf869147ed8dd186bec32431ed7c0337f32");
            result.Add("eo", "ec5d264eb9953c07eb6b7c9162be51a354c679c744e4a67512fe35bf2ecf1bd64244da2ef204dadf0f1a75302b148cabeafda7eb96ae1ea84b279e881977e282");
            result.Add("es-AR", "fb409583f12db7a70cef2176cfc277b2f7e111de2b3a706293b3e8f285fb6f805bfd7ac9605ca17aeadcc041d2b15974af8a8041d96441d9993296cc974a93da");
            result.Add("es-CL", "94b19a529a60d12d70698bc5ba71686c6b31e55ef6bac9931fdf37f11573f8e8460b0dba6186670f69f599cbfd87612416c12c6733c65ff77d410602ba9c6b7e");
            result.Add("es-ES", "316903878cad2ece27e22eba1177a08a193f7068978569e6fe12c72c2687734ddd160d35b73f4b3136be9888ff0858e9cf85062fb94d1b7e90d5f7dd651a3711");
            result.Add("es-MX", "a503969fde7bd5614f4e86e849e5cc9f1b7a2f85bcb156b9d46591d263a4a372cc0947ab324978597b04afa47e69abbc405ec7c65c1bc35ee1a69397d19ad256");
            result.Add("et", "253f94544d653d3cb9ef973bb5d00f886465ec6618842ccc3dbd8e537085b759723df3f00c44551d226f077b2305e56ca80e659f9ec609fe660a9c6340a5b660");
            result.Add("eu", "15e2995a4d6de3fcddac3124a5f856a2580a4002b7ebe4322d9e9d07d1a1b95faf7a05001f68ef12e73356b0ccd72656ab62c15c56d8f4a28122d6aee19ebdb1");
            result.Add("fa", "fde6ed96b3999b53810b8f22ff3a5cebe700256aec998e7f7657b9e816fcc8c68ae265a40babad165d5c3395223bf955efbccb7521b2e0999112d9a3c33f13c6");
            result.Add("ff", "b7f78f73716405dfaa855939766f224840cc9f2edcdbf280d23ecbac34bcf8224c7b70e58a6b7cbe1b7904b1e272cc4db9ec2a6aef39f8efc863ecb18124aa33");
            result.Add("fi", "0e2f6f16fa259b5b79eb14f5a0ea9bf42d60df4e9897f11c9d1b2f690550b918767e1f0cdd66fa6f13228f157d3c18b753e7f6bb4766d8d63b8838ce2548f82e");
            result.Add("fr", "47131a5f11d677229afcf5b728d83333948cb9b0797386fd411b0afb1892d6f14580184ad4db4ca9dd4c2e9a0308d575170e8fc5182c72521c0f9ce589ebb92e");
            result.Add("fy-NL", "dccc678a33127cf55a00b2b3c08b852e9c355fe8a3ae4cc49970a849c94aab8c85802f7dee892e85a4c433c0dd038bed7dbd5fe1e0a11e876c1d1da9d2f53a57");
            result.Add("ga-IE", "7c07378ce3904e97f1d58af7397b226832ee39ccd4528cc15446794f45a217e1c8b3cef6ab5b3b13862c2669fbe73f6e61b48cc40f7f6cfd749b1032534198c0");
            result.Add("gd", "9c14af60da0be0a300be296e337a411ae646194c81e8e4a51a07e810f13846ef904777918ad8728da025b3be200032176a1aa593fa5525955a608822c407424f");
            result.Add("gl", "f7a997b6465f52bffd5125e0355627a8edafd0f0e7abca7d31e37699717759ea418eb16eb04a1c98b9bbb834683e9386e02777c22a8e7b4112acc86b8c060ebb");
            result.Add("gn", "a75fb7a39fdf1db7d9a8863ca285765b4e09083dfcf9b5176fcf03f7002ebd2ce16c4de9f3c782bc45e35721cfdbe64474a5e65eeb3afe117974cba17b3739ff");
            result.Add("gu-IN", "bdcbaf7ac20f843602f65bbb2667b032757c87ca7b2c81b152ed0abc5a007e1f0eb5dd31717e73597136c375a9d08f660d5d70d9a7766b0e0472f737fd490345");
            result.Add("he", "9d70b361b3db34bd6c796de43df744be0cc1ed751f66f310c4e024315304e0ced85cba2b80f9c013797e99010365ee9685f28d74e05b37384686954f7e16aa10");
            result.Add("hi-IN", "7e71a25c7889fd9398642121e41a8909d1395d1ef67bc73529e4228d6a495d9e76684c59b4cf8ee0918416581489568e5bc0723ca4c0545622743e0639acff34");
            result.Add("hr", "97355db7f722d7155197846844c5bd7fbe645b1ccd7ce6e25c520a86c76dbf90c0105dfa955ae7d6089c72e5cb1930e01c36e5698b3085cc5395afcfb2095b98");
            result.Add("hsb", "21009e16b032f07d30aa9e2004d695e5a37316fd7fed6546cae81aaf9516bcd17bb15e6461d3dee7de18318c422a470aa9a4964f70561b2aafaeaf13265a8b12");
            result.Add("hu", "545391e6e43c176a293faeacc4f1e72504f4e37eecbf020714d6cdb1b21c8a4f69ccf6c3d37765cc4f811afaab5226c0d76c7b7f77ef09779f65ef719dfe7ccf");
            result.Add("hy-AM", "7af4ef46d046ba44a19c82b044f394171da1ff2d4ab653e4a6ab9a582d9b2cf0f1797701d60f8ce9e94caf79b58d0a4cb6f970fd6f88d9348505f1eee1c6e811");
            result.Add("ia", "398a04be21419fc15092fd86b14f2bad4658fb1150bb5ff88b9071876e1b5519315ad4f92401f57b161ad5a050ed4b2e185867bdad354ad130e9d60b931183b8");
            result.Add("id", "9fb83ebb8b23c28d69b8687e54c67a590be6b35f81268988cd6b0dc8f36d0335ed25ca1283dbbe79b92134a72dc0b58ae8f9d3e0d3b4f9db1447ed8ba8781c2b");
            result.Add("is", "93e6142bfe952b91880553f6203afb672352f012757e12592eaa4c16d94d51d035085b78e4ea6bc50ec587bf46f79cbac72a17e54745a23ad39c6bc980c98ed4");
            result.Add("it", "3869f42e1bd1c74273464cae9e826da5521f8b7fdc09e375769eededa57761858e2a1f075233367be2021a92c55492f52b285a502e74fa43c45b79baa0c2e2aa");
            result.Add("ja", "2bdb2869b45e8d39d7867ebb8e1ecf07a884fbecec9e4caaae20db9b35d43422ae108516592c58dadefc2a332ef63ee819aca74376f215e45fb5d8c9cb6d0fa9");
            result.Add("ka", "af3ccc5bf466fae817ca418c7d56e0db065f3e4a2669b292c5fe38a5e3c376b8d1db272ca3219c7dfa1ce4cb5722eec408d4d1c28058cc2dfb8b652ad579c75a");
            result.Add("kab", "d72010e149f326938aec8ae8f3eb2564fcb98be4571c2852f572670b1619a0daf1a96edf10ae41c57a554a793dcd096da19e04bbf28ba6c72b4cf854c3558c53");
            result.Add("kk", "bcbc2ec4844c5fb8e18296294e4ad5704a4391885bb3f8ba493a5d98742979c64e674ca03f20a336429cf8db7df20818ef37e1b393d1c3683f5ac344240c0d1f");
            result.Add("km", "a9681c879fe02eb3943a7aab1c7fe1f21b446e7cf91717190ce96a9e2cf1bb0b7fda69934ac326707cdad44c03a9e85bf5d719da94a6696dee6b797e70124d75");
            result.Add("kn", "2bfb477325131c7bc367d72ef1df9de2c987f672f956b23e1cade8adf5d1d05a6a465f50a73eca39670d2352c082422444c1e4ab3f6885c121faa8fc61fdeca3");
            result.Add("ko", "3ad588617932f71d3710d6907a1f82a756b464023f0f9eaafa648f5b701e4a64fae9ffe2727a6a640bcb9be920fee46fcc239e103c0a023853e6d3e13217b460");
            result.Add("lij", "d57fda34dffbee981efc4ae53fecaaf34dff699b95e8ca53b1a783898d865f0831f93e1dd144770c6d22e01fff1235f95365b0e3c8b607ad75f4a13e18c6201e");
            result.Add("lt", "e868c8f3f5be9fb77454adf0faefc61febacb91f124f5fc5810257de14c099f08a09cb8756fdb7ccbd94a66c29b6d38c0eb443a848a92c204408e0cca2215afe");
            result.Add("lv", "7a1e0d9236177537cfed7938d5afcd6a149d4c444710920196cac3509e1446d9cbbe927e9cc6f1101d69f700563c035c29251275a3b270ef297c39adac7c9b2d");
            result.Add("mk", "13af85bd7d9a3a819fe05e4b9cfd592ba65829261730d542037178edb8b30d05d9e965979b0e5a5f848be9368dd401277984f1f4bb8f75219232e13bfd4af278");
            result.Add("mr", "36077466c91b4eeed7d753bd8c3df175e58d355bdd8cd3851fe19486a35b640211774f4e3ab8a93d59d3f8e785deae4d58d3f16b9a7a51cb3e0dfabf4b0b0f0c");
            result.Add("ms", "d1117b04a0e099c2c9459ac854bfa63271e5ff7c5ae3d39a3a4f0bad6540fc9c07c03242173203a9202d7da9673180c74bbea09723770a1c0ea4fc16fab2d93e");
            result.Add("my", "59def586f984cd9909aae95a2fabdc3a7d7b2301b2a8f2a9265e59dc503eba898f72126a8241b546e2bd8e421a35d26aa9653cf83f09d08113d51430d3532dd1");
            result.Add("nb-NO", "638625a0e09ee74970dc5d095edcae9abfd72c500cae77d3557ebfbd73a896fa844215bdae6d2a6daa837bce6664d58d7b6b184a5576b7d308da0c32ee428b32");
            result.Add("ne-NP", "7bcd3af832c93d9c8690b28a5173d33eea6c12f386fcd3317a03abb618a47eba892097988a1b007f435a0b01db6b9b3ef616516e1adb68b760bfee47057aa336");
            result.Add("nl", "4f182ad738cd2e89143589f142f6f69e051293682ac1f55823ee330eb67521eda81a4b2f63c8146bf95635037663ab9297477470cc3422d39fe4708971c04825");
            result.Add("nn-NO", "ce9ab2a0c6b93851fbcf71a577161eaa013fd7a3f82f80da22279f769e53d33de5b94dc99556eb001b9cdf3ccc64ab8c94b76051e950f657f009966c956ba5bd");
            result.Add("oc", "d9f35533c69fe786c3148fadb5c9d39ba128f932b8046fdcccc47ceebe5435ccd611b6e7f698ee1401a2bca2b6de6a2508a29035e7751bfaa38e77a27bfb5b16");
            result.Add("pa-IN", "65bbe925abd814e83375f5726819e301da57a1ad5aff76ac1d5820cbe9402ba1016e0bc2d54eebda526855c47694053b51fbb49e3988df981fb41de021a3182b");
            result.Add("pl", "2ec205535d6a091c1417532acea0704b03105d28fea781beb2894d12c13e6292f5a4ab1a0895bd21b8aef3632280a72a3f6f3b447aa20f9c0668e8874202d1e8");
            result.Add("pt-BR", "9e686893b74dcf0b2c77ca33aa21437dcdb107428e07f1168d7d1eafe2656ff224b82c008971c2e3a2728deab492ebdc1251664379924901447394d72c9671ac");
            result.Add("pt-PT", "861baa61faf686fcbf75a8f2f4f5af8043f3b5995576e34ae7ddb3139c36d23b2b584f07c58ef6acf95f60cab3c4d351b28af7adc176e1a5a07a3e6d0b4f9f8e");
            result.Add("rm", "edcaddc2cb3db10f938c2379ad5f25a8b1b00a22d3b1f5ffecd0678dc23a7bb508cdf9f2605408e051a233f4de5c3f2e1cab47a8e5a85c3089346b3b83fd3a3a");
            result.Add("ro", "af7cb78cc00256f88202383142b3586728efd03190c0ad16e32f86f1531a5eaa62cc16f9297b4a4874f2a032c8a94461fa66ebaf7e5996f577962f6106c594c4");
            result.Add("ru", "219b8ed2d6cb25925c3f92733f3739f80f0d2eabed7dd20572588875fa551c63adb4cc4cfa4f3fcccd777f9ea87e741d57b1c4b5753de1b5c6c2d0224fd3ddde");
            result.Add("si", "968aa3b2a5a8ac9e5abbf3af4fa8d2373a838817bcc0e45c10951a911028e09138e46dd48635656ab27694f6b268f9381b85ea0a57d4cf625118cdfa03cf7dff");
            result.Add("sk", "5f836e4d96612ad8c6b75627fdce183e7c8f51652b4ba5cd6ac19b2f12ab55a06c38c22c08b6fe88eb6074fde6a8933d869b4aabd8a875d7dd9007e9a15d6cee");
            result.Add("sl", "0414e00c0e8b2caedd1aadeb1c9e154fbb1238167f12f203974723a13d86158733f0693fb9b17bf80bfefc20c7db08c49de1a20b8c9c64804db75b9d4d540ec8");
            result.Add("son", "4f3d651bf59341c35129837df42b459630ef3edc418f9b7725e19dad3c6a4bf5978351d6f3852fd84b05536cb7a0ea5d136355ead49d1b056d457fa5a082e95b");
            result.Add("sq", "34178bfbfca47d89571f54cfb5dce5f2a3a65af8eca808b8c97e37b65c56d714f71f23b358cae390891a94f74db79c6d1578f4d79a8c22f33a00b0323e5ae4f8");
            result.Add("sr", "e37d45412fd65fbc2ef4aea15d04ddf8ee660cf43120d8f435011ebe13c501ddd0583a51367171a9bb851035dc3c402f8e238840150ad2ba2d63a0b29392b050");
            result.Add("sv-SE", "58ad1cfef0919fcbc4de559060018f884ff42293ba1c2ec8b707a6d0f86be2ea8ac88f9c8dad96bcbc663e93821a721fd3af1e86923f9e65f6ffb4f84fac0dff");
            result.Add("ta", "46f0acd653ebc88fd9e1e48a1d6b742aafeb5ca53a577e6da930fc9c2d08f69dee4d7ca73f720ce53ea899563cb20c41c5b0d715605586ba0fdde1cdb175cc3e");
            result.Add("te", "3b89d6bf662189d60c28fb265a4405d7df152cc765c25fcb591401b5cac28f4b93bb3b851fb32729a25f0e4d7a07bd4e57c84e007d7163e90d6faf7401865571");
            result.Add("th", "e1cdc65ca7ddba0796bcdbabb10b903658c244abed3498e1ed852ec4185fe9904e3712e6b0306d7736efdc69b45343cfca8a88c86f999ea4c7133eb765b0f520");
            result.Add("tl", "423939091adb5a232a4727a318ac341dbf78540cd2a8363085b0882aceec28bf2ae518e6625ea88a0a7b21d2adfbb6189798f179151f14609c96243bb92a870a");
            result.Add("tr", "eab068626aad7cf71e6486bdee21a858826e27bb07463c34208155370366a064261806bc1cf81d7f65c97fcb96789d8b5513bb2fad9352cb9bcdc81b410fc849");
            result.Add("trs", "29b1b60e664981e7469f1dc13e4468b850f6ce0ed1ff32c0300895485fd3aab5a2bfc29f8401d1fc1742e8950e6238c872687dea671960e3e3ce846bd37f4ec6");
            result.Add("uk", "8fd3a7d2b18be048ce41fc861d434e2c3f5591ecf24b00c5a6228b3feb3e7b2cb7d1a8d0f2e93dc4d4c5ee727149413315342b8af2c860618aa4db79a53d5b33");
            result.Add("ur", "68cc80039baed2f1517b01f953045a05c7ef6ddd6270b7b148cc3a46b0140d7cfea915aa9e8e9b6de882da8569eacb87d25efea1281692012c778622b4dad3ce");
            result.Add("uz", "37892708600e06acf924d3185b0da308273977d7633ba92a71c7f7bdfad55bdc9d716e055d325c66a672ff72133ef40ea792510a32d1e457bf2ff307e808140b");
            result.Add("vi", "15d584000d88c887457ff8e39ee379810e7fc012bef8a4901290ee9946c13be7a7c5b3a06c3d8b0f492e980bb86dcce2232ec4436e6a84b9fc8b57edbde3f6a4");
            result.Add("xh", "2b321ed087b005c07c16be91665223db5be37b83ca6f543857f8c64b78f7384c609cfcd7b0ce2d0ecb9600b591a2869790e1ccc88f3f9d870872d35565f36673");
            result.Add("zh-CN", "bbe1eceb20a719c26b491128fbfa83fe8b7e750a3ce59174dc4286ec0a44a801cf6901a943f3a5fc686a1e87a0d9ec882e517b4744bdab012c0537893258562d");
            result.Add("zh-TW", "66d1f36fc46f863f9d0f3cc54d01e8db29353d470dcb23b1c5b21b933bdd7c940e068487d147aca62166e3d0fafbbd3dd8574215ca72916a2fba86ec58a42e6a");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/82.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "ce4121b5aa8be2c9ea9748f43271cbbb6f0897b6d94532f6ce98d39dde859ae156c7bf616339d6409eab31a23903157f1587d6db9dcb31feb73ce75285d4f1c1");
            result.Add("af", "1d3566cc613757c0df597827beb77d31e3b752d2b3298f2140d4eeda16ed4608a931f4045308a22ff74cb8ea9b0ad1d4a560081d91c70a51716d6758bf6f5cd0");
            result.Add("an", "1be3291058efb363fe853cb073cb999ca04c9b1f2551d16ac4029dc20886b925e34038f19b7bd19ffd301995dccdce262c2ec61a8c205873df3d6fdb5be8d20c");
            result.Add("ar", "ac2fe89bd1cd66833a4b48725ae3dbc43db843fff5dcf9f7c7193b4b835b3605ef8e6a9fd2fc988894c371c6f6237e6532aedd9cbd6d64f26e508f83ee96a9ac");
            result.Add("ast", "a955ac9a41a119f059dd18d4ac98a1d933d0fb39432751bf22e9babf61c612178747f187b8ce77f8e87c9efcdb9c454fab70484af65dd44329af7e770942ad80");
            result.Add("az", "b973c076036adf05be9aea0ea57d3213fd5375f55c3d5c984053b4e56a7e4e7820faee947d8dcd7bc71a257ecb695f426987d1ec6a838d7c4d5308e1e71d6a1a");
            result.Add("be", "9f34901bc233b6ee12ad9b8067f9f5a40ef17bfa5b6158dd2a0c47e82d4887248a6ffe32f9f1358608d3082f79c05c2ac8c8047fdcf53ced381919cd0050e256");
            result.Add("bg", "e1c815b267e65c764fdba7b02f2148585d1c2d35d6733e052c861730b4d9e41c095474155b41823c80e70a7dbecf5404071d2059825b9d2d5840a0eb84f88553");
            result.Add("bn", "2450f12e16d99adf82ee85c1a627434ba69f311ffa4b969fb4268bdca0ea7b8a7989005b76e7edefff72bde3a35f78cc0a470d22b7ea02dd08613fd8402f70cb");
            result.Add("br", "85d5ac71eee90a4af191bd8d0755babd3d6bc454010c72975a25e1b0752ede4cdd614421359435bcf67bb590ee3e4a6934bc33cb938548aff58618473f29d5bd");
            result.Add("bs", "dcb3ca2311a275e1c1e4abb5263d1598c2a901e7b456c3b52e44373c6b91d424e6bdb76a9999c3e6e43e600f24584f3119d7db851ed5fae4b757d9ef66913db5");
            result.Add("ca", "668e7069c1446d3fa1ad2e7d73d2dd64d4fa0ec67040ca2591fab2c00577c88dc602b0cfd0159331b1b49f8e8b9dbc8394ebb0bc71aca1d5804ffdc8601db1cd");
            result.Add("cak", "da79d160fb802fee87f427a613443f970cee58bce5a251f91129b479c74bef9dbc97eb64a9f4de62ca416a19c798d3328d003b74376eb19f8a26091750968536");
            result.Add("cs", "7af706f82857ebe13bd01e6a82185147f16b0b84270778e84bde7e481c83d7ea7ae5cb2cd849483c3346b5917c1e4cdd1f95f52f5cdeba33f3b4fe3575cea9ea");
            result.Add("cy", "2709a860c7531d754474a5ecdcd47eb0545066d12342c9e53b64377ed2f42536bd86e73d0e75955bc5f6e34f756d7572c3deaffefca85c59d42647640e8322af");
            result.Add("da", "b7287b0b6d40716c95b4cd92f7f104366ee47800795dec5c3909955c3c0e2e784aa40d5db99a6dbf52e148625a288031f25a4f7dca64e9157131db52919bee14");
            result.Add("de", "b3b3cbaade07efd0f31d2989daa2309fb1f1e18caf104b2a7a29e8f8bcd2d75e46370f45121e169fe378bd62d0aed5aa2ac9783e1ad51f5d9e6660e448992b46");
            result.Add("dsb", "60e5dc6df79cb3bf8da766004629705cadbd147c325857e0b01a5fca9ab5c40cb7be9c85337a63304fcdcacc82823f3181b8a5279b80b3f6293c5ed063a15973");
            result.Add("el", "e004d5248c3b05510868a6955efe3b2ba7b9a5a75c741004022a54cf2b85ca8d59aa02142ec4ce67c1235ef708bfa6b70f45c27510171069bfcbae8df11360db");
            result.Add("en-CA", "ea818b1b400e89a1d3eaec25226fea879c703aa88384f875c27b5b5a4c63e7a04309769f0a63c999351a1442682e7505b43e683bd820c6fe62c9fe03329409a4");
            result.Add("en-GB", "04a3894b5049f8041132a6216b2c0c9cf519ae936d52e46ad25c931aec5da39885b88ff2ed8b0e5e5afe08bfdb425b30f7baf7fa87633f8de7f2b7636357aeff");
            result.Add("en-US", "8c6aef748ad05848d67513a422d197f73ef99f060f7fb9963e9662ba518d496e5205ac6ac71e76bea9d92735758ccfe0cbf9ac6f1bb599c3f09696ead4d3cfa1");
            result.Add("eo", "d2fa42bc6a06c42ef6ad84e3fc7d7a7554f6ef230daadbf3075cb755864768669f8723f7948e0210559db183b5a09066bf499456fb6e0115616f020a7ce7f825");
            result.Add("es-AR", "7b77124f44ec5d9eae3879878f639f66dd3209c0b1b8236d075a07703d15802371e132936afea89df904de7bf59f50d3c94d9a6eea0621facd7c15bfd0ac44bd");
            result.Add("es-CL", "5c022a60d268b32e21dbb93661c3c0cfe6a61c9b6bd66488428f81dc770a699b128079adce082bd78614b5673ebd037a4f5ad140dbc869dce68e0d20f47d9bed");
            result.Add("es-ES", "423bbabccd7d9655781a3b1a124d555dbc03ca6b913d51af7d7805ce5ea9cb8f92043ee927d3c08f1e0a01b587b58641ae25423bc5bb08979b2ae64d56500520");
            result.Add("es-MX", "80bb711c5e7461e63cd6fa1f2827a0a210ff86b59a6c7a301fdcf18588175e978704a4454bbf4ca8c83ded01492e7ffab5eccab29ad9498a2ec9f9c81d7f7b36");
            result.Add("et", "0c535b72c758fb8c2b37edf58f75a0342b694955b513429448f315249994d16c22a63f2983a86d72b5a4d40d499cdc191798b677b28544960b58651a86aacda5");
            result.Add("eu", "be97a3bb1414aee15e53de553b53c08aea9d8268f0148bf30b0661a7d917d52f80b3bc54f6a58d60e779ec55e173919cf1ad66cae5cd7c4e13ef3f9056f39cbb");
            result.Add("fa", "ec191c624927cf87d06385a9ddc645804f1b2962d410bf59d62a4993d371a549fe6c53272c26605303a9a5f83399aeda45189a5ae597a2b95e4c4e328e6a0673");
            result.Add("ff", "cddff2f71302c37089fc8f4e626300b54f218261acd34ac769b97c553149da208d065964575378395260a061f8b2a44f96c88420f6449381b5f25cb8cf937869");
            result.Add("fi", "1db02dd6d41c810d64609235e8083fe5a69be721253dc842fb6a374bdfd08e2527ffbffb06940a705fb6b44e5c9ad430c85e00b4d4d83f491785ff477c517cdc");
            result.Add("fr", "b5abb4cde54b9d2c2fdb9a914927f9cf8f62c5730bacb29a23e7bb20aac9ebfd150db988272d54c05693d21454cceb69f4d9162865de3a147e27e70cf68ac148");
            result.Add("fy-NL", "11ff9fdb0dcd49a8c49dbaa62f7210b25501c42538cf3a49e5c26f283a8f03b84f6aca581537bb188787ecb74d3722290657914f9ecbb4a731106b79932c7734");
            result.Add("ga-IE", "2c10ab459fdb8fb1d157f6a1abd546fd9ba7fa26f6ea5873c3cae9c62f35d6c8874efd01f8724bc1ada3f08ac1d0f15cec98a909d634a2ea4a1820dfed13091f");
            result.Add("gd", "b6d266099162afb8e6bca6b4a80befc4b573d0a8bc299465e68011b938efb8ceb7d99e3db24f81cb4a43a4f223a7a0215cc4c1ed0d0af3fd8149a71359c8acd9");
            result.Add("gl", "c0ef803962078314ef2b35635dc9e1c1aed5457cb09456377ac0984de0d46bf0b9b05077607b600f3025430fef5a8a0126dfd185b8cda700e192b66a08fb00ae");
            result.Add("gn", "84c5e303403584a81f41e4cf8f7f1e1c0394e24ba24d31fe592931e1878aafa28d31767cb17aa5f34ed84d5891fa016e5ee3034c945d51886e7b4c80fa3169c3");
            result.Add("gu-IN", "9f791f2a505783507b7197f9bcd95cc00ea7e639392bd4f848b029cf90222a619accc4b4a0e9a36f1f60eda9a4dca91414315faa09281d65c8347387cf4b70d7");
            result.Add("he", "2109d7802273fbdbfbeb0b9fd2a853d7731dcbffe5a956daf8fa191eef4b24c1f978645981efe4b63428051be3c290936996a6866d80d5ce582de301f0448532");
            result.Add("hi-IN", "9e40b9587075c86db4cb32605c9ca7a77bebeda05d957d8ba82a9932b98b5971f17b03d6ff1f3a2507c161fbe62c0f53b52c89959576e58b6afdfa1f7cee97ff");
            result.Add("hr", "3352d4c4a1f0c3d82812cc247c034558a5818259d6447397b977671558cb04f7a7c1b83d5e97da471bb01570d67ba66cfe8709c68bcaabd411dd7f72c9987957");
            result.Add("hsb", "7f7da7cce5063bfe842413dece17ba788d4bdeeac358f300e7f717fa16c832d14b9174718c2033c3dc6b8e3b322a94e066fdddafb3c8e953764f7009dfe1b2f7");
            result.Add("hu", "95a22c33334319502088d7df49afc9f4372596686a32b9c6c3dcaa1573fb2d834176a03fab9512929282cf57a2a85a8649501a80148d9e1f7b0398e7b5216e75");
            result.Add("hy-AM", "6ce52c8a575b2d4c73704f32c4ae771021c286426562a46bf06476cc84f5c99e09a840be678ebeea6e4b175b73c143ffa925f1fe359ef8a5035cbf33ba1379d4");
            result.Add("ia", "190ed660288d4e1c5fa32aa57cbf1441cc761885e641cb2a979444dadc661aa5d1e02913493190884dfa473519d53a9066eedc7a99269c3adf706115c068eb62");
            result.Add("id", "63d1dce88f7057b682428186b87bd8408e3ba24737e580688e12e6116fb60dc24fabfe6d808a65682c00aba25a08922ed4da946be4ac3ecca207ee28b16cd70b");
            result.Add("is", "bb2e629d13440021197dcee5bd34f920303ce7e26065e1a17de39bdc491c852b437009dafeaf8d77649f0f93218c3fd1f2aced4ddf4cfeb6740659eaf761e71d");
            result.Add("it", "149df9c24af51707ef01292cb537dd2ea9bdceef26157e69a59e459da5aa347e77fb6d88921112c7d4bcfff6b7dcb9c44d8f6e3cf720deece7af7d97aa04dd6d");
            result.Add("ja", "41c41e6b6a1c5fc9ed67fa5079cb51767d9f5761bbf3ef4044c91fbe3e59892addf9a0b7f8bf19b6e7afb748d2c69262dce80620889b45948f2fb4297099abd4");
            result.Add("ka", "928305195a6180d5d356675efb5b8760b025886831281aeccb51eaef24045bbb0f8d0c2ded230378ddb2538f53bef989c3cc096b15b45dac4061bfc22e89c985");
            result.Add("kab", "6a6f4616b15f7aa595d0d2514f662f70e923b791791d9321930a2ab1b7a6007c432976738461ec16eeaaee9cccf8f9211a13daad3fdd6da1628c2f1dd5f89b90");
            result.Add("kk", "34842b3e52b36e0e840a5a62b27193a7e47b6929f24fbcd87a557cf582aa5f56b885036922524ffe701f4443692127e1d0bd787c4fb60448f1536f668fccd5d5");
            result.Add("km", "4d788a3a8973af34337f99cdadfc150f3f6c57d2f9cac13c327afd42b42bcd4b4c2de6a4c0edcd0dd6b448ce83db96dda597a455e55d4a0ccf305a1d87c722dd");
            result.Add("kn", "282a2dd7c5e12ba985b587d6092cae04764104f69f37b331613bcaefbc9a240bab04ea04135a7b010337a9b894316dc3f3ec07b780f06ec67c8b068719daf422");
            result.Add("ko", "16af3571505eaa2bc0a59b3419b7298a2e8891462713f80db4c363eeee1486b666cf7dfeb83179057b77aa141be73ee10842192be9ed3648209ad982d74bd156");
            result.Add("lij", "0ebf59c336e57fb622bb33f9058bc7ef7b950c0629e48796e4621f9e038eb8790384cf85452bd665dc07048be37433891087421e65d0e0089d54b5274a10b891");
            result.Add("lt", "2a72affebfdf6949c1cf5d763f8e59055d29f955f3e65d1508a2f3607825215bffc006e0e382d13aba37145b7d71b9f32cae6287874b65de2140834c79f26d22");
            result.Add("lv", "21db4f9963c5f2ad0fd9b2b7845e357213779de78b6fa9fac70da88b3c2fb442433ecfdc5c4acc234ae611da6b1676194fd81155248621fc9723439853c3d2e8");
            result.Add("mk", "ca0818322c819cc7e1e794ab5644c12b324e47b8974f9aa90b7e8f44f9b589b6023f1f2c8f975515bacbcc0203df8b0083a7475e55185b6aa41a718b0e818465");
            result.Add("mr", "82c3575a7639cd57aa5148bfefffc7dc8277107e11d12e1f69436379e7a4f9cb5dae1d6b49b5eae49b022c254984b1a0197248c88c890f496aab78e8d1fc1a0f");
            result.Add("ms", "25f0f267ed14b1d5769114e63288680763951fa71edaeeb098746d315c48f572cce2c1cf03874cd50ff7720221fcd373e1855d0847f3ec1ed16a534f6418648b");
            result.Add("my", "d7cd62a21638daf7852f70689ed5621f3cc70ac1b71e47ba2ae792375f380e5dd207bcc4e0531845ffe46c5cc18c91dce37ddfc059d937f3e79248c25982eb8d");
            result.Add("nb-NO", "a0e4129e33edc4964b0f4e126bfef88af641d472fae87cd14cccaaa1e21d912f97a4527e4d72e25a50ab384736a8dfdc6917c05a839c000842d96964465f4ca1");
            result.Add("ne-NP", "9efc3036e74cc3b17df2941e9954b0458f1e0b9f13f707dd5c2c7433634de51deec0936c62d228a9f69608ace124c77cedd0cae53c96259677b51d21b537eb04");
            result.Add("nl", "1af8da236d7f729077617d979dba0c84b02fc15cdf6e342946c853bdf53ba255b679551a4334ffc0796e92ef111681f61e2b6b03a62907b4b63cc9b2956c0999");
            result.Add("nn-NO", "aafae9a915286a83982ff4137fb705f2ad66db9a6cf63addf2f2ca2e42b46df985281a8e517f298fa80f7af1bcd108e3a6e10ce78cf57150c4ef26e6c873f850");
            result.Add("oc", "5f50395ec6921b10629c43bf0d2464d7c4eac765e7360ee5959e4c76dd9645a667b6d6368f7a2f52fc7a095c26ca686d822e81352a6f4f1c50350e062614f4f7");
            result.Add("pa-IN", "932732b1afa4aa10addfd9504fbc1b7915c2437f82a956ad1b955641abea53b7eef3c723a9ceeac67e30c02a7a0cd156f4089cd9037cc0b9f8a1ff6b1e0ab7c6");
            result.Add("pl", "98658858bb65969517e3d4b9f894241ed9b65f624f6dab0eef99ff903276d0e64cd43d2f3304472a19af8facbe06ac1a1207f78fb2a0e9852f0fd3b0d071d8c0");
            result.Add("pt-BR", "55c9902e6916e607322dc5dedad49205baa9cacd28f917f0e4e7056dd073f5fdb905118e2042dd012f499360d92b13cf9ba896d2cb76fb6fac69839124b7247d");
            result.Add("pt-PT", "eff00fc9ac7ab03621ba9d48be2a50696db2aafa8e67c209ffdb451f0912a1e10f041ecffd658fc5a4d3fcfcfb7a99f87cc242f733ca8f56388d67b31858b35e");
            result.Add("rm", "2e79a6a1b5ec7f8e1e33ba44ba2b224b3b38142ac84fc69fe6725b4faea3152fc778671cdf87f2d9c26aa1a2068d66793e12b7cd3ddc0881b6462229bbd7bf03");
            result.Add("ro", "78af24efb757aca5ed7e1688c5a68265679b9dc4bc36d2512c4cb6688393e3861c846b838a57fdad8404000eaff2aa3759f2f37eb483f8673c4e529c0bfe68de");
            result.Add("ru", "3336db76b0b51b7993896eb317b43b8a87656f21278a5de13f3affb2e0e0a2199fa182f25b223c9b5005ee050cc6497f7e449cbafabc750010e0aed4a71b3ef0");
            result.Add("si", "d06e4ceff6cfd4acc4a42bdbdc4850f062dbdf57724c46d176511d2d260df189b1ca23b1d3f92e9d1f2694bac7305d1b98ac5e0bea58f0ab00197851bfdc03a0");
            result.Add("sk", "7d46cc24df9aad3c82c6f1dd257a280c4fac061843ffef407cc7ea04860ce805f246d17094e455be010a85a0f2d2aa95a5fc5ad6dbf99664f4af4d923b416092");
            result.Add("sl", "28960533c15abe0404a199bea92222c53ec79fbd61e70613bb35b7ef11b0c90a3715070b821805dc77eb7b1f9bb65b947a74c52f2df72c67c845a03b39cb3829");
            result.Add("son", "5787bb843bac2e115e479610f22c9a09fcca2faa1175483da9475a56095850998103ea269556c11f12613fd8bf8614f30b622b80ec0e0fd4f835966413015521");
            result.Add("sq", "4095e78c51a49c7030d8b91558c934d336283227f2ba6f04cf94d1378ad65156193644269da59a06cdf5084177ff800ac6b9751cff82205c5ec71e6d4a988988");
            result.Add("sr", "6f421f71f8a86bf58676c3af9837229cd8d1209db1103404bda676ba1f8668f11d4f83140f7f5f5ac00305cbde0367954ad0b0bfea7f3e96680a3b927fb55ed7");
            result.Add("sv-SE", "9ddbc374feb8f87212ba0aa827275ab9c01cf57bddafbf5638240189788b713a692dcfac6f0b4df49bf208c85e1c3bbcae58e1bf326a0854a3b65361a752cb99");
            result.Add("ta", "3416351b2299156b1ac8ae60c9eb059cd3d14aa601c818446331f9e5e013b28cf736b6f7ef6bf80816ceffecb70374f93afb9cbb43d5fbae3cf4facd5e9ad353");
            result.Add("te", "30c4b095c932c6f6712b55992478bfb76d367be3c70058b045616fcc0c314d132bfc48a3ea3693cae66151f5052f16deecba7998004b2997a28593be5288e3ae");
            result.Add("th", "df1bc372fbc614b0578a993fc0d3ac837b8fa406d408b387f5870ceb597505f7e43e5ee6b5aa35949101861348acd884608eb26ce5d6f71a69c586f2a7b6d8c7");
            result.Add("tl", "da4ed51a7c0374360a7dba6e54730fb4e747cf551aa5a7267b94a930a16c7682d4bab972c8a504fe94f1da70369985699a164a6c6557245241c1492102688650");
            result.Add("tr", "55674ed8b39598fa16f3f39c0aaf9cb71fbc9383839d6f433461087ee92ddd72d42d61a84ecec92f3fe9f8c199ec8bbb5dc85811de43cf004d4db3a96cbab0a9");
            result.Add("trs", "c33fa1406ad56fbb3419719ebe6d99abfcbb15a65fbecf3196de40f7e53e2c21ae1eba4edd5de6027437f8c406585edac8d3e46d78f71ab9b06f0f228eb71018");
            result.Add("uk", "4cd4532fb7bc40372d5c0148746c38055fdb08bbe7afec6c607015bf040bdecf1caecf2fb77981d9b28abfe91aa901451e8bcf57aae2becd065d933ac9fe2e17");
            result.Add("ur", "0dda12afee6bab85aee40424cf26d49a7ac79d93ca1bd11834786d6021e0399cd170c89be440895716747298981b9173478e2bfa46aa29ddff91240a11f53aba");
            result.Add("uz", "85c207c4a5655941bdb8d5af587615623e9b825f3501f8aa2f5898e5cbb7f65b2c6dac1862ae962cb63d670778129cc52d2fb1f037775499df5832e673e14eef");
            result.Add("vi", "9ecbad084e604ef88ac7b8210baaa142369d92876e4901f4b03a0948bffc0ed5d27facf40c83b093278ca4145b17bd63557b82e937a16c388536706552f61454");
            result.Add("xh", "b8ee8bb613929db296e7964e51865f4ceb039f2f7f802309121dd98a8e91e89caacadc80150cee0319749c660082d1b1022ead2b32adde055f960d4902ccc1a9");
            result.Add("zh-CN", "c150e8a7f387a7af0ca9894c042a3a72d0c8488d361bafb021e17f1dac0d1faa28cd69a8bc2622bdc8481fa9a7363183406fcc3546262db8deafbb2413540b68");
            result.Add("zh-TW", "703964c57ab497fa8459a85a807ffc9aa4e77baf0a68f243f8e55aa82bc51e0ee0c2598f31f3c7faa1e0a623fc7d12cd96cac355abe1298fd553ab42568f1254");

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
