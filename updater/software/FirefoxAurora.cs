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
        private const string currentVersion = "115.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "9bc3fba0341c84512dbc9c5d833f0835d84056275e736b42f54b59e52007964ca8feab953d040efb6ba97385bda835176023b784bff406e4c94307dba86dc2ee" },
                { "af", "13d6cfebcff15394b19af3df520b2be69d3af010719ab52499e87e495c3443bc6d355a53e9a64ecf34905220f2d7f0d1f64a4f02f5ebde7bb3e89d273eb9ed76" },
                { "an", "81fecbf8ef49c76133ae77102c1ff49fc559dcba03f43919dedb7b69e4bc5541fcad2e1db831af4772507acc383d7bf058190008898d53ee197169ca0af07b3c" },
                { "ar", "313f67e5a44865d5ce28e1a42a5185d39b9bb569c234ccebeaffae1322d1183ee8391288e8ef652243f253ca99dc10fa3e3cbf66b27312ec0e65fc381e6a8427" },
                { "ast", "45f1515f4f6ffafe2fe8a29001b422b963d11b94c8f1934ac056652cd128df115f1d737804434fd3d7a54e1f751ce1bb29cb0b7ac50fbb21e721f42ca9082d3b" },
                { "az", "2caaee0d44865dfdd9ad45c3202da3bd9197f8bd9163e2ff6d406d3f05cbda2e3c156fa1a8401b8fddf3b448e5cb2e6d8e45cb1ac554db4809c5f0a91f3f34d0" },
                { "be", "d2c0c800baad4c3899111fecc8d09262db96a9d355eb998606dc841ceb09b08e14d72490d1d358a72731451dd92b6d1b0bef97ffb03a774da51e3590c6822675" },
                { "bg", "8bb43cb9ec55e1edaed0595d343c7363d80821fb0d0e94ce5f40f624197c2ecc2a304000645e095ef10b242445fafc1629551c53329dd4f411a7665a340e499a" },
                { "bn", "326b16d41291894b3ba927acc52452c46df3d54b073c657d55b7a7957ec2b0791692f33c6c816a8eefffdf632469b92fdf4e3cfb267f03bcacb2f4fb9865dd16" },
                { "br", "31daaf95af38efa5ec3d9b0697e9674e304f6bd39b8d7c598f4c2bdc376fd41097c624480409201368a1c7414d3ccc588c78d1307e6d83917ba7f8af53673b40" },
                { "bs", "47bb181d1ad9cdf30c220d89d9f4826a06bb95911d79913cf9f1b5dfc2feae532050ae40c47aef6fb0f4a92a5c15cfd20b07431163055cedc7c21ed2f0bf121a" },
                { "ca", "0d3d154a9b6880776d372d669f07702edfb3dc8eac0dab699035f3c21a5a7332323925928bb19823c207b70ba3034643a1265923488346306f6c88691722db6c" },
                { "cak", "b751bebc6f488bae87ce294264d075d852943f5c7100e92f9f2dc983d03818f59cd038d73d5f4ee25ce90fd9e1c3e3dd579d477e1fde37f53cfede604918eca5" },
                { "cs", "669d040f30934ccbf581f4e14a5daec852ad854c4126d3173e85a97c7396d696ff745f0833872e933f7edf6f03d040d0df9c88ccca1359f1f437d283d116278d" },
                { "cy", "0f6576fd0e518b235a921180cca4098496faa6db306433444742b4b69e30ef9069b7451393aa9781fb4c1388ba4f7c29d8de18be0c84419710311dbb5c1f807b" },
                { "da", "b257b1fdd7cddee38b867e6551aeceecbbab76275ed27dd1bdb1aae91e1500be439ee45bdef9e54709cd763fa0eb270b7e823fc7ae568191e45bb6004bc9da19" },
                { "de", "5406a380453df77f88dbe59db6947997e9d5776f2efd04a008c84a1e7f663918de20edea1ac8b28805fa63685ab6a05c01fa30788dd4c144f57b9db1153c10bd" },
                { "dsb", "6ce54a81696427b3d99aeac74360b0ecf8db8e815466da2991b0797a63217aeede06ce4d1485742fe5572334100034426aa30305e9a1405ccd1c14246d31b9a5" },
                { "el", "1b031d2276e1bd8f9685561f8576f6a6b3f3120776be5a19ccd78d1eaafe3da9d82998a63c2333f3e818ff71477e92c171e0844554406a7a7b538f40f058845b" },
                { "en-CA", "88354775b4d84446aa6ff906f05693c6b004f9fd09dfad5ae02bc3223f1bf6f3d59119ed18c7f9da2ab822e5b0ae6712de9610deb77729c604baf3a45d8ac63e" },
                { "en-GB", "00f02fa1c010cf0138c0d4564f92abd07b6528b13261529dce4173ab2e0ceca2e8e59a5e32fc18ce356a1ed0034d87960d7330f3fc4eb7e47fcbf375acd20f2e" },
                { "en-US", "367e27f83a3b1cd7dc797eb0aaa013926bde3208abaf03793b5f5a510961d3a86bc37efc2691f34b2ce99bc0303b4cffdb210ed319b816a196de3fe6084ffd40" },
                { "eo", "396f0509d90537c20b7d2eae0aea136a37fe8f75b8af663d7088fbc8373b24b0482196c6ca8cc5c207663ff9b78c455571ae8df78fbe2bdd96fcd3de7640fd13" },
                { "es-AR", "053b89774217d608c47ab2797997be16a0377a4fbdc441802323f0cacc090aab4a9e852aed46be48952b709fa575de4306d42fd32d8e313daf89f538946ab1c1" },
                { "es-CL", "0c311162b8a5017e0edac0b0fd5ceecf1973ed12611a8ff008e7dbe9344734e05512fb813c575fb2cbcc2a7d4c532a48526a7fd3eebdd560624b2f0b83f36045" },
                { "es-ES", "bfb12b8c662131ab66c4091c2983f196f285bd7607514b580c0290ddff4e54d19c01f57f47cbf611eaca1b562a5fbfa04bc486c2a21ae4fce63a0375b1ad4bbe" },
                { "es-MX", "fadcd9847b757b5bfbc506a9b88b98d52805a0ea5467c292592255759562584bbc98fef57e9762c157b92a69ef64750ddbf63ef9eefe34477712493491a6dff8" },
                { "et", "8c894cf49517fe570694daba0ac7731d70e18eee382104c6230461899b08710481e96bf79807acd991a2022f92e05b165d3e6f370350a8a411d35b80ac2116e0" },
                { "eu", "1df96b13304a6a73be9e27a3bbba361fc987921f1acb35904934059712f30fe8736fbe4ce540ba5c798c9b3295645548efb28f6b9a4f608459129f34a452aa5a" },
                { "fa", "e47a3a5f66f327bf2f90c6da9ae6d389b63cb21d931131ee8c166bdc45e27e58076bb597a124594dd9369b0b41b0b5112612da702ba59571f7b9e03f54aa7392" },
                { "ff", "be44c935a4c31472c54dd3a27d9bdd8d4846081f342696e6d6a21e420b755ab87143a6b6ac19a91f313c592e38272022188f2a6c31247250d6227a0c4ffd9ee0" },
                { "fi", "ef1d33afd56b6cc06def2d5c50d8dab43ee44fd47d334e9c5212bb719ad7fe7a1e4d85dcd22e8bbb47f0f454f23c1bcd2ae994b8d28b85d0043431b6c9d37c0f" },
                { "fr", "95be8a1be8f3bc8cb985b7677ae0361ad93b83a8456e1655b5dc3ac09e2dbd8e467314c53920e70b851364fede34e2d24032fe7f9e3f5ee31fb31d864fe1eec3" },
                { "fur", "b2f7dd490c2435c32554dfba0daf146781bd8b1acf325435f826745c4c31d29a580782c799ccbb395786b14feee871f8fb0aaea78defffce763b400dc34de918" },
                { "fy-NL", "1fa20db5ffb932d9a7deaa0f06496b6bc71230357b73d9c38a49c9a38af1742a31098d447bba70fe24c150350cf02af3df0d0e4caa05fa4514765c231c70475b" },
                { "ga-IE", "985b3f0c7de142ee910416ebdb60369a3b4f4fd7ed807cdcfee2e1d6f11dcfecb439d6c7b4b014b50f7ef9a3811ed8af54e841c28338d7f69a31a6e118f68738" },
                { "gd", "40da5a2c721c86808c5dac681ea77c750c51bbc9de69f588d6d3d2ae1f97f46b9a37c20aa002ea150ea6765a132c2b969aafef9edb4f3eeeebfb57ac33f9bcb8" },
                { "gl", "25ecfe7168a50f8e967743895579899cc52f1567ba18acdf574b39afa25ce8ce4a29a20f0efcac969d5a90defc77398de4ba80d1a270601702646fefe6dda3d9" },
                { "gn", "d6590b15e2aa47b759361dbc9cf91f23708bbb4aabdc5f1f8d2c16f052ea4e00ae3029d73e5df40bdcd0c8fca772b29d97be961c13a4258cb50044477bc805c7" },
                { "gu-IN", "e916f74792486f8aa0c25fb1e77fe2ed360f533742a65fca6d598d11df10458528518a7e4e963e669e7f12c6a1418a37fd12812fe1ae9a8bc6f799d156bf84b4" },
                { "he", "3823cf7a98975854e984c01b200608a453c17dc63fab9acc4fbfa0ee3424466d3fde13999115e05081187a573746228ffd35a53f0ac56013bb6a8a8c29daef62" },
                { "hi-IN", "091228bf62db5b4175afff8491a768bb4ea664771cee43cd7431234a90fad7909232b5406d00560f7842bb35223d1e2407c0b001b0b2357b94ca6525752a96e1" },
                { "hr", "c14a360fbdb63b5e2464b58c58b135ebc8c00ec0d4d69b7a661379aa2cb0e8cc5a503bc45ee41220aa90ebe7f1a62cfab380c5dda6c5889eb7a6baffb84af7da" },
                { "hsb", "9a0a011e71bc363ff4493e54728539f51c37a0518aa950884981cb48f90f48ddd13db1658c26fd8a954cac3c35962b5919122751904eec29289a6b3a618794b0" },
                { "hu", "e8c47d6383dea22140864be642f91ed3a4f3a65680c137274acfdc195866a48b68a46a248fa34b9d2d416405064d1dd9455128168312fd233165e900aca3478e" },
                { "hy-AM", "2ee18b27c54533b22f4a8c49f783036b6b6143fb6e57b2b85ca89ebe9c3216e68b2e7b4df5cd9dc1a09bda290b0b15120221263fe84ed8472c85175b20d1b895" },
                { "ia", "898febc064f28246d673a2def6fb9f5a89ebe08ba91b5ed37546f2660aa7a0814dbdc8eb9b23a128ce9095d2129e1f6544a5ad7a252713bd99507d258db61759" },
                { "id", "a48e41029e726066af23cc4fea0ffd2fd0c0b7e7fb7a423439cfd9dc73c8cabad556e2c4a33e00d8a555eab18101dd762eef6f12fe3237c95e18b399c7d2f50b" },
                { "is", "89294c10f14f6f907f6c52b7d7c466240bfbe85dcc31f35436b7bffb2f54318b46de57f4537b82ea5180619f6a9783e6ec5604c9c4929526c0d92e69e56c6c3d" },
                { "it", "98e9404e87d9892fb4d4b4a0e62b96f7f7850f5004e58242b5538ecbb6f07bb4f27d008dd7e12a98b5a88ba60063d0f74b13b718f5ec39d023e4240a8ce51b99" },
                { "ja", "aa22aa94fa07b57babc6ba1f81e1d9fcb33e4b2b4c0e61712002d5da992b2f18761fe686a5c45f2ca36205eaeec83059783d0eb2e9ad343a8e01d03f377ee449" },
                { "ka", "14b38e7e51b4a82fad09a15a05e8342b32b1907437403274171821af90d9443a076406975ba5ccd46551be6ec2527e322bb31068db41005d3b54bf833d8b3493" },
                { "kab", "3728434ce46cc998da9d24e471d1b54dbcf6b94f5d86339036def0fc8be23b43ec86cf11b1248a0c5366d761e8e56acd299ad1149e5e2ae89139a6445bf36140" },
                { "kk", "ef799d8a761aeb6f92d25976f3c14d82db48aaa7f83c0afd21ffbc9d5b41a6f8b5003697d1a9510191ade00757929117402afa934cd35a659a649a41776804a1" },
                { "km", "6dda001f5a5d426a60d16bad24b7f7a25a18116be46e8ded2da73ecb692e7b314db69b68a390bf318ba1380741b395ce88f75ed23e28b0eb12c52eeb461d5e67" },
                { "kn", "dff03c94afd68be53ae56dfc10ef4efdcf04742824ef7c973cb81cb9ed050664178dffce124e706b9cbc7cc8f9b41a92921ea91b8b7ed7c6ce05267a760c05e5" },
                { "ko", "4b6ff919612377ffd32d1a9a326d6679d3ff1bb8dbeb9d786061917146be0fe7f170fb0b043092e9b5c5ef86caf035e7ca1c28304524c1650da030bf96afadce" },
                { "lij", "e8b6625d383d737fe9b0043945e0bb496b3e2e0f97ee7a2f97f72a3ad0bd95d79328ebd32af4bbfb6f216ba4bd722fbe42680b082aac8990ebf15c15192a1b94" },
                { "lt", "de092729f87a2d3ab3609b9892a86edc7111c47eeab3793dd7122721b76fcf99061f6bffd805ba16fee214adaf781f1727308cbaa82a70fc4d2fad4da825da9d" },
                { "lv", "c441cdb16730b3e31be2c993a484c68fb6a0210fc1943a36c0595dd96cbef0c382e31726215105b3f5819e4e7bfbd6d09f8308a20d95cc35d3fd135528726519" },
                { "mk", "b332b38be837f08fae1aa58beee21791556ab3e3bc5203d028dd33fc544409eaf3e6d8244a63f754e0bb1c8d850e6a55df72291829aaeb53b87cdefb1c88355b" },
                { "mr", "11de942c065577c1745ed43960f76d84ffdb238cf60fc64a35b3f3e5edbcbee89d16482b3f17c87b6738108da10af869c0f7ff7d3098e258558e69e004f31c3d" },
                { "ms", "a5745ce6bc6462fadd02631daa0f4610a0199c05f9f03f3c1dd64c2d278bd8b2d960ef2e8b0bbedfbfdfdfd3512ded5b30b21fe34c8b8d849da104a7fad6b377" },
                { "my", "801e30cc9e5f15d30c388942ddb258a40542a63cba84d0355f5b076abd0655a249a5f2647254e55671e0b2a38e38f51336b56c25a90b473bdf6d15656dc74a57" },
                { "nb-NO", "2ae1a5ede96f75f1f8826cdd3eaae6fe103d159644c55f2d35835254e3d6b60d996425755a283012860c7ae56f6b69ff17442263cde8efc84e797ba5ae25d29f" },
                { "ne-NP", "aa2ab71583b2ba7c96e3b46580c4b214b63de051ca7723893ca9c307009654858ddde089cfbdde033b68e33a54475b380c7284351c1e9ebc07baae6d2914fed4" },
                { "nl", "d3673b8efca6277fe7b621788053b471966b2bd299074b7c4518300a0b1e02e09aa80acbcb7169b46a6e14a4bb84850ba254733da827c5bf10a2bfd346bf926a" },
                { "nn-NO", "9ea5df89da0df184a7eec3480a8261f7edc1c7d6d3eff1bb695d488185a1dacdde856cd0e72b2f276a4e1a6e4066e64d53a45c8bfb08cf514f0384a03891f79f" },
                { "oc", "61cfe6fa8959f0c9b0961bf331690496bf41369758e65861d5268f9b99ca9b46fad4f99e7c098180175fbeabecfb610741dfaadc4993c60b7647f7b074ac7293" },
                { "pa-IN", "f7f24a6f95a9d1b8a7c04cb973aa3b2378535e2e7f9ce3a950861ab46745b64ce93afece0ce9fdcdc738c9d15a07ac377fc1ef9896286ca6ebdb5d8bf7299470" },
                { "pl", "bac662a67f20c2b886200bad6ef8b4b53908de6bdf88b49a02c9e7fce69b2a58e375768001962e4c8215ceb8bbd738649d091ab2b069aec5cc570864f1997cd9" },
                { "pt-BR", "f51306f0d9caf86ee650bc527c7b5e6be61511a44a182424073257e237a3261a8a7bf66762ade8cbec43d6deb38cbf21d66bc1264ed005c803d43154681904d6" },
                { "pt-PT", "e76e8ea01b8944d90f6971cd12a42017658250242b7c7ec6579a60d83eeb9ee1fa4c5090a59ce6d3654ba9e61e72ca5f9e7a6d7b369c89fbbdabbc670ee19666" },
                { "rm", "61d1c9db8325392f30abf8380c4cd926075ced61792321447526171887cc75192b1ff1c6a93a0d7c6831f096c247ce79ef485c7a7b9d499a8938166bb67530a8" },
                { "ro", "5207e4f2e604381e91feb1e22a67ff7e2de99dea6c3bc9c3f3c01aa698921b30a07aed089bc542a198f15d5ada327a084a354d5ae9ecabb9c53672432450e7fa" },
                { "ru", "2a32f43148e5a224a57e0a5ffd5282c4b55927b2e83de98a60538b93d54274fabe830b1483e2d60f67a1997f537d62d37c191e5878bb8fad074b6b64307731e4" },
                { "sc", "3c9465186d62c7eab45bafd105b2c83f362bc0494b889b4e42ba3495067f90eb69501d63f89f1495c13b6ee9ecd3b7354f8ae9a0d8a8098794ae0e98715608fd" },
                { "sco", "66c91e9fa4eebdf6dc0a73e20ad07ad5a02ac65140092deaeca81741777182544b59543bb18c56f82f95f4679c896bf0f12cb8938d0d1a346b223bd50d99b4e4" },
                { "si", "e5e00d386083225d0d5486620d0d5ab8336804a60699d6e0415fc1c360241b87d5a638dd90793b303ae3a34f3bc02993a5f565e74be8c6a711ab264e599597e1" },
                { "sk", "81b378c16e6d0abc10711daae110332c7c735ff613bd897ecbc9d6c8171f4a752d05e9c876f32372fb077f030b43c57ee8b940e4beb945f9fcc7d1053660eba5" },
                { "sl", "aa006d568905174e8c6b3f80abae3a2f22080e5d44c30f9d24f716225fd646672a19de897715a59277ae9dcd42d39d8a84ff5c37faa4bcd4c7e156ff76d2aad5" },
                { "son", "6955d981369bdb336bfb6d4e05802c3f8b1ff45be96caa3a2967c0a9657f7ee2fd974fc5bc1b285de0fe2227d837bc45b668af36deab1f89be62592ba3f4d753" },
                { "sq", "b1aed2a747f3fdde3e7aec105c24e0821a11b56785071f48ab6e9228949fa94d3d7080038e3aeeff12199ad7b612991eb4042031a1c1429d7af42b980dc4e335" },
                { "sr", "af9a482cc2d1ee7a709ce1eb00e6c07fbadbd37833530e992820855b337a637118faa14d510bfb4bd04d33063ec413604024e0816aabf8625186138b0a79f519" },
                { "sv-SE", "c3cd062b64d4719baa3c583ae382dda8f0540a2ee5eb97dd54b1084a0ae5e8fe1ea0c4723e16f7ef4a99d396cbb19a634f8e9261887e28c30b4d9d47c0275a9c" },
                { "szl", "21f74e2fb27a8a7372b57466e6f98f686e280df3836fa0274650aef1291b0b5021a318bc638b720402f0b2b75084031effc29b5aa8bf70fdbd49a028bade2c48" },
                { "ta", "fd3512d9d6ef1bf01999e7bc41f645d551a2b2bc5ba380caf7e5b8e25079397ec2e158077a976b39de0df8c2015aad8357b9570bde9e432a7114ef718a4d83b6" },
                { "te", "cdcd0d35cc291556593310ba3424db78a4e551f711c157c8c3eac269af82b375222f63c1920dca882b207fe502811873edfd13daf37dea95e3012a9c83bb422e" },
                { "tg", "35ffd5f46827eedc5a6e9c492ddd39ef43cbd84077c823a64e62cd6c0e18e81783f122cc1de560fa562ba32298bad33979d01bc114a022b7ffa46b4d93c91209" },
                { "th", "bbf194651198b5455d3eba797bc2f533b5760230e6c660d49172d4a5ee3985055e62d4b658bcf43118e0d44900268ee72a4ea8b66addef3460dd81c695a378c5" },
                { "tl", "6cce7e7b49ff9eb680a634329024e1eef330d63cda1cf194180bed6db9719cb955d6a54788f1a4c7af02318bbc0b70f4ddcf24230b29133d6320ccf1f82040e5" },
                { "tr", "9301bb50ab79b9505ee446f91a9c48f6cd032e66c3f9fbc8442719be76f39c19ea23a9b7f0d5e0bad7ad09a40319243f632dc00452dcd2b50b9c738d2da90c23" },
                { "trs", "b50e80af6567ce0711586771e1aa620783d7b0f10e137c93fbdff14682de73f130069d7cca5edcc982bbcb8592a08475b1f375599fb276967f315e2a875b83e0" },
                { "uk", "be1d28c6d74f511067b46984097c58ba7cc2c03b9170c1579bbd88599bfcc82f32c1a9cac5c8f8ef34d00a39012c6080706738f92cd3132e8b93d70330d7c9cf" },
                { "ur", "dabda9482edcba3fadc761c0570f38af18f41fa165411bcddb9219c0becd60ae72676f01b05a0c289a3a197e1ccb9d72668bba1b1d0bf213c4bca981eb685b4e" },
                { "uz", "3e5399dce93c8b24eb44ca578665643679615da681ed9504f88137c43f55202b7f18f816b9a1b19b3e899f468b733427b0e47735013956c8c3b950f91e279831" },
                { "vi", "29c89cf56e970949125dba9c7b4bfb4cbf2167b207e47a95d151f6927c77d8aba387c223e1f82713676cbc9e563c751915a904355d55092e12f55beb49c2597b" },
                { "xh", "30b837978ed53bbf15a721c3e28198911b1c63fccea4ae01c22f8d8486ad487c2a70c0e6f4c3600fc5946ab97ff036f6cecbf8f822abd255781f30770253540c" },
                { "zh-CN", "d55f0045b9dfffc1bc6ae20631e326ca7be53d315147b40dc10951ac03b58b6ee60eb59811faf2856af5644d69606501f5ce64c69a4ce7122634b35a51b3bd59" },
                { "zh-TW", "24c18a7a2770de0b6a2809a423a401566ac38a6f5105bd8fe88d135256b6f7246fccebfbb81592c629a9c52c217cfba9f3d4a6f72968e8c17a72ebf38fe8a048" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "089aa53c8a24425e389a716732de528631839e6b0f67fe9a5d37e296645b2a9adcba046257cbe5df34439bbed6374e5ca4d00e12c4e4672f1884d5bcecd7b81b" },
                { "af", "07ebe00368766eda434d6e42d20dfd51a0944231af8a777c467c5944bcf36c8037a81f54f63eeb518ac119e8711ac7ec3a5d0506222611c7ee2492988e629a26" },
                { "an", "2ef4b09530d759a27c631a6520bd7cc07d14c570148ea62306b82ab102c56206d195f555f14e30bcfacb0f9a0694ab1834930561b2c02b6c83653af0c6195df2" },
                { "ar", "66e365006815081cf2ecaa5ff2831681eec3dfffe72188325d5f35864821c5722f7cd1a5fa14a665eb1847ddfc70a2b9b146b6ec94f9ab1b8a53bd8b35f8971e" },
                { "ast", "2006c9bd270c37cd2709ceef62a56681044502059c5a61798f11443dade70fceb401e7fa98a0d8a210bd533d834a96ec83ce43f619929a9769477d9c90a4c1c3" },
                { "az", "372fa0cb8eaf5bb99d6b25bd813fe51a6dc1e50058815caf7fa035ec3cf3753c225cd10195c8096db420c1de3287a9c644c15599fd7426c50bbf0097b9083fda" },
                { "be", "1b9c066b51dfd69a8f528341f6f100ae2856d3d34d49f2cbb20bb48aa92bfd79cf4a81f944c9a9cb1da6290c0997cb35937fe39df5609c82d64fe87ac0fbed85" },
                { "bg", "3bdd4f6f1f9a4e807e83ec5a877a0e821b7b319e11a8322bab81c2998b9306554efb136be3c38f4c2815398ea6e21624aa0bbf6ed96ea98ff965fa32e46b334b" },
                { "bn", "ce89b925b5e101ef2b1001c6e780a732710f457876481090ec3a3e50948b7ae623148f7b577a01eb92f506c7da52327c39aa06588f38ed4d5a19bc9e7c0be3eb" },
                { "br", "e026e0d5346c420372bee79c87af45ca9f7b4adb0359538e8bb0d7b109f0b767ec1490ca94c5db36e1209b078bec336863496130dd4f1944043feb937c505c66" },
                { "bs", "8a960bea8e46298ce091a02d527ded82720f65c08def1f127b19f486aa52cbbb56a26191a7f9ee56ae3bc0055a86b6cb15206372e4e157a6c2edde52f2522db9" },
                { "ca", "9bfca11c8c4dff83ed36a89acc58e418206d222cbb2698e8aded005f5c9dc3986e9d36965ba00045af5cefd9693df9cbaf43001ed281a901a9d6877d3d1af4e4" },
                { "cak", "e6f36d334eb47c70e55971eb4fa028641b6b8a8de78fb7adb759a03e047fb43f84f9fdfe02b8ebf2ac97b15d705bda8445d8ab22d7a21d91fa559abe9c508abb" },
                { "cs", "6d4c603ad8b237c98e6a154de456c0c329015fe5fec0756e33ae2e05604188e85f8544eead946d481c3bca274c116c72ae3cbec6b3ba25dabf18c637dd0af154" },
                { "cy", "6d691354a174f1bafb1dfe3669166417717e6af9dadc233615df2330abee81571f69206fba58fb1c81fc4c9cecd1193359f306f3764c7fb84abb325c7114b167" },
                { "da", "00a32ced66ce2b94e0baaed51c6c41157a4b03c42594de1e3e77f1675f1cd97bd1ab2b1511d592f18fe700cea1ea2852093f3596863fb8d945e1deaa447fc585" },
                { "de", "00bfbb41bc93680c09a17f1adc6411cd8f5192aeaea59e36f93ae5702a12cd8656d53b87ac2e4137b9614f1da6346c3ab3f6844fdf48493100736f85f8d57f80" },
                { "dsb", "e3de30c6b514797d1340dcb5ac8ea04569315587dde1a5614cfe3c7d48af1444cc4d8b1a1801d0e8435e5e2bd5ea08038b9a2ee0c3bcc12a21e1026edd1e0b6e" },
                { "el", "5eada10e369bb00068970c2e8442dee9a9a32b581320f6e3d1e2d1e665901cc0b5f2269bc854c84bb794dcd8fbe0baa82f30de55b22401a32c3e3edd294ad758" },
                { "en-CA", "c2a332eba58080f1cef8c9a589931fc69779ef5f89242cd14ccad0c67a5c2a36a63dce5d7d8ba6d619e1c397d60e747c991450e29a29ff3de73849b74c1ccdf6" },
                { "en-GB", "8aabfb7c756b318d29146a3e8ba2fef4e82efb7e05dc9f8d309bae3b44be1dfed292a70a2a48a98199ac6d753c1ba1df1fe6acdd925b8aef987943693aa50700" },
                { "en-US", "b112e733c922ac356b1b46ce8f4906e22dcb426560eef6324eea0d52d28ea06fb10b8a278e93b2e991fffb2e1f0df17cef93de2535b42d69f952688299f8505a" },
                { "eo", "413955de9d5a72f7bd02fd5ff1c2dc33c48f6788da25f460d82c7bd03acb61fc93a51b6dfd8e9298ed2965c7d4341e7be3f20b9e3bf5aaa90b43987f48b33e69" },
                { "es-AR", "d3039f87578599334d18bc7056ec0e9b7efe7aeb6f1bb593aa6e19dd0388365f3a50269cd1be9756a9283007e8f5eb0bee81cf6d6f0080b8b44aa65b02b666b7" },
                { "es-CL", "63004e79388faf2a0c8654c671934393407bcc9fc7d66993352ce4ba14a053caf63abe01fb0d39d21cbd48bed10a3494cdcaeda8f02d18c442d53e32e115d5a7" },
                { "es-ES", "6e9696e46c6dd6a679384864e9835208233b694a5b56fcf8c22863a36175811394c33fbb6db0846821ceac6f26f1225b18ed15773b0ed72b3dc86cb9553b72eb" },
                { "es-MX", "4c9bfabbd137199a75260205d71feaaed8b24d5f2ac249e9e5eb39829c30628bb1b4a609d65745e382cccced6330148c69e32373309d1e16ecdf348917b8e9ec" },
                { "et", "ca61fa86058afc0ce33758793c0225fe1e7f5c0819b3bdb1c1f3c49b92ca47444459a6f9101cccdd65c249fd7b4e6ac670916849008f9499d1a73491067b9d57" },
                { "eu", "702672bfa39222055468ad272ee44c27baed81f07c18799b01c11ab08e7860f5d5481b8e7d596e066f217d0f312357668229568a5b002e23ea12e9416e0b34ac" },
                { "fa", "f5774c1e12c0361da7d34a0f28c31a7376fdf693e82855a31ae389410f2a2a2ba76ae26067b38c551de66032e2d036a169f397abd0570bea0eac39ef0cff8627" },
                { "ff", "be6fb3fb736034f5d497cb0dee021562de56870a3383c8403c9857c580da448d1da9dda65b419eabedc35527c3c182dcf662c71cfc087706312a5e0e3cede187" },
                { "fi", "40bf00263f221c4a9d5e51d2f959ff919a767c2a3994c170a10acbda41f8a75078f865b4e2792a3365715aed1c94129cb8f2bf4b50a242cdb2d3bd61612a01b0" },
                { "fr", "f07f9aff848b5d3bcdf3974791084a304d4448cdbeac80ab46f2fbbf2fe1ee7273e7a3b2d0b6b3e1fe2af5f5e5ae3f802af6f651e15d9627385f6afda4cf60a4" },
                { "fur", "c4da871f2f2b2db1c938e006a994c39c1a48923a5b7eb3a242af2416cd5a32ad3721d09209d3316ead0b404bb375d28d1674a5ce753f1c36a1207604544db1d6" },
                { "fy-NL", "9c1ebb1a91995486db6f74661491c0e8d458226ff1f3f9674c2a6d294eeb460fa36600aabbb584ac1f4379283530256e600c95a177e93ea265eeac74e2c958f6" },
                { "ga-IE", "5f4ca1c5671a89f62eca0c4528ff3fc5d726eb488e401df879e2f900158094bd582c6c1f6e306e6d1a58b826383f659039d4b86c82d49f32ab39bec60bd3abaa" },
                { "gd", "49d5425f7bf4dded4c898d47cf509bdc0f18b3698e98165e706744d6003c83a4039981361c6a0c220aa633e77f4f9fda0d86bf963697423415d7c1acd428cd4f" },
                { "gl", "3ac12ef8ca4a01a6afeb310430a542ca1ad4c29a7359a7ba95a5d2c0c9db3e307f7fd41a6cdd204e6c395976d02e45e45f85cb0997720abfba8ef55ed827d8ad" },
                { "gn", "f1ce88e52448d93dc5422ca6ce2a94caa9bdd3fd10e3b78e8966a72b684449bcb2d458b8243f481b38866f4f5058361baf638dabecd5ba3223372cf5ea03fdf1" },
                { "gu-IN", "e31119d4c6b97441dc2016185df9c906346aa0f167a8974d094e2f1b11a6ba3f9703fef7148c534d0e5eb038957fde3a05e531ccd6990bf7e0a52a91f35d6e4a" },
                { "he", "243e6badb60f948cb2dd4cdbd3c25ae93f622750f001167410a067d751d5c8034084622cc23a6d2dcf2aa46178c132609570ae7c797251c22310aca0502d201a" },
                { "hi-IN", "3f1f9c7f77abf506049813ffaadaec228f8ed3ccb2fdaa43907754508c7b076728e56c95cefb036cada278aa22ac8f50f0852a115d40db152e312b4c03efb000" },
                { "hr", "d83fae71b88fdf4555709cf788a8dc00d0e90b27a024ca45a3687d6c5d6e2f72a1344110d438edf662e76d12ecfd57bf6b7a4bc5c1b69f738c9517fc12870a59" },
                { "hsb", "bdeacc2d0b1e6f5dcac5877fa9f214ce795f202e378b9dd8110089a4404ea1f83eebdc88423112b9648489059e519c8f84ee39a8f141ce3f95f9520765903cf6" },
                { "hu", "a33a3cdc3b02ad232fb719f3f2d68829a3c7beb7306df82e22413fe32618fdcc4c3373fa38aef9e897c602b6400f190a84a1ad1ffcb540fabd531f6a346ff991" },
                { "hy-AM", "df1db435e0bfc317d58034ac842d61b416085d1422dca2ee90247cd8679bdf6969aa132d68f39680204f689ddd423ad755d0c8837af54563ad2c717c515e00ee" },
                { "ia", "09c80c8889464e74ef3e83b4dce4b2fae723d70f7488f6d35bb023dbd5f65b37ce55529e5da555711a1358aa4a3e4830e852dd939f6df1cc616327b13b1cfa23" },
                { "id", "55076c35faf4134d263682d814e544d5645fba3f5bcbcda01395e43bb499f8e3df6685cc7a9c980eee577ddc9dc30eb9e58d3a4b7344baa3d76d44f295a00f57" },
                { "is", "8ad5780f01ad4ca9b8899e0ea5fb2ce087ab196c6cb61e33301e452e958119feac274e89cad721a07854e89cf001a20638b6d434fa9630aae56f8665659e6a4b" },
                { "it", "38e660435d6eef2ea086123063344a1cdae9053b1636750559f76f36e8fe15863fd40ae6e722800b3d5943b3b3ed00c95eb922aac1f19c9cc765ffa014aedea6" },
                { "ja", "be79f26d794b1aba44864fda27d400cbd6dc652d2e7bf62695d2bc3c132c378c506407050b4676e7583e3e4bd54e48ece58ff1b56b8b9fa5bf2bb859b79d4f5a" },
                { "ka", "b5d21e759417457f2db624ed957f86273a68bc98ec8312672e4e127f2034c08e2d069ebabb42296b56f707b65b4d4cffe9f376faac91fef134d95ed381525574" },
                { "kab", "cd168fae0bf42e2b08bcbc065555ffa97c8a3347f17b09efe1d789a346fc9b2877c1d4a1a9e672c707f1967bb9675e8a707b681abbe3c03c988ad7bca891d0fd" },
                { "kk", "2249538d9d1a711f734dba7b61887ce2ac6625ecb40e12e99fedf270b96b5b8d5b06a640fa99e39d0bd9203b8f974f9db607c4671dbb25e3528552552d377007" },
                { "km", "a296e76efa8742c0012628487d27d70d38564517868220583debfa013cf5b6eb8c579742d061af584070a33a00ddbb3b211a1f55239d8e09596a8243256aec89" },
                { "kn", "ed1c4cd168db7ea8aa73c30e6e48d63af73a7a7586155e7746d8b3beed173bc1cf6d975e5ac7cfbc0d2141ab305311287c209dac7f6d81085eea982009d2577b" },
                { "ko", "ec48cee2c29644990b97d4eca64fe138fdcd87884fdb0f5cb27dac23f12406f8f663b0e410d3fa7d6651f4ca997aebb53ce64ba5b5689024e3e251c2cc9e4d05" },
                { "lij", "632528babf0dedf68608180627f52856babc7ffae97b0e2a28ddd153796e7d7b3cbf8812736b4cee5776594e289dba94ba24c3ac4025de3367c8de1886d895b2" },
                { "lt", "70215f590c3e8b8bcbab8bb8508cf05d921c2907fc0858530865b61d2d0e186111f05273666785bfcdb56c0b2b3a2b4ab49d5057599252b30c4263bb7318ce47" },
                { "lv", "170b4c08d4e598a4e0d2a5a31f148397d54abb2772ccb2b1978a99011795d7c94483195191fc3927ed510b0f3523c064ec07001ad381133654f9ea7d330d22ed" },
                { "mk", "bdb41ae03cdddd42a74e165d86fcd38c03568a384e096f95d60c84b42e01f4dce5f3516e52e929fe416921dc3f6fba7537015d7d57ba19923ac8e4b4f1cfc457" },
                { "mr", "4d94cb8e19dac51b259c03f2433405e9f27294db1cbbe613ac50a9a3c528c15c11113612498faccb5a12aa7a1e98e8070c6926c47e1e9c522eb49bfb845c4c04" },
                { "ms", "8f3033db0a054e2e6bd21152fc66dc1b68ce16e4d1bb1b298a3a81385b3ee787b3bc4547ced6622495f0feed38678f9e40eab738c204e3c205734878cccec9dd" },
                { "my", "0a7003e34c119d37c34ab790a11c2319a51e36c3a2747d5a9b4deb062dfd9864dfa32766423715784682ffd425d4426a543a21b2f5227fff3f44e19915846348" },
                { "nb-NO", "81f0dc201fff06879afe6e5e7f1c14ab3359497c8be365acde25d52bc1ae3a9dc970e3ca3233def61cc29525e111635e9b2894f4d672a8a9271287cf0cdb6234" },
                { "ne-NP", "887249e7781edb7ae4554aff538f57c307d2b787d9047bcbaebca92222fa6d1726777f103f79735a1499abea6e196603f4015745eb65f4dc2c89f1279381a78c" },
                { "nl", "3e0b3a09a0092f31b030e1febd89c0eee95c6bb5ddf6690fa59ff10c23a2849f5348b34f82dd5546375f351f6b2d08ff2cc180f8000261216111140ae7cd9300" },
                { "nn-NO", "729e7ffeb240c0d5d4499f6cea223ac862e5a89168d486890b6cd95971a85963932091bbd08e86e167834c083b9de733bbd677e89372e0f7c9dc8014d663a9df" },
                { "oc", "a35a2bc9f4b467008c83bc42b7ddebda519cc3036a7b3ed637c978920de0e84985b4c2816965e5078761024fc39937ae2ef5b38b354d18147fc5001b1afe4db2" },
                { "pa-IN", "3823267fe3b6eb5c5d3a08224ba0f49b61643af34937a14d741a7e1524ba8e16b90cf85231b2c2aeb08b31c743d691dfe829d744998d7e6e1e15af8f3d696a74" },
                { "pl", "32684d8a93722d6b69cff06436e70e1754e074e17b28d718be7af6d60d17720a1705d76c89a418b070f832fecd175db6ec7acab035ddf3296ee89a6bee500779" },
                { "pt-BR", "cce9f34a8f09695dd21a13af097ceae7340c25313a345d46741f3d457bd282e3a772b6311c34c3aefb886dfa101df18574a1cf3c6b038877861835b3061c97cf" },
                { "pt-PT", "1f02ecc3426511d4ca1887c640c40fc81644aa10397dfb2d0d901a50b940b243c66705288ca4896838ed6e22bcae2181e5c5287f55e22763c8fd9de801158ca1" },
                { "rm", "12a1ae46ade76149e4b811ecc1a87d4ef31ace167e691351d3193cd965eaf6516ba070a1c5ccaeda59bdcdeec19b70c8930d706c76080c305b13c4854952fe8e" },
                { "ro", "af43741491413dc62c889272c5016547402908f4c682ae383aada1af6cb36af8d792ba2b72bd4987826de18da750eb8f638b316adaac26e4235ee0ae3e6882cd" },
                { "ru", "29fb7e30e2271e43e4b0c5f1f55db972f185736ee7a9daedfbd5c267258316f4640b1d349fc21af5e3c1cc113e696206db7231cc9bdbc3c4be91ea36e32a560e" },
                { "sc", "4dd7ec0eea9bf1525b057bf5d8387d842563d3b7150b55c69843ca7bdbcc1f721009dce518edcce0418d3eab9d742be4c1b79e2b9d7ef6c1d6fd3fcaaae0a92d" },
                { "sco", "0d902c4e693f53d05fa0fa438601a6bc7aaf199bb9231859e7a486e45086faf8b5cc2b38519c1c0bfd5b5ab54897cfc79aca27ecf11895f0cb1883c1e80b12f0" },
                { "si", "2b1c472f867d825ee2a11f8933e58e9738dfe984e86a0294a74e8a1c0f6e6e3bf08a2fb408306e333882fbd0ffdf3043b6508919d4c4a17352138b648706a088" },
                { "sk", "b316694edffa33619dc5d3258ac6cfa5d77f15ddcf53843cfdc9aa46ffc4d7124ab31b4cec76359de39ebcf7b8c005b0408e83e21fef88060477c51a5d6f4dad" },
                { "sl", "152275f89c0e027e83267b0724f2c066616f2fd6efa1042b70689aff047abdd1aa7905223fccefb654dc77f2b48d39a73e146e56978711e7a391f9a6082f6028" },
                { "son", "c575e4dcaff9cb371a5ef7ad275d11aa136f0a20149ee7a48026bb05203f79b0e3f9952aac08d85fc0849a89d4ecd3d17fdbcabc9b2964305c20a09f8da66416" },
                { "sq", "57567e1b7a10bca025e2cc7b213f45e28d9e0b3577e81a099a1a822f9e1002647facd1d28782aeb7fb3283f97cd55a542d29c99b404ce87295de4aa5bc9d6af6" },
                { "sr", "ede182112b36aa622c255960d6c03c339e9452c218d9af09139d1c66a5ba937b87ffa9f6a0b2b25565323dc4e51c2dd2c5eb0eed73bddc62329b0813bcd5fbb0" },
                { "sv-SE", "efa5f3e4c7e1736f0ff5b5e9ff23650e9e270f2eb016cc2bd64f19a3948852206c6dd53c02db208ebdce51657d20c68885298a19b92fbaddaf0b3bd974a3d4e9" },
                { "szl", "a5b1fb651f2a9cb5d5fbbb8e7ab988242b38e75baec5f0e86a880d41a95e77c1bc2d328a236f20f374562edffc166eac496a2f8de705cb6880f343c457a6b441" },
                { "ta", "2b2391d2859da8d84714c745a27445013ed72f410710fc8957badc937061d22e73085f6c8750d4d4f1f5f19a051156e07d5eb1d1c51f20e1fef0fd75181d1c02" },
                { "te", "5919df80bf576b8831f17e51344f1a260a63caefaa5ada2e351d64dc4914493537425e0e93280dabac8c20611587b1425b18b133b44c0a6fe70b23ca5122d493" },
                { "tg", "619ee46435b4d5eb343d30a2730f87976908ca9f6bea4fa8057bdb8c18c16dfcd4ab72a97e17107326419698df32c7dee48ba046dfb726e82fbbd13a2f0a0996" },
                { "th", "264fb8c94d4df0089cfebb283fe169696964aa530faad1da99a4d604c0cbd587b3340094b20a14ba2d88ad8c774b0995aa1cb36f2a9fef2e5b8f6219ebb21a3d" },
                { "tl", "20aa8b3322e84b37ea8d266268fefd0c430a4928515dced8e7ab0ce1ceb62413b17930b0542fa049ae20bd14970a69e2fa5929e008d58fe751201c08bf0f7d43" },
                { "tr", "b26324f4421f41e9ad2466f20b5f064147b377e48813067c9fa27b0856a52687c65ec13bd1335248a7d5645dc57b72a4e11758638587d42f70cc32d06f1c9f50" },
                { "trs", "82d59f0118f9d0577395a126452655083278e2fd35b95965718053e6f570c558cf29707e9613a03512e03a8d3d499fcdf0f12ebc6cda44480742cbae5ae4d623" },
                { "uk", "40611f4b7964252039b633be814e35b345656c0808282be680c04141d8f192ae6f10565e2259f25d81f18ac535c708b627ba738e0bd6513ac1d53198e991c8c6" },
                { "ur", "b6dd186f9a5b8b3e3635c08f1e9762dd11b3169b516e17914ef85dd0531645be3c8a9fc95b3c6f7053d214938ff46fee16e803e74d2c49d70a4f8c4444bd41f2" },
                { "uz", "cdc48a7d78ff4e863843110ae665a808565b7d8da107ca0bcb1c8cfe3781f05fe04fb6d3947d54a35adfa3adcceba5d2b3f68fb16ef157c5e48719ee536c386d" },
                { "vi", "96dd13bbc9ebd8e4a288613c6c89a6b3cdc2f2436338d04f2b79e090237aa1c108969c40067a3febd12d605f263fe3e05871b09b89365e0baea28f9a7b5c5d51" },
                { "xh", "4465f3d602e1625e0bae731c5a28319a696cd1ed02d87c7f2ee0481fdb580c8b837aa5774a80c76320a239f28711489080da1f3d3627bb75cf0d40a97dbedd7e" },
                { "zh-CN", "fbe1fb670097e0c50a9b6cb4e1b7e3eeaa93fb904a203b81c3b5580b25b46bb2e64839cb718bf4f24bf99b663ca4a6444a6a33b1229244c4d75b7f0995f63dea" },
                { "zh-TW", "734567494c6f9cf0f749c1865287d7356813607644e64d61b34952742e1c060e80a5c8c3c9c640019c29dfb84a9861427f8b67ad2998413acd57fad1ce44948c" }
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
