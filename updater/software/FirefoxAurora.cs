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
        private const string currentVersion = "116.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "7e1ad9bf20077fe3a6a119751957f5c2d342098686b55e51ec8ea3fa4f592e6e158752d57aea7d03fa01b214e2a452b1e69994ea588e2acdea9c4d3866569b79" },
                { "af", "0557f9991540f5b9887eed04bb5c6fc4fe668cf5b6efc02027440c51b5535a71f01fbeed3c5826fddce2d9582d1dc06f36b107f1f62a3e3f54036b2f164ebb30" },
                { "an", "2fc8fa5a13990405da103672037c4378193d222397cdeae0d6efdbd21f204e82c9c132411d8801bfa82e28a8c6cfd5d43f75e715ce285662c20f30eb5a03f5a0" },
                { "ar", "27f752be8b508bc41166d6fea4c793751a7adf3d264b9f0e1dd7addfafdbf8fc2d7fc2adc670984d9c05204a18f45d55e12027c76d86b7a60d03a1fd9bf1f7e7" },
                { "ast", "2e94495ea24711f6d6603008b217b6fc20adb0c4bb9b1a7d3e52baa752f963d3a0f26cb5543337666ad0edfbcab38a0c8c04fe8d2ddaf1bfb0c5ff11bc7f0bac" },
                { "az", "bc19a54a228d4bdcb83fa6e687cfba56611fb70ef6da695a81a1a43def525cb9ba426b56bce38d0bbde078e03a9acbf65c3cf1f9ddf7b4fd3adb52b2f8dba6a4" },
                { "be", "c8fc8c8e1b8a41e529990fb273f1c776eb4245d0969428ba8d572f3c1e220721b3cb89cf40b4baf35d7e51700c8bcb44fc59ef534919e8ee8280bae15d212302" },
                { "bg", "16bdd427af5efe9e6aaffe44bc5c6594fc4c44d0bb7d7117e731132f33ea417bdba33c87786df0a26a8c13e59ae17bb17061534941d54861dc6ec4963d46aefb" },
                { "bn", "3eb5bc4ba51c1387ee85ebefa531ec3a3136f8a1a2ff8d62131ce8776bbf69077fcd1f96dfc5a3f4a6483335ee4faffd793d04992694d70d86eedfa47c2b1a9e" },
                { "br", "825e36a93fe7bcab8aab6b55e24562c43a013754b0b4f4c6466136bda786fed579ed97334f326f7c29303aecff034eba32050c10094b6db6f62782bc4b912d17" },
                { "bs", "1ee7f4957731e993099e8cff9991dc3c0b5777cdb38efd505fcceddf5d3bc52f17abb1c3d32bd7ce935bf4928af05afa89b703123b531cf0a727f6e8b1efc6df" },
                { "ca", "8251c3aa076fa499acd60883a01d1ffba47648c379d8bac48f485bbc3ba5d437f486e7437a2a0a3d3040e156b1d6ca8140eb0581dc72f2a8f83a55dae3c3fe63" },
                { "cak", "bed958c9075e57b0fc67dffb668fbadf5a9e6064593128478f1dc45814fc546890485ae18f2aa6b0f98cd3382c219f63ffd68cbc87cb313885f7953ee733d136" },
                { "cs", "19ebb305b2b766f280f5da0d20f5462b99f9cabdbdc0d0da027df5e78e2a9c75c0af26e44821f57bbf51840724d9669dd7e0608710fe4a0de340db308cdb0ce8" },
                { "cy", "aab270e7106b24e3fb98ed555806a61fa80b45d24521115d3a3f4ef9e22d5de5b300b119e382716efb256a71619621772c4625f53e1a34a4d1679c3d74880156" },
                { "da", "72b6fa79caec08ffefae0acb49e0c66c7f47fb7f2754b4cc22f9e4d64f86d56062ff793810f15b66950cf5bdd4074d0551843485e7d15ada238b084f4e20bd2e" },
                { "de", "fb6061bd684d0309d38eee93f06ddc9264111874d655a6e8132e6397ff0cf5fc5e8965b2eda587e8e3dbe4e7681f9d8abb3f451bf4e64430f51c70735225d8d6" },
                { "dsb", "9e84569208f7b530f0cf369d08a5af9c999a39447cbadafb0a7514394407d43d08214751d07d02d0f0a9da51f7337c7108f9c71f6414ff4d0fec46fac2f4959e" },
                { "el", "9a8d0953b396e41ec338125610487748b62a6fc6d85070daa8cb90ed134a86dadcbc099177d2087d7ec0fa6225a7206c5462323ac0bfff4d7820d2ce5a7e59ed" },
                { "en-CA", "1aee547399281bd43647ee39ce916f3adda87270769e060f783c2621b01fefed0b3492e4bc9bc9205fe5c759de94e198ba5109ea70c011405081ae0eca185c11" },
                { "en-GB", "edf7483a21214c1042b19aad01146267f95ed6650672d0bcdfb7c61563c31c41c0664de38bb6438afcf80a6ad7c4c5e0771a355a3f239514aa4e8f274e9c87fa" },
                { "en-US", "959f87fd8ce565d90450a2c8986846e9d5e0b1ece4fa83f4949a96cebf4418d50eb628946205a382da90d8f4dd0f90ce06fa44dc18c15127d4c509dc2e13bff3" },
                { "eo", "2abcb5ac85ab5def983da0a560e45da8c448b2a527f1f5d93920d71b6a50313edd9bca71698d29e36b0229dbaea7883dbfb0ebaa95babd7e1f78c59508e35cf2" },
                { "es-AR", "ffe42e44dc9dc9b981154b5cab5a80482971602059a253bfb368f2203616a3bcfeec3938406b771e2c5a63120e51bdb46b633c214460b068f7d9da383b11f6cf" },
                { "es-CL", "c752379ff0916fa89a16fed82978e26f25323c08b64b7c25b8588328a9ac697daee4adafe69f912dd40d535e22e784e12c22cb329ec3d9f66a2d4c9ba427be90" },
                { "es-ES", "4df28bf2a5c504ba7e12853e46ba5d67dcacc6abc9907c017507edee9596545a31e7ff5718b311bc8645d4dfd0ba656af65721c0e1259e94b4107ab354e4d9f3" },
                { "es-MX", "7e198ec70ebd69851f147bb7f090e3d68aa45739bd0dd2e17e1ef07682455d40b2919ec409b4916b3c63e6f35834795fc8cdad85b860b5c97cfc21e9cac4a82a" },
                { "et", "430866766a0857754375f60a158ec2ee01638d41b926eabb24f531824e8b2a1fd9d182e9e5e5815e834153a372927382613305f0ab2f78c064ed508efc60d770" },
                { "eu", "717ed2dd2925d769128a0fc58c3a3f3df0615eaffc19cb7b8c736f46a436fab7f756a3920ffdad49bf825c321efe10a03dded91c787a7a9cd30d71d0385a76aa" },
                { "fa", "1eacf0c5fc44628bf6692cd10453779ea161cf90afe1c26ef54cbe0e72a3341221b8461eb0a432ff5bcfd76fd1a9077e1a7c1f6434fbaa46e05c9b246f393bf3" },
                { "ff", "f2fcaa1d6b18957a3191e9937f18894a2058912b96c947b4f34596af3d37eda11002366a7baf36afe86292890292043315c86072d2a7c2a270c0ec2e7a16e86c" },
                { "fi", "b9e07796aeed8d9ca930226e8ba21b9015cdafcb0014c92fcaab7e5c248126c99f881cadb23e4204e5c4f379051b2e9fae5ffeba5c313d74571e1d279faab6bb" },
                { "fr", "24dced6524642d2a40959930808eeeaa36aa944d05282f240c9af523b2b7a681409e0444d617076a048894b22f37ef252151dc96fe839fadf8f8fd3940d6fd02" },
                { "fur", "abe90021826cc65ad1eac6456ebc8ba422262b0c050d10777d0e72837fa706790df0a7b96d20edd0e2cf892b70e4e32a935a6d3d7ea7bd881b70c0bf6c1933ca" },
                { "fy-NL", "b35655cd77748c09fea1815d70ea0239aa5da4b37e0feaa30aec3c8571b22480dd8482f30d4da17b9562d204a072206e5f70679f0e3bd1ef8239412e452ddf22" },
                { "ga-IE", "bc70d8cddfdcd13c94d7643184511f119e544c4dcf7a4d6ad04853aa9179f3eb993f5e70b9c10c2dc6ba97af36c94fb02f15cb6166f232f240e70171ee7bfaff" },
                { "gd", "75e83cf6a401d888bca3edd3c91bdd1dd9deb5ddb0b3f1fcc96a3881905cf452b60f2a38f22a5c2838b93aee46f246d53c27383c811c2a6c28a3b34775666929" },
                { "gl", "4f053391b980fd2a41e1a266e2c02e3e3246b9445adef6218fa62fd8e8c3523ec895819dca088e4146152ded795b14828e7cd94a5f31ff5cfafb78c0e79cd8be" },
                { "gn", "e730a7ae24c64def40ed2aee7cbfdaef10c7224d00423f8afb92e916b70c2e861d4274e861db5107455a4fda15d6baf20a77d91eb972e8570a517c409c5e980a" },
                { "gu-IN", "6e442ccb6e87de2f2f127a11345aafa249bdacfc70230af831bded36e671c4675da13fe51890b5a7ba7e105384a5e64f62a37ae1808aacdf5bf10d85447e5cea" },
                { "he", "f7b24ea78c1238a6755f129fe64be8ecf4f3076a668fc34249d191a0c982f9600d418ad53c73a346c3808fabc5ce09f21c060e0514b90cad1ef8bed4d6a67bbe" },
                { "hi-IN", "72c4b5f16a23dfb7603729babd658a6bbf78ecb87df11e88803fa442b0ddeb0da78bdb3ab5156d7ca54eb9de190412a10457f3619739c096d36e0a6180477709" },
                { "hr", "59d66d038610c4556b8a6b187b34cd92b7eacbe7e1b85070d8da52e975f436500d04e80eafb7abf9b4d902ae8ee90b20e86fc4c08468ea31d87e3fa2bd3fff63" },
                { "hsb", "ba34581f7f5961316b09eecabc5cf567b32ce360b1e794f79ff5a5c8169813ce4d0a52600dac70aa8b0524af960320beaf0bcee0b05d0870222200039c060eb0" },
                { "hu", "1661b23a6f8b5c323fb4133217cd74c4e927d995a8a49873a2a81209e36ff1223c1c59a7bd598c4d838375b83f71a67879ba52de6c103572c77530e2a65f4fd0" },
                { "hy-AM", "9ccb7411524cff82fd239126ec7bab77b80dc886317b5218cf2a082c3d52cab7cd96cf0fc7cf6bfbb5ac93edb4f0e1da8f6f8aad5581fc0224e2097536e73ff8" },
                { "ia", "729796ff4b2c62efdaec1b192420bc62914f80ce8c82aec8db19628bc2dda52ef3a71993e51855b2dd91083f32ced2349ee9b4855fba27cca1ff052a909b4fa7" },
                { "id", "b4c69de97e93bd1aef9350089489cd8d7976bb16c609c98c7460e59bf9dc4a78725bf80d75477c8d4d36f5c7559a83db47cf10752942abdfbca1c74a7e1725ce" },
                { "is", "c60dba521b8c5a9040b9f19b58aa22591804d5b10569f2e1218e95021b03546299fc4695af131776b3a2fae7de73fbff86b84b56f2b0cb0f4bd0a8c1c08749d3" },
                { "it", "93ad3a6c853064ff7523e6045bb23e535082b6cd0f3293d96e08da4786987c0cc11e87f5683a5c5aaade73e394a9c6d853b38f920bd5c144abad3bc8052444dd" },
                { "ja", "abc56df95694ec231d354dda59ec88b15eab01f48a2f9780e5f88c966196f6230bce4a5eb2491855ffae37ecf492b42c7efa2bcf989ab56bf92e43f0c18c3766" },
                { "ka", "88e34b7f347a89f4ea9845044c83809fa2497331ab7d96eb440c04f4c67baf6b84fc9d0916422ef9569b56f73667e651a5b894db19dfccfdd2d95cf0f5583ff2" },
                { "kab", "7d0009746de7282888bcd36c82df2f6847883a9f38f09bd7fa9bdcc375e18fcbb85c46594930ba2bb51d4350d2ecd6744dc475ed5e7d0a0ed2eaa10c40347e3e" },
                { "kk", "876c3c354a8ad1497ca57704c9c29b39df37b56807fc3b7c419f5b5525eba24a9853b8471ae535c78a858c750a7484c9d03c4b465a1473da5525a86d14a2393e" },
                { "km", "f5c51619140c91f905f230afa13234527fe4b0a43fc81e300f93dce4be36ae83b20cb34d6d0fb388e01abf03dd8ff8ef504e22bcd9f8ea52287b605953c8735c" },
                { "kn", "2004c79e54f21258b7aa6576e5e7cda54ab7734629517a749bd11cbd910402a694c1d01fea083b7ec3279b6ce7320ff41f3846e62d0efc55b59b400028ade987" },
                { "ko", "0f6b5c0e27d55cec7db0437ffce9bbe3e3e980e39e2470305e8397686a3a5a34d8ad9a17e337fdc1da759c88e6490281429a0ecf57a46f5d1e8f7a566fba75a9" },
                { "lij", "ae37bef6ec6f5ed50fe44d4ef95bcd3353d1c9b4a730317d8321b8eac32e88a69b321fbe06298a84ea2ceafba4ea555c46a5f5ea671759259fb62c6b8ce3c92f" },
                { "lt", "4607ec8ca85897bad180bb0c30074f02502c194227263ca7593017d490c580b2f4de9eacaa1c3d794e618302784996fa44cb8c2ba83e33ced654d9b68fa927f0" },
                { "lv", "7af0ceab17eda1cb52dfdbb12de342ca24bcf0bdcea6fc425d39be7dad33959494fb3add6585c50f7137e765195267d8dcd59c19e93a50a76d70a5e841bac5a7" },
                { "mk", "929214392fc1ee5db05ca337f98701a081bc75842af6067678e9ec244c5e320562c41c95bf1fa662e9617d1c57fe9ff3f390f92f54c1e89df4a21f1e31d66dd6" },
                { "mr", "fca349bc78736615c2bf94ab156a9d5ae81024343cfa29fe0facf6380df14455596aef3b2d2011f0c425264c5ead691c5c6e1c55e384203a36bd3a99cfd2ce68" },
                { "ms", "69f0a2fb352205cf76c2850666bdef071bf2273b6e263a0cc633657e0f26d6bbc2540475b899dbda9984cf4b5c0a19102ed22110856ddf20535dedbac73c43c6" },
                { "my", "778fb546db3fd9f5c8ff111db58f4e9bbcd0aca1a9d01a4e27dc7a91b3f21c8a8f6cfc73a780ec0beb3d95616318d4a85a4e1fb498da3bddf85a9a3b17c3114b" },
                { "nb-NO", "6ced3f890bd86ae12e4b443b30ad99974de02529b53b3e4e843600c45fa4d499e9b231b929d00b3b6815da4dfc06b364e2641d836c5db63767263f6d78ad4635" },
                { "ne-NP", "bccbd90b86264964b071ad34e3d7f3be8ac74f2b1827ac389dc62693e418a6ba3f95cc7a8dfe08bdc224c1f4355c6feffb78b69159420984407e3edebffc18d6" },
                { "nl", "a273c87f28f16bf77b9c21b54d1c6ca70e6db5797ba3b53892a760431622df113587390bbeefdd46d416af1680e6d5c1f8bd7db305c6d4ec7292a3f5e54f925e" },
                { "nn-NO", "5d14ef6ab5bd60280ba41519a091b6bfff5030d513717b8e74d8f099b8bae10401f721cfa3958c1bd7f162746140b770b4d5669aa3e0122bd117e201fe6d9dce" },
                { "oc", "d69c65dfa0da0b0696d9fa5561556191a88bd7453013f2fcba85f385f44d887e51c0cc6bc4e2b52dd1d26107bde1e75a57f2ed0291aefe6d33622021402d9629" },
                { "pa-IN", "3e719caed964a3e6b10256b67bbeca47394eae92412100fe9ff51da10fe6ce5d12e54c46fa1c3318851cbbb13cde898fd35c7fcf580e65b9ff1c6aaecb7e3ee7" },
                { "pl", "b33752161b708631cee2c6bad930b0cee0d5c59b6cc8829ffc845e8c683bf14f4765c86f5d5c80da8721d9e8d7891cd8c9ce4484e6ba400a84ba00366cc4804c" },
                { "pt-BR", "55771d0f302f99a291cf0717a4cbe19b6f00baa1c7e57bdf8dee942b2cfe6a4fce2789ba95b3e8e0c4aaa6c47a3af5de55b0829e8941c395c5997c3c16eb555f" },
                { "pt-PT", "e546c9519acd5ab74df98786cdc0c48b93e122775fa9544c8aa32ab23ecd0b894d1febe9f8ad06b6d9e0440fa0297424611ae44c5c2666caf920dd305b8f0f5c" },
                { "rm", "fd2db1c6b7ba1972588ac1e3a223da58186ba7cc6554c42cfd8dbed140e5dddde007fe0a136bad6a88c495acfe153bf174bdcba3b44c3fd2b6983acd98c43694" },
                { "ro", "78aba7344bf4594de42e32f9b9643fe8d75c482a086dce634e071efb076288b1c2663d30db35ba9faaeb65954f51ff4b57b92362e798f69baab39cde65dcbb78" },
                { "ru", "87fafc255e5a45cfa43c4fe57a60ad23468a2b439341cdc35b963fda5307862c97715c68d8466bd033b463e9ab6d82bdb8a7ceab1600e3b1cce145ef90b627e7" },
                { "sc", "5dee3b5c97783bd608e4c95aa134f86f8cd9495a5f45daf512d05fdcf1dd12ce21bb52d15bb7bc9683220659c2b8d51303e3ed414a3f97539608cce81fd52383" },
                { "sco", "74c6d1424dc3a85fbd6805a2bb0da7620986b3e24b834a6a0d6b40530ecb3a9dd9a8057054da98c2a64a11c6e72127b02d364e48cc25f9bf1e5fa734ee54a37d" },
                { "si", "d2b5354118deb8550ac1307371db91e968eef619d43144f85d1cd2fcb0c66aa89db49d2ed3354ee2ee1a12d08bd2ddd2359b616effb79352be6d45adfdb762e5" },
                { "sk", "e41cfb3c8f90bfef82856db13318ee525503442b4aa42cd1e8fc3409014d7aff538cd632cdbb0c61f50b9af0ee34fcf4bd2987d1cca9549594e64450773dd0c7" },
                { "sl", "49246fa5b6ff72921bb31716486bb15b1d6217053fc31df7345200e66d8449c1dd4e2a8fc432b99b632b4a8cf7c84ad116cc59f000515d32ccbc3e39d47ed3ac" },
                { "son", "703e0fc52847558cec61d86aa7338cc9288265a3ef21aa22bb01d6c87f3fb24c7223219028d6fe56e27e37292a569a94cddaf43cc5c5058f091ed1e5b05f189d" },
                { "sq", "17e3ed17cef2836c0a8063aa6225c698d57a622520e8b6f09bc01b0f3dbd692857ba1a60c954d026f220363e16d646738d9f9f74be2e04f1b2cb378873575ba0" },
                { "sr", "98e92711ba3694fcce101df56215fafc902d1b9796aa84899055e4fab66b5d82a71dfaf7e6b378ca1fb1a796cfcfd0560f09cacf567792eae400cd1e884c542f" },
                { "sv-SE", "dc72006df2d77b384f271027d1204cf0b8e7ba1fe57c24ec4a57f856627a76bbfdc5dbea76af9adef3e53d757bad0f88e9a293a1b5ffdc2c32e825fff7dc0489" },
                { "szl", "abdb81ffe5fea8bfcf9b011635440e5267279b630bad0df983558301246639e2f3d5cedc91eac40a6247e8f0ee248c6dc391fdeb449191296a60e72d534d6af0" },
                { "ta", "9d28234880a75219d6fe091b5083eee4c9ef053f4751e453249c93842bd3cdcbff9a6d74e3114f993e1ef6913e315b21a8287c6d4ce057e9d1123e96684683d5" },
                { "te", "cc58b0a5db6c5e8b654170f7c6440d691281a3e7f1a46ecd67fdf8f0ba62b2640ffa6ec488468a37e65a6e2b76fb679f580475d660788d18be52cdef2d1af406" },
                { "tg", "fb521fc75f2b0b15d29bc7379c4da2b58a6c78ecde206439971e602236e55c5fbc736af622ccd2a8b0723729b125d67db09e127f7ad848f5f0d6b5ad80fc8bbe" },
                { "th", "3d6dfb5bf7ed1ca469b6e70c05ea9329c8c176219490e1e50ea2b5bc7d60fff81e034861e90c8bc2945856d76331f9870d9e8b7fa4f0c785ccd05b65431219ff" },
                { "tl", "80a3296b9d0395ad2efd7e315832cd3b9b4f6d2e880bf8056d66b72eed26eaa889f4430e94068b8e7b40906369c385d74a4f4991e9a9e3cc575ae618baeb9c5c" },
                { "tr", "f175785463c6bd1bf96b05c3b1cdc48f9c0e076094c9dad97af898f351f8cdbd92f56c0e6375acd98a2182f3a7ed208110a45af415df67b8c49d5f4cbbc2cf9b" },
                { "trs", "a60f6196e87ebaa30c626a5d7e8644788272409df8d95bbff63e7056595e78945f625c196f29f8e5476775fc380b9d9b28230386e3d28b8c15f3fdfdee5b9fac" },
                { "uk", "d8eccc63ce83d7d24b7a18ec4c942db5dc2fe2d1d05bd843556b8dfc55762aafb1b4fed3a2c491b5b81d578b8f660852bc97f359da45f2a7789d204380c93e20" },
                { "ur", "7e33bf39106463db00ee0455ca61caa7d14a025d0ba0948ed4a49445d04a6bd5469bd0d922c2e4a14a7ca048be1c8783c94d2b34386002578e7d618621389bb1" },
                { "uz", "34ccfbc04aa524a96d1dabe7cc2320a69127a35370c3c3c8769d159dd0943ef95f789c3a9fb10c4f59841724404c6f7be1f7163b455c40b54f1df4fdcc18e150" },
                { "vi", "6c1d5b9a782612441202f434b24d2ed307212d98fe9bda9d470a50a16f54fb473b7be41eaaeefc5cabaf833939b3aa261ede203293aa2b0d1cb57f12a60ccaa0" },
                { "xh", "e0748f91cdad677a2d103af2a5cb9255dad0ade18d1e097c605a18b1fff94c905215d2aee5cd7c86b9882e2ea3f8e7cc5df83f0c24e459beb11ae67fa0421a31" },
                { "zh-CN", "b6fcfecaa14a365bf95db7cefbfdcc0471e05399a9f36269372f355ceb0cb41a5e398a3a17b2277bbf9c1b946882a82ad0203d7903a670ee84a48d8bd2ba4577" },
                { "zh-TW", "546da02cd9a435dac929fe5624b4f101a0dae2915229172d99cc21f2e2dcd39a7d89cd29d329f4286e393436dd5923339e777dea438c2e089af7be9e9a722a9a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "0fcce4736a1653f98e2631826d1a2e940081be2fd0d4559f998735d9091382b649150a217891785aee44ea9de2bc3b4fc699f7e2b3a0ee1d1d38839533dc107f" },
                { "af", "fe5beee43a80d558ecc5509088ac72ba677d7597471146e5390a8d26da33dfb83152cdef88791f532dbe15aa03c31924a0f7b4ad1c1dea8bc5e163e13ecb2812" },
                { "an", "728f3af85b05586bdf55e5620ac41ec53191e8e1de3d981673f16132de3b267083f54e0808f5919633e57b4d71dd0aedfa809d013622884346e13b4e094660ae" },
                { "ar", "6ce12e06cb62a9e8e605a01dd935e7365ffb299a41c7f0ca6cb5b803c96dafab9dc3a953319f47f28a76c67005b4d1194fd1a24932268fc67c587c288d6869f3" },
                { "ast", "d6351838edb57d00b2f1e1ee8826d9c3efb10d467a3826e70fc1f4e8809e3be44c030323faeea0f2f511eef063c6da8895afffd5b4042c5e1e9400bebd87191c" },
                { "az", "a95f908677236d2f433a6ff34b5ccb8846dc9da7be54cb05a920420aee731af4ff439c6cb722e3804a8e0bf36b7e483a597627ccc21c40001bdcb813883acd56" },
                { "be", "415ae80546e4d08a3a6f4d0ec3e73e64469513f0478ce8cb8ae783ee60f120ec10a66aab130d75a9990c8c779c83e48199f0d6f6346581b0ff58165cbc7ae00d" },
                { "bg", "1eee027ffd500c6380f74c602eed58d9fa9665768a42a2b1406a85ffad7519acb20a6b1939121303f9ac82efe42ebb9eaad21b4684d4996e27d1d024e195dea4" },
                { "bn", "88559d55219f0bf1f53a951d4cbef5d96255a7b8da7e2200971e57f64829a06bf5e54168653f6661fffd6e98ca552e4630b7c366185e9ee034892b54f33b95fa" },
                { "br", "5d9a289da7d371089d2346fa5b26b8fa42586e92da331c8f20ccb16a564407f5d64d7c5cd420432b1d478bf8592b2f253b1feb3584852066a641fec2250d6951" },
                { "bs", "4a18e51e2d3e2513850fc24bddbf38216fbbe6857f259229429df1f4d21e72d4a33ee135226962ab0bf488e369ba52b95abcd7db18cdcbf39f553bd1f46d4ba0" },
                { "ca", "f78b28465ee0b5304fee7fa30638ff889ac8e30fe39df3a92ec08f2963de4dc8f1731f5eeb246c88abc57f5df6da97e7004ed84799293a923e1cd81cc2e30fce" },
                { "cak", "d881635f231db9a46809fa6fcbc220b118700c09cc6cebd965a70b331fc7ff98045a5765767805f4d9e88ac1eab821e395a5d673b0d757a45d8cbdfef71e2f26" },
                { "cs", "491eeef8fc7a1f398095c9fe78cc221e046bc07f7700016ac48811f397b16a4be72f57b14de1f859b9e3f0ad352944b708f3be9a9f266ae9e3e2c41b30afc3d5" },
                { "cy", "faa3d6659d0d24ad989ead618bd9d66cedae274ae1f8dcb9860f80928c386dbe83a698fd046f844a89615e5481c747fd3af977df213de715abdd8b66801726e1" },
                { "da", "b99fee4642b89ded58deaf4045407f9894f5361b20e0f96f52fbfc2b20a9a56a44ca840d2ae8850e10e92c08f486e0e1052a40e803d5e7e9a3693da95c6837c8" },
                { "de", "f721803f8e37840c7caaae6aa768545ebb532a76b85b88a509d25ad0169dd030525daff131fb703fee58e38104d446a27abd83239f06197e2a1570268de4fe3b" },
                { "dsb", "1dc959dd186936184502539dcf98f8e84d906523a671e972ffb1b23cee4ece58a97a75f4c0b6230ff07edf624882b176809ba4376dbd982468be95b8ddb84379" },
                { "el", "37edcf1fd85744304f3fd95c00d74baf272aef1208ae1c256bac0b261c0fde14d28c700e101e50256bfbc060d73f662f1e767c9bf071709fea7b5acbfc5d5c73" },
                { "en-CA", "af1d3416f45a885e77e1bf4dc689170108448f611cf502f7e952a5b21a259bc7c551d2722c614b2de110713249f5fac769c0bb6e1dc1cf5436b10ba33bae710f" },
                { "en-GB", "0c4bbb3fbb6bc5f5f92c200ebddeffb30b7b00c401d618289e551b657b71e6b9e9b2a3f2f7fd3e4c17b48fe0ed8c62025fba3e7c1be86eaa7fb49a0cd66e0ba6" },
                { "en-US", "8e1b54a10f21612430b196f498914094d14d2831187d22579012a107fdd1aded65c5cf5d08b7f76dd1e74673a5adb4fa98b05166035e1a652f83a8a4c1a38ba8" },
                { "eo", "98e7b019a565927b55bac353d19539ed2a39e9bcbd876ee1b2f9d073df5753ba8b73f73aa8df141dbb92b7d62859cf2de40fb8487ea9018ebb3519aeedaaba60" },
                { "es-AR", "083227762a12beccce6238367e83bd1e944e01c83abb49b111eaa1c9d866b01c011bb62f54a12dd7dc61eac1729467d7bc0018fb19aa6ccaa7a39ef44979ddc6" },
                { "es-CL", "c6bbf98e1b9f592b68b7ddada777f7b5e6b1de4ef483042b6514f66283aafcea97586963511b6a59d56d970db3305e67387969c0394fbdaa7bd6798c4ea26a12" },
                { "es-ES", "bcc13b8be37b5a553a86966f54ce5088d2e80eaf130f928074a43c95762b7d822d468ab731a6d3069637b8780ea27a9cd2c3d5df6e5d2cd7f4f0bcbf1bf3472f" },
                { "es-MX", "f8981ab5d77b2b6e6b4eeee8799fd82a36693183d10a91bef4256c08303f351468b349dbb1345def16d1ce184ad5e5ac82c1a45489e1ddb833e69e558687c407" },
                { "et", "9f83262546ae0fc77739dd589375c4c4082a0b021dd486fc7e3f19e150d05f00b2e234d67b5366743694fdb4c4590dc887d77783bcde34fc78aabddb6c10f753" },
                { "eu", "2ee28bf775138af34dca420066830817c21c469602a4692e675b0ffac89b2e422513235da8cf6ee2b77acbfae87749065a529c3759583a971ba77333b65833cd" },
                { "fa", "2f5e87e6eda2b30492768c186633ff9407b3b98a9d1450e725342e4e0bcfd2d21b2ad7e731174b6137fe33d304f52baba05db5e0a972932e16634cc6b65de032" },
                { "ff", "061041c8d10fd7e179d00ad8ab0bdfbfd206ac3bf7ea9167103d63dc7a47bed3bdad6d49c672f99e437d71c38e23335f88a2a29868278c4ec9f444a0788e1fde" },
                { "fi", "2acab0014fe4b508c49cfad43e2be35a399389e824a4a371c282f6a7047c06fbe5eb5f8117a0ae0ace615a256e05562febbd6242e4f9d8ff649a7066b3f99452" },
                { "fr", "c0d88c5a29ef46ef3c07f7a5c690e4bff22074e4f6e850cafd886a7e1f020736d787a334cb8fbc5f2614abf9368fd06002b65e17af572d135e7bdafc5340266a" },
                { "fur", "8f2196f112679a7ad2d0881d6ea81e618ed4bba31d0f7dde50c4017f949f969743dc6bb1e679e029439d59dc5e2403e0f97ad20c994561d20687bd68d5b89fba" },
                { "fy-NL", "cd8614dd96f854d2c5007bf0c115585a8f360914c4c237e04ff3a49548f2b9c0c12e7d61ca8152336e1b80a297b059fccc7a374d3e66ba0cf701487e52b9cebb" },
                { "ga-IE", "4dafb8d3ffb90742f6c31cf615ebe084b796fda6b7d5dcb31893e971840207218be44650830ed888342019e737d91c3a31283429ab2c18982a4f86627bc3139e" },
                { "gd", "e63014c856a6d00f0e2ee324d2cac55b37efaa73268c443c44bea4a764d1fc858a45f9c85f3ca0f2c8d3e19f3ba77f95be2847f4222370a8683cec4644d48e11" },
                { "gl", "647bc7d10f86f911f43d49bdf18718090dfae5382d8cf79a7d935683fca74ecf86d41641bb53fc5665e6d1a958d0c9c6465a3d2484efd4d93d4a4e623cba5a1f" },
                { "gn", "ae8f3725ff0444d702169cf6c96a646a4f9b19173d8ba6e0527a91d74054561863869ff4c212d674496807a3a48219953a893ef07e68be665bfefdc025e7bffa" },
                { "gu-IN", "1be1ac575759825655cd933e72a96269179aee94e5209ccb7e505fb85382ab6c507df3cdf5d3dc725dcdbde47a3a5bca68b7800cd06ab1ae76802fd1b92b3f78" },
                { "he", "daa3699b32b0aa3e1f821bf843f85a56b4e148e0d01e6421d25861c54267061029f686bb45359b4bf7cb1deb83f10f8729e717b7d785d1b38a63d68b78daf3a0" },
                { "hi-IN", "a9c70d5cfb54a393545564d89600f08164f3cbb14f85891349fbd1c008146a119932fe038406e9706a2f45b13016df9dc63380ede7d325acb141e1c3304e1b5b" },
                { "hr", "4222767a6befd92097b87cb6c0b3b4ae73b46f115babade5cc7209b6313db9103e65a53701e3cf68241dc71296242af451d668320f4d13970b2a91cbe3797df4" },
                { "hsb", "e39442f6086bc46cd32a4cff9f9c50b852c9210fe461b4c435322eff0c0f90d2111d5b138e6755c278ea701bd4f26ddcc5b0535149a6a7019215774a7facb140" },
                { "hu", "d05ea5bd53931d62de7a363764d93a0c8b917e17925d993e10813d1e30cba38a9a40743e349496d0e510f06aaf6ccd7e73b88763de4688623952e65fd4921bec" },
                { "hy-AM", "70f5ff45e754c50337f9fc15d3314f64f03ca8632981658695e6a15f350cdf09140b9248be3a5325f78ee67f514d873bda04759eb2e9dcb37747baf14d8ef3da" },
                { "ia", "303fd256bc4e8b6b751780593082ec782147c0781235216df7ce087fff36f0359c566bea650d456ba6609e2adf81f5a354ce1be2951e3501ffdc097a96894a19" },
                { "id", "9e8c8b5254eb23ab8359b04c105fc6127dd8c749658a86bf0494217da91b4c4ee68dfe3ea25ff18f9ba3522f7135a34d0219d28952cd508283e683119c32c562" },
                { "is", "e7d7f58b5819e5fbae70d855dcdbbc4e7726d47c70944f18af851f2a258484d08672154faaa592cd9c42d6dcdab992d04019a09301cb1b6529cb6c53c22c66f0" },
                { "it", "a78cc43a10560743cc602837011f402ff9a49de405d8a38d9024d51c2f68e2e199c5c46473a49809cfbe7a10d7684b2742cb3e7a789300b8d9b2c7ae2f8e6890" },
                { "ja", "50aa85669ca369f9bef3e34387cecfc1469bb868ea16067e07bfcdd59426d46421b0491f2e0fa6ab9ce963b78f6aee071676d8290d350e8524f6d57937b5a2d7" },
                { "ka", "5fbf16b5ac0df081c1aa76b3a45f063a08ceb55672cc55bea8880ef24065fda4c6f8e550904afd5c7211ea02139dfe6c474fe2d6fb6f3241f6fbebb247698300" },
                { "kab", "f1a6592867b36badff0ac3bdb06754a078cf3d60769b261cc13b81c42af6e9caf094e7df3235af58cbda2a8545beda01661b2410cc4e0601a2b9e372e6788cf9" },
                { "kk", "54d748e500cb95f89007ea4794e30a7ad44a1fa9784f9e2f73c64eadf7202e5528be700a96d805f8f30a93eb98e10791303285daf5e513a8f02735a24dc2b947" },
                { "km", "808626cf6f345c54e8ece94cf995b7234f3aff36047cdf9ae79e6cc95fd44894f116ec0c86cbd1d46d0b0dc66ce92df82380d66e4b031e68703cd700918843b0" },
                { "kn", "1761263c924e350ec4cdca851b5924b7da48f91947148a326c9c2dd3752191578e6e01654c70d219d48456f84935e1ec45184ba6381437a91009af5889ffe850" },
                { "ko", "5b212bd4b70c04d2b6e0ae2a72d1083d4b7c72d025cb0101c2bbd4693732ad7b9eaf61f5719449df6e2afe415bbce75c64b707a258d108e05417db53df9a9a44" },
                { "lij", "04911f1e3c60b11323059941130c34d39cdd248937f8427cb0d1ed0142790871ccb6db58daee67fa870a312c284fe29e683d8a3ab24fcbc5528aac972540a6b2" },
                { "lt", "2dd918df71572f3f3b019832931a0b88acbf3fc123975aad9b982283e4f5f0b3b5a30c5dd2e8dd2bb11f14f2b5975059460011162b11a22b3ed11c4f9add0d16" },
                { "lv", "74c49325d194e66867cd5869fcd9e5b4d935c438bcefb13d779ad5c19ecb8ca42465fcc6315c3a32739aa668eed3b9ff0eed82612ea8171a93da1c58ed1f3972" },
                { "mk", "8892b116696af1795c2004f6cd03411b9db8d1fdea83b8bba46167fe0899f7161524d3a5c7ff48dc13c035076b5cd55628ffb00b892318223daef4ea09e5ba91" },
                { "mr", "ff4f156b309ae9322d23b5a0e1de8f7a7a4e8b2c04cc038a0dd4adca73be208432d5c9711d9109b0f338007b25ee0d5e5ba28eb97f9f8273289359c08cf63fdd" },
                { "ms", "a876492005d2bb66165bea7d3a8856ef66e937ae9e8ea1f9549399fb8df4c7b638a9306f258b57c7af10ac88b9c87b41d8dae34a08a5a9e79178302be98a6b83" },
                { "my", "3688dd780783a2b25323ea201696fe97b593b2edf19a7e6f26f3d65c880be3ee8e44a1db6751da2a1735033cc1c1de063073cdb1fdd4aa6ada39ae7caa3e26d3" },
                { "nb-NO", "8d4e7fadfb0f41f6b613e4bcbf32dfc81840847cbf65b1fc43b94837ee196ccd43562ecd82198f60dcd631d6ec6ad9443c7b4a98c51d394e4b1390074f92bd0e" },
                { "ne-NP", "eabec9cf844585eab2d4f9b04d4a95fe5fe2c41a99f6c62b85c4d6ac1d24c9c36248355bd163a5c7aef07d4857ec0e7aa0bde4107895063ca460feb200a2f083" },
                { "nl", "59eb272aab43846e33809af1070936ed105bedc0680fc70c633552ecb5bb4daec94131823a58ed129c49376f38ef95a4c18e9b620ff8171755c02e9c4c9e4406" },
                { "nn-NO", "babb604c8c913a927d46ce249a9e608ac9df3c7bee805a69c9ff76e2afaeb6765e4026753b7f7f11394c936f922b92b33c7c97c9f62be4c44a7cc055b7cd6a95" },
                { "oc", "9be8e9b8ca7ae6541e3b9f386c26136db94750e74bc331df68218a0d45b37ade4228e6801f453731a035172ac35293dbfcc517a5b30101223b7bb31a4a1f0541" },
                { "pa-IN", "bbcc3c2ecf4dc1b7b04c25bf649adadb8678d8c388276bb59c23c95525d82973ffdd88e0328ffe603528485631dc6468b637f5180e2b7eb3f190b931cc329987" },
                { "pl", "67ef111cf304d93bfb090335f3879157f76667f1abd0e94325ab59f42617c93e9b060dbbec1c9d0959023510dc7fe5d3470c06372445bb541fc68e4e7c8feb2a" },
                { "pt-BR", "a966832cae26b20e95158bc69a336bf40a4d200e3e5c316c16237cf0d21365b257f89a64e270f13ac6d10590f1a836712e0843d894d5bbd9c64673ea337ebf3b" },
                { "pt-PT", "063a9e8702dfb151d27014799bd23787a0a10b7d50162c7e7944fce6e4ee3aecbe23137aeb1418e174957c915ad048d8654a0710735092fcaa34d8b2a8ec2cd7" },
                { "rm", "b6177d641b9fce268a8b414c7f70c574ce2d0a4c9270cb03d9b2ad3b203aed6a6ed9db532756dff2a51dce2fa5bf8642249880eadf46f10e5e8c1a0e56c1908a" },
                { "ro", "b9c2449739dc0b6be844b090fb4a0a9b761e569275f8ab2663b968d0fecd56d124705c582ded579eab50414774cfe5d8bf0653fdbaafc9a51b5b62442e2cc711" },
                { "ru", "8a3c21f3a7155d4cc66dd038ebc1777125fdc14fcf49a7d852cae4bb608a435e9634a0181b366c59a30350d2fbe9cd43c7216062be1409028aa32206f12d8567" },
                { "sc", "b9a4f923493a100b959e84617b6b23b789612cc2da12373900947a8f54179c8bc8e20b5b218e9c8d4dd3e0d5f04092a2b00415c73b8d7651c02316e3cb310b80" },
                { "sco", "4bc446ff8a6275c42fb6cd45d6356b170d1339d988ff556c0ead248f42cb51854d08389f6693a136eb05f13edec34dd3479d7f66035d3f727afbbdd574c53021" },
                { "si", "0b905cc3ccbdeb7c9c061076e56761787c57c0733d39fa8f24a2d32c37fe373604ddc0132789eb4f314aa752784580cd83d51a921c3ffe0992ba9d37f17f7632" },
                { "sk", "b3be1d50d01083327b94df6410dd3b52753981f426f29c63d3fe9043573bc626e76af70cc381b37f214c04d650f4c54d7065b743eaeb541be4baf395e69a3975" },
                { "sl", "88d8374a75450d7855e8ac724be9978f97ac28e40d55fdd9218e94341e41af83196364190683631f96cbf4f00f136dfa0acd3ad5e49aee6b724b81d55e4722b3" },
                { "son", "8904b60012540ace70d8214729f6bd71c69f6da92271aa4b10d482d35233b5956488fd3ed18d9bc650063127ce3d24554e35dd2bb1bbd6791b06e88706b3119f" },
                { "sq", "9c82af429410a1b47d51eb0df262ba9a87c1b51a110ecd499780b091f1801c3470880c92a8ca933b3b5e3b8a45293333d1ad08eb3b60c01d251630b887baaee2" },
                { "sr", "9e96310e4212748cb454dcc7c5ee996eaf7622564431d49c58f3c469eedc750d51cefd8a1bb14a0aca3591737f444429bf829450b4b990e16f8f16ebffaeb87b" },
                { "sv-SE", "489b6c9dd786c20733e5e4216d35cf9df0b0e3eda5bd99eb44b3a38d8a39a6f6d07aacd5227cbb635295efdd808c57ba94bc07001a264224952342653e7064cf" },
                { "szl", "65b6a452f993baef2d79aedf17e260d79bf7b14a13f3580c9a829c53b6ca264fd35dc95711e62067d4f3792a195c2e6c43df7a9aac77b0a022c375177cff5a47" },
                { "ta", "d828901906eb515bf1e1640f3fcd0a1ebfb7825f2c59ff93bb29af3541aba051dcb97a4838168685e2c9d722a71b981101b60dda2fe4f9933ba25c277c0ee5f0" },
                { "te", "3e30aca63f250a7cbf51b67388184bf5ca56e1db36af6215beecfb2770c22e1f527d02a328ef65187f22986ca3d172d65ac225008045ccf93ec8f2bbde09c3f4" },
                { "tg", "bc89ec10e9f09a3f0b0adfd973752435175fddf0675001a621bab91ee29ac327ee6735f89407474cec4945292ca286ac8ef6ea469e960704a6b1e2d176bea8ca" },
                { "th", "dd8fb086118c125da990bcc4795822e7f4668b5303279686b74d4b05a4d471117fd9f975b03b26f956a5c9b4b5cc6efbce17d83884d642da8f17aadc1ccc9c85" },
                { "tl", "a0caa6f95b7d15d8e52bbeaec6fd2db909e75eed52b76d05f47ac7e36ae62fd30d657e0c127890d8fcae0b29a5fa369d2a56f998567b16516ff05a03b339e763" },
                { "tr", "fcb6d8ed575a104c442f9eafe46235ecab995f2aa80d53a9060812c63f22a90d6ec049935cb5cbf3b2d7b2f1d0135e2e1267dfa015443731f56ac90e485a92ba" },
                { "trs", "636e42688298dc755db728f58839db59498bdc613903f876382fe6016c79f91db0c19f768d99b14c0fae15c86b359755283acd28e55abd96180e1a916fcec4f3" },
                { "uk", "024bd2a197c9aaa7ed34d51fb29181001087aadd8d7fbaa9f8952f69d73483f27903a5d2e209c85cafd4b5df922399cd3d7b4ba359e53d5159458ca59719080f" },
                { "ur", "2ce15dabbda8c477998d813f3d39178d638d9132bea501c3970ab71a57e4e90a4236d65c6e904f7b14e125387fbbe2fa3c93c3d429659409d473525cb9e71311" },
                { "uz", "1e85c7b95f666fecf456b78452c7c6f370dba1664dd7db4c262855f283ff244b5d1ba24300b0c328d8c60a2ebce3ef9513ac9cc26b9665b16175d5e4f14d9e89" },
                { "vi", "0080c1b62ad5a62b9e8b044687f8e876aa6b66b29c9800ff883639569a1257a92052cd22a026315d7437f91e17a9d2becaeb892de73f851d986311b15d6e0fd9" },
                { "xh", "d2a51f24093719c3df6ab12270d5001d3575d6b8e19e5d3e8e914e6ca70cf2e32592bfe7517d86b8719f260a21c95be749799a58c84bc69661c4921ca4638b9c" },
                { "zh-CN", "a26912ad003c28b6a0cd73f273f12d1249552b055211862c0a94f0a218100de2363b4fe96d5dca584a74177a2ff52418012c1c55df736c240e76380ff9d08bd3" },
                { "zh-TW", "5b811cc45fdcef1b95201195dee7e639586635f6d003e8e7d8d7dcdb58a081be96ad4f9298ab1c69a393ad5ed3e86f6376829a8004fff6acfc569b01ac7c2bdc" }
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
