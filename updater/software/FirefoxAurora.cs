﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        private const string currentVersion = "137.0b1";


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
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "734438c6296ba18a30f2d5a6aa23e6c860650455e328ab2cea5f99e015725178502f9d9c0c093af6e1c661b72e6662d33a59b74dd04a2424f804cb088e6f8be4" },
                { "af", "aecccc2128b8c32cc531ea3b0762d30804d33920fcf703e70d83420e57895b497586239108f130902c000b31c681df9a62fda4980605b091e3be6c14aec79ee4" },
                { "an", "2df525ad6d48ac54fe656cef69989d45715d8daf2bf8ef3baeca3f506049ce5b430f73b5a1e9fa376103b8d742c1301937d7358b31fe3b432621e29da7b617d5" },
                { "ar", "272e533f6d7e2fae8ef375f7620bf2b0e9c502bb6702c366396ed3671ea68c32e7f8b2fdcdb219ca5d31ef01730d88c6879ee167b59f12c912c6676faabfce32" },
                { "ast", "6e4ee33b9022791d93e3df4cd2eabb551fe90ece0ee3e708e5026b8f851e255837e030dc14e7ebf854b731cccd441c9b1be1c765fd9ff6bbc767c0976552a407" },
                { "az", "921f80ba4991f34b2b7e7724e567e48ad1eccd180206d6009b17ab8f673488d6a1a047390e1ed36f582eeb765a5b34ecf9d45506f89c6949a10b2da74b63a437" },
                { "be", "d941219def70685aa32bf17dbbfd90ae10d2b1301e75cc9f35c5a6e96ec91bc9dd3555a7d7e386c68d2385a3b748c992fe8cfc59ef7c7176be5a8c03b1a4219d" },
                { "bg", "6a3a30d0fe4023736743199949b67863f9817acdc63e47c44deeba350a14e880c3b9a3db1085bf25e642833ec0177f4783628a041c5012ada07687065e1cfb3a" },
                { "bn", "cf51ea9d781e02a99de9f410bfb8022522fb97741d24a9cb33b3e34cfd23991d952dc55a97442e61e4cc3fc0ba20191baac50bc841dbeb511ce4a9620027b11d" },
                { "br", "a3e9a221c1107f2f0986e463d07c75ffaee6062548eb4cc3420ccb12987d5c7deaaa2654c025a0cba4054775be7770cb03dfd41dbe1fe707a5d930f8a644d7ea" },
                { "bs", "f57c41bffe66cb2e26296b1a304cc85bb1c3bfc017fcc905e4a3b092be73dc0e3de8224838c49b45be9238bebe7e202a434053ff8369e5d23a5892a6d371c3ba" },
                { "ca", "a9044fa680a2bd18ce5c90c6d698dbfb781e9bb2957e573c061e145c5ba94dd021b2144025c8401cb2ce0a24f5815ba465a743a9d04f61ed385f77e8d66d5253" },
                { "cak", "16aff2f45a3d6bc0f87a88fca97fa8cf13d40b6fb27d3c717f10426bc7bd558a98a1b0729bfce0736ff8a914ef9e18026340af61965e4918494c16628886b687" },
                { "cs", "5ce5d49d21b51f114ef5bfc0521c056edadb0e8babe9a16f092f9d636b6041e8e1b7a0ef3b369732fedf286f90fd94fbf73987100b16cea6b632ef0ff0a663b2" },
                { "cy", "1a686b334b413660e8c1e15c0ebc7489fb15556d16ea9429065c4245c6013c14f4fd11fc430393eb13530161de3d03113a9fcb61d24fcd21327883d5ebb72490" },
                { "da", "c8be9e19879c8e7967b57c6c55e94dd2540447a5487e351bbbb2ac2c70b846ef6e7b40b6f1561364f3067321d04c920f00b76835b34c49cacbc2bf9fb16c12c7" },
                { "de", "c04c95f1942de71108cc4904e332f90bab0d85bef216db308a8fba09a449db647fb91063a93a2e46ebf60afaa5a1f3f550a1631c8b4af3ed91eb280004a03def" },
                { "dsb", "911db174ecb9ce1143df0f27b5514c72d0ebb2a1661e8d1f70c7041c830fefcbfe25e6d1d4ac10725de2ea44821abe326938aa45d438b6997de0607d63d2545d" },
                { "el", "b4174b30362a78f8c3c73e498eaf42188b6464815060ff2e4056e123dac56d879ba47c847d70a3a751a5376a0f8a214aa666d3b2f9e8dd8851e7cba868f7fc26" },
                { "en-CA", "59cd88f8a1cf88997ded82125b049017d2a691d76529c31d06d218b44d41307871c7ff6f25a6385067951effcf68d3142077f949047658601bae6a74a4a6bc1d" },
                { "en-GB", "6adba451a8defaaad27f74f805e70a9b9551e908e94f337d206ce379d9aba167f05abd9c9073c6c89d57287c4d1271b1dccf623650f0ad4cdd0716b5f6d0c7f0" },
                { "en-US", "d61e59d2bb21c44e4273dfaa682c909ddb4d601a2e3f6a769c2dc3322a55d8aaf8fc4a7c066901371fa7d12999a411c8903054155fec2f6e674e401ba1d3a5da" },
                { "eo", "109debba7c7bc5f05e6c6fd9ae74e363d2d9414d29ea3969c22ef286ea55e68cf90c052d06e32374b042e77369a688851dea027182bc412c50dd43f02f28e9c3" },
                { "es-AR", "814c36d614dd10876ff34ddd7737f0d792b75715c739e514569152469a70922f94d07f545b447e9c25b5ada4f032427c2aeffbb39c921a921c91e4e87856b569" },
                { "es-CL", "6b8463740da4593fc105430516d0cb2da9915b3d178f574d0fc62e2b38780bb9cb39287bbb170726df3cbb51f7d50949516f3887c0b6b7d882cbc3389dad4ab5" },
                { "es-ES", "7833029289cdd4bacbbb07dd7da5984e902ac2b7ce8f913f1c573996e9fddaec982760037747a670c82f4d9aa5aad903c412d0de39f3de528d58a0210f37d588" },
                { "es-MX", "d9ae421c9bb43e41d8a99904048351e77fa74a333faffe6af1c1219ba6bd26c57f060a8dc51e835b34f64374ad8103cc6b1c21fd993b22d11daf7d6d810667b1" },
                { "et", "f844f721a6ae10a98cea3d9bb62c65e7c223f84a0a84df3960b4b1e3f3935ca4811bae6460bfe368fe5f65a76de1bd8e047aef74301ef034782d89b841441dd9" },
                { "eu", "9da08315ebe3c3a2cde5411c1e3db37b7dcbdd68325401d5009c14db1f43baebddce7c4af48351a3c119ed96982e5e0d3f9d027073eedfb5571fe225acd1cdae" },
                { "fa", "c8e3555093139ef28ea6b1ffc14174507e469017fbbc481124731b334bea56c5747c83075bff276bc49a1d86530e59602d94d47f19972f355350f00044d0e336" },
                { "ff", "d3d4bb391ed29dbef532a5369540663c708bd19bf7127b68e91f54c337e023e714827193f6202067ba5067df3e554cc015e1b1c8058128a78a4eb8f091bd7180" },
                { "fi", "767e118235a6176b981b773c6202ad2e151116144e5a520b5b7681434ce634e6c8bcb2c5ae338fe587a8995b1534c053cb135f5264d0001ee204de195a419148" },
                { "fr", "27fe72d82270dd5d54fa1c8fccbbbac9a8b77fb78847cca7b6a300e9a82bdc0bc760be4c13f22936499b835da22c90a8375dbf47aac2bfa1f54ffb9a02c82843" },
                { "fur", "3cbf2fc4119b15338f29cdeb2b521a9b58ca92a308747be659a1552093f5ec0ee8effd95e403caedbf905e8b6f1d1dddb794d6570ad268dfa1956d1ebe2b14de" },
                { "fy-NL", "989baa0ad2b046282dd89151743ffa4660956a0ea17125fbd1df717fa5282675c0656fb525725492cad2e013cedd422e57cd38a74043c1362f667d1c273cd93f" },
                { "ga-IE", "1cbdb54fb6a85d0429e63f045fc5987880a6e044ac75fd2b94baaacdfcbae06bfba9a58cb5b4dca9723a1af5cf2f302d66b5cef6a27790f763750076252626ff" },
                { "gd", "bf27a263ccf53c4af64b23d4b6d5cc7a0a6572a411e0bd5bd04c3342bf0e9b7ca90c7a17d78ed533e065edb33eacfc343793f6026d266bad36af587945753990" },
                { "gl", "8219017b52214946c8401a90859b6354b1f6cafb70f1418c15aad94a66fb56ab533fc43cce0c89e82a9f4eb39c64acc455c63afe924f8b6f130e66eed57cdb55" },
                { "gn", "60daa2d9e4f8abdc4169d172a6a5f63e137dffa350d5367be6814de31f240000e951e278f8608915aa23c4e2cee48bf69d38a1eac3629032dd5217bd42b57167" },
                { "gu-IN", "c9b6c4fd5b85fb11299fbf59339dc8e9457b7d66198f400e1018fd005b445bd682feea66a26ea63881f5338b49b950f2eb7019aeebd47c06c3fb94ebac1faa5a" },
                { "he", "2fe71f05770d383c5440b82a4cbb0168b5be62496b2ad823f6a50a32e7ae993bb655ab958a0222f211718c624e222c77f551112b786707e599987746b54d71d3" },
                { "hi-IN", "8ca2b94afd2884d9d5d058dd794a241c30cc31fa12d6be9d1b3e0f5a54fe442d68a61a0a0b34c16c7a233ff25293f28e123650011a5713e94d962f22db3675dc" },
                { "hr", "6474678e69ff15b3489d77467bbc568edbc6f7592b3a3cd8eff710acb0e1dbd0d72032836564d130003d714e656564be83de3ba2bbc00555c357bb5758a328e0" },
                { "hsb", "9d7e528f155ed8d92f5f3b724da4f4094727800b5f3cb7d59e4d04c2497d0ce1711bba49d59db3ab09288e8d3bfe8b551b0f26dbc94241108981460f46143242" },
                { "hu", "9b2ba996d0eff7ac2e972e023e6e27551f88e4cb9ed73f4de816f7be8d9b15263086adf707b5e044a5fd85735ff93c0b6da5de386fc3cd6401bdc92889d2fb25" },
                { "hy-AM", "aea022d8f36922650b64cb746e67b1fa76f582ba0889455b37edb70f67e9e62595344e83f368393f68497d93cab4e40ed410fd3999f1878a7d85e719fac81501" },
                { "ia", "db5852cc48c442e2280386631f8aaaed2d4294496c530371bcf56d6daea92bf5d981b9ebc502fd26686a572d688073235b185131adf0256e6612735740ed6f38" },
                { "id", "d40a6ac69426156062d87f43e18930dbb3301104f9d93c0cf37a4eb3528cac940765710e99113d3824470ec515b8b6cb7fc2f8e40a9808a09707e89f51fdf987" },
                { "is", "5b420cab29202f4cd30398f39d0557b84d76615ac77621ec82efe5902d5b43ba87003d3417278b9759cffb4c4cf215b59619104b3d0a1cf06b1ba0d316fc5e82" },
                { "it", "eba04ab166d45dceaaffe71c1eb5ba197e68b8197cd553bb6174bea83057e4f2f61745c7940dcc2c648999ca773566cec5e026d8bfcf42e3bc8c035eaa85b0cc" },
                { "ja", "9bdd5f049e019e3a1944fe5322eec73f2cd9cfd337d9d56a368148ed03e7ad16a365b05312febfe277f16feb58df3115bca8622d7686a1cdd845c2c06b333239" },
                { "ka", "819b4275b55fd3a6f230f83c4c46698c6de42bafb59124ba827c59128a5b1e13b68e0e2cec34f6baebf747eced2b8268443162d57da034322960efa4cf5e2fe8" },
                { "kab", "6ee7a126a493a553145c9826c49ccd753fb48a7f46e76112442b88995e2b5ac1c0b06c4c326206fcf9a3a1ef229a751c98e3ed28dccfefe83cb38616b599034c" },
                { "kk", "632b359e7ce5ae0f2cea1ec7d44a541ee646d59af84b379314441f3f90ee3d55d26214d37b00fb893844b23d1d6d95340f7ea9df35d23d3f6e629504e792bf3b" },
                { "km", "a3c61107b9e99737ecbaacd7a8914684e208458de0606196a942407273f24fc9ef6ef5bda42182707e2f10359e98a558d44c8d6cd079226ccb295755b30d511b" },
                { "kn", "10340015b6c280df20d6e132528c91e1884aca519a883b74b588ee46181521674317e1603e9e9ec44e1d7b8b0e2a4ab9e6135f6c314ce7f4f7800e72b548c648" },
                { "ko", "62a4e0afcd196a84d72f5ed29498aa7586fc37a0622edf3b7e59cded590b00dc3b3952acf0e4c2a2f6b034107d1c530bb1b1f0255984d7e1fb818e12263a0ea5" },
                { "lij", "3c6165d9cfab0b2569ee672279c4b8f0f3184fe3cf5b4522e41960591f6bc02ad66bd31eb2bb6f32fd8d26e3c3c21c3e367845da7520d94cbd4be9ee7843ee31" },
                { "lt", "0d1d56d4f4606b79e40c3649835240b57f29e17e7b84477fda31888c0f708a9fec74601468e0dfbcda3ef7bbca91e201ed92bbcf13266b7476199d85f26a60a4" },
                { "lv", "faf9e4632f291c207595d5225c33b35bbbf4554a51aa6d26a7d2c31040279737161f9ec69c824ce785e21f0fbb9b3863ee068b937de75996d2787a594c27fcb6" },
                { "mk", "e85f9e2e298e87c588fefb964b1dec828de0924dd1c275323c1f271518b449e746a859b1e0f1bb22a9c1b8292938760da49af068a3f551d00ec4532e22008249" },
                { "mr", "879a2e81e3bbe6316e9ff9863f269a54333ad0c4fdb89ed34c4d59221fb6271f0a8aac7ee9a97dd6c1580f4ba8df742926188cf54ee50b46881cda69edcce459" },
                { "ms", "7a9211419cd4e4563b5f6ad5c437ce701ce5c3860d5ddeb3e8a2eaa582286322353cc215dd162fa11e05966143e103117f9a1470427bc0f2de9e721e48a461a7" },
                { "my", "92343c1790420431a9b61f6b3720e6e0f4d7f27b9baab281d6e009153829ab17cedaa38d62e910cf28b394572049e2c413b4aa3d4bfa404e48590de3a6896b8a" },
                { "nb-NO", "8a25765fccfda46fda9d40e658ad49a5fe1c282761cc80420b9988821a079c428a27d51de3bf9d178564f5ae8e50e82bbc071ca58deda7589309e200e7f108f9" },
                { "ne-NP", "8d2ddbe1f258fd35830373155bd08abff83a4024bb817285417b4f5864f305ec816a0b0f11292bd2707e1b8502893cdfed8a5ceaafc229927f5bc28eb688f11f" },
                { "nl", "e105cfe2d752ce2b6992f6f0815a3226d8c401de98aa670cb3dc77aab9b8cd4028e3efbcad83697721c9cebc2e87c2d5045ec6135d678958fd71da26493101ce" },
                { "nn-NO", "1aa6238b864fe441e0ee7666a06771e4ab69b6494902195f66316ef735dee0921ddd6872cab5c20bc44038b5a06fa072acf3c8400d4309ab0b0382660faea9ab" },
                { "oc", "f50951e1e35f7224ac410a26c4469b4ec987f2682701de48f2a6ddb1e465996f69a47f81ede496b39f5d9b593ca7befde66bcd78a5b33d2f401da037c057137d" },
                { "pa-IN", "2ef71d9391925c8393950f73f220e8b525146383b9b844ad79044b21281e2e31f50828bffa612fe899669566c72776613b34c14992cff75b5f2b236bfec80514" },
                { "pl", "954e598452b3075b64c8208ed08aca5a129576a795737368c2163cc26f3d34b9128f0247e0663c7a028e03be23e85a5b6d6d7c4919531a94b06b1453a6c9eb9c" },
                { "pt-BR", "fc32efdaf0dab1faef64a46b97f103a750fa7509b75c699cc3d066695a4a880fe4d5987bc2719affd9b337309ae22a0a2d86eaa0cee7d0d148795c77a93fa3f4" },
                { "pt-PT", "5486a2cf629ce3dc949606685e48a7d6cd4136615c2ac30193165c9e0c048c1dfa4e897d1e87b2295dd7399a1a4cdcb290f4e798ffb6607538c31d59a199b3ac" },
                { "rm", "f1b85d9e0bc4e38147a4518df81ebd84d695507a0e1a17e16960ef0a003cdf2d68a985a108d36b18dcbe3b58e377cad71573c94171bec244584efbecb9440afa" },
                { "ro", "23defafb181d0cba280e02bab2934a6569ccdbcb457d9c7e7d5c29ba9e1b4296c6fe6ef7bbe234f9786821f6d45241c779377cf911c7ee5a199ce9fa4f8555d2" },
                { "ru", "78cf92b54d3fa9894f4a7a7ba84a9e308315eab87044ca1f6a74f62d2ff0af9ee120c0ad0c22ce4a4f2b9669f9e2854549b26403edf303c8330f34da675378f0" },
                { "sat", "4e8dea14999088cf9d66c06fe86b0c9a1099c9dd21494b9c24a6957410abffe0232f8efce11ca3836c5690466b902422e2b3d27b690cf3bb2a808da7fdbcb735" },
                { "sc", "8f1da1db0f274eb63e8a1fedda901e58a1daf440f21f9bc218aef9762e050c701719139b12769c5adea3a36e29187c56954426f029c83d145ac311f4ba092829" },
                { "sco", "6f9bc3d3b22e94e14239b0f1910c1633f82304f43136a0180ce361994d83ec21edfba29bbe5d474af35bae5b92b583a944a96a2be9e301837092b89f7d656bf9" },
                { "si", "6bdf7b972ae99523861de06764a5a3874c31bfe50ff4d4257a74768987391cd12f34c45d6416ec06d860fc5dd865d287d0aa11431cf525600b4c7ed839232534" },
                { "sk", "967319571b1ac0d878064a4ce592c726f120998010dc86ab36195b1c7c8facb156b1a7aea35d3ed2b3e6c5d9fefe0a33f794b1a7a529a7267d18e15b2a09ebfa" },
                { "skr", "2faa1435fd9f822303e88b291bf355d7aaa47ea46ed40d7c4b2d93341eed153f26dc9a4cfd7109676c6bffaee4e7ea97ee5baae529175d6fb2920ca2e8ceb9b1" },
                { "sl", "0a3241654cc080e707aac63754052cf662e16bf070ee38ef468bd10d67ea6231250122f117161249101d2918cc430146f23b1b3c42ddd80a69abf6adad18f3e8" },
                { "son", "954a4bb614daaecd04d28f0f04467e834dce381f21b203be56b5d8834697e73dc8debf5c6586882c9c425fefcdc15fa9875a4aac89ae3667e424ad3b6f9bddce" },
                { "sq", "82fabf0f090e40cff85e383a2a26356450df58e8073e4208a3c11516d3e64aefba2769dbdcc594610515cdf9ecd6b0f8d4504fb4c3d929e17d5daac9d22bcbff" },
                { "sr", "cf34eac2418b8ac728af2263d428cc71ccf3e075867c3ec34f07855294c765c1776c91925127a3b264bb01877b10f9b2752833ea9e545902b25dd973c2042770" },
                { "sv-SE", "be1461096383058bf4271f77c84ae683d68a9cc06f4e0a7ab3d83561adc8ad524963b51a03f4fb64a2002e3c09001c963b67cc6a7070999ccee43bedee1fd124" },
                { "szl", "4ea2ed27ae83a2f97f6a35da23c4b2aba99a58590a278d98bededfeb5991ec86787416587b2e3ee7387f9ed5cd5a0f7a28e1e61e627479a8f3492822e53d87ad" },
                { "ta", "dba06c09e00733d9a1534cd92c604f82282b65737908c988ca19412d0695ae5e057ae262928698cb782d267d2043afe99003a479cd54a30d99b3ec4b26deef58" },
                { "te", "188843566b1a0a0dba1cf0c9310a9fa192051a9cfab9785652be70d08678c7de43ac0e9a192d633bd7ed27b6b436be0ba3e2773c83fad047aedc2df2171d96c0" },
                { "tg", "00846d654505ebcdb896539913abd063805c1497372176db833aafb9b3db2184660bfc0ae20a3f42f01f8bbfe6ee4f50fa6ddae23dea8e113bbedfa87503c80f" },
                { "th", "8acee00db99da896136b03b68241622abca5eff7fd3dcc6ac3a86f3044fff2590ae182d1871fc10a100f338a8e4d801c202e731d6e5e6af45fc20ce8aaf0e850" },
                { "tl", "11612222d7c60892e088c6144c6fd38e8fc3df4befb6053db204c8293a0082fcd9f618645fe74df9ca1939105fb1b024467fcc1a5abe713932b44b86ad2b6fd8" },
                { "tr", "b9bf6929ece11c991877de0cad636446be594e84ec48ff2766463a803b36b2d994da4d2f2ff4065defc1cc919c5ac86d726b64092b3088410372ad89bfbe229a" },
                { "trs", "2ed1c72cdde76230de71e5d176ee82cae762ef58cca066a68f19e8674c5be8e68cbe03ea361ca38d8ea62c8d55385addc822a90b0f21d763861d58ad376ed63b" },
                { "uk", "c9b76e875b5b5cd6a507bdaaa994e31eeadefba1c883e6489966460ecede0487c8d4d0d316f9888912500398b5ac8aac69a566a71baf032b23c84e2cd476bc2e" },
                { "ur", "a127c8e6ba5ed4d9efa0a251e759ce327fd9718ab22a52fe6c7d43139474b4a3676c81b801247e32d3991a7a4c3144e5692dcec5174fe1c2527fa1ef8cea31b7" },
                { "uz", "357ebf6d108c39a0b6862dbaadc2419d01541c23a0a5368837ce3d356e6160a3f405b501023f8ec470b387b26ef5dc0b7f4005b7f7459b2f9aaa0ad898b3b490" },
                { "vi", "668f81078c8cc1439b59401e84a2e89464aed41bfb974f393334435314204534e23f531fa6f56934b04529bbe491c51be67285ad6ed179baddce56584cdc0c85" },
                { "xh", "c6e7564b60b88672b968fd220ad2dc1d436f5aa732f5fce91d6d8a24b0cc0e1babff73e7867b01cac9870a09017f499da6fefc2635c11ed67cc0bc8708473b80" },
                { "zh-CN", "65a51c8ec2cc75327462d84274d9e54933a821f90c16130055dceb09290f7f6d99c9427ea8a11405196e3635f9eb8b8b9a412c7b2983a9280b93c05ef65d04ba" },
                { "zh-TW", "4be950ca1ea040e1598fc8eccf193dbe2a8ce7dbdad0295d57c9ee597c71bfb09938a750b47ed1d394946d80e5b2a4945fc8b4a2fc9aad6bfb2df2edd4856e94" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "a950ea34b1476e7ca0db2f104b87efdeed4cafc2437d1b158bb81c1ac55e7a397b2875ebc13418a68163b2f0e5f8982457f284e60f1b726ac504f7cce86690eb" },
                { "af", "7ef47ce40ef17198f00a01ca674172b8832ba91b1c000c96d89b5444025ecd899e58e82c983d7b1017d7df896d7008c94d18d7a7526fd5f450cc7866d17e0828" },
                { "an", "d7fc8d1e95e4fc7c8eeee4c10279e458d19847f6407ed19e362f48315c74d1c8757dcc3d63b337b00dde9afe72ec07ae819bdb984588afa85b652c79dd0fb7cf" },
                { "ar", "4c7c2ba5f7e90566e2b8c7098e3185b7f3be0b089613b5bc10463b6d941ed83a9ab48f74c5dc4e98100e577e9764cbc12a884e62cbfe765eb974d12d7dc0ec4c" },
                { "ast", "f94bf238fd3d242bab8b9bdf60abd5ec90dc79e2f3c0e66385797470eb7a8a6f0716149841ad8fe171249059dc4711119f79c53c0ecec0dac7f4d063f560200b" },
                { "az", "63ae6e476049c87378d4aa16b70c69516b67526e60dd51b41e059344dd238bcdbb2d065bf09c53b696f4e7b38b0bb6ea39e1fedc3b0207c1db98e193da31241c" },
                { "be", "9dfb1c434bfe36f29cdc2373e0f2e4573804bff9f342bbe38fefeade6bceffe2c98549f4a1191b9673a9b366f87411279380e671c0d5fb99e877b32fd2c9b1e6" },
                { "bg", "be69cb9774fa59a8f169824a6d4af3300d1ed43fc8d1e9fe13567d1ff50300cf659ae9d74ce4d0d6e767b945f3206a3bd8259357548a9b6f3f47f763e0a3ebcc" },
                { "bn", "d596a90b0d29673a2683a868d3093ed3618ae25e8e3f217468992afc51de39af9a2d4ed9b3baccc667c9e1453288ad509ef12d6905387a365dbedab0e946bdb0" },
                { "br", "48feb32b3ab286f9783e9d44fbefbed5e4be711f85c3c63f798365ba03ef6164ccc7d05b4b4e7506a2b8518287f4a2f339e035509072dfceb9fbbb9f16143f35" },
                { "bs", "e4cf4b11f9eb9a0df14e24ceb950586404d580abbd78c6487c3e23712a8ee2a45e079910367acfe641c387a888e32ebe80d7a7c7ceb48037b62ffc9aaba428fa" },
                { "ca", "345b7775f7017f359592bb005e35723d0e239fa479c07490ec537f3fda87b929f6ec55a71df782dc7d69c4575c3a38446c7143ad1a58b9db943d02c602248b4e" },
                { "cak", "5db1b3cafe4abf373e4b05ad58ae6847bd58861a87f46b74458f7510d02d7ee76037d12062d6e2dfd367c15735e4d1f33d08cb663b5cb5762d3edf4d29717a6f" },
                { "cs", "3b2468bf60793432886e86ee7279b4bbda4c4ad3bd40f67001db8089977d37ac78e05bbd4e4ed4e99a01198f75a56438b5ca1578e350432a2eebe1680a85fa73" },
                { "cy", "6ef5fa09fcbb56702003cc20fb113321cd3c41cab29ccdaa2ca1756ac8efa76915a8c5d3051b3ea6abe31eedad1b0676d63f1564eb37a3f75a60278f767771b9" },
                { "da", "3adfe5864d5dd7b34ce90af65282823fb800b7c9adb5222a855a902858a47eb4c17368b67096ba43e9d5005ca9df73affdc5290f638f24a7082dc8efae3af7b9" },
                { "de", "83182bf1ccd1ab611721bea019201451417bebc2aced81e4594fea1e8591cce58564211f761b654a9872de7a1230b9347dbb00aa7b37f05678454a750e4409ac" },
                { "dsb", "0a12c27181c4d05219a53f37bc1e20de0a8f71398de8b48161f2a141e285c636d0bd1290fd321665d0c891d5c21f0535c379063ba4fc342cd560638ef972506c" },
                { "el", "b07ccd41856585a1e4f0425061379bd2aacc82396179502647de9341ab281b8910dbdb2d15f6873f7780a008f82d260efe3d783a27028e9b2549260499ca32ca" },
                { "en-CA", "4ec8ea7b6a5d135c840b81b7a6484a9ba7e0b17c404d442bac3453f44a156546b6630fae61d7cf1018c322a1e885026a4ddb812a4ebc542ced1f748241c943d9" },
                { "en-GB", "91da65304c91fedda29e321e5d0ec6447b51236771b3c2095d25655aadb5e3040f1cd173750bfbfbc5396267d3c0ed89448b64b0f61aa6270d7c62bd7c6f36f9" },
                { "en-US", "ebab192a35176292bf3476bd5f198844ac4910e270866fb655cfbcf2db498d36a7b85c58d2ca2647222848f087e71b6513636d7ad764765baaa1a079bfa3e9b4" },
                { "eo", "b8c71499a509be276986fdbe7988681108eb378884acc645c3f86fe2ced8463e0c7f89df07a151d59048d1c93e2d9b165254e93ef96a42a27a7a65652861fef1" },
                { "es-AR", "50542f275ebe74176be928ba87fb2f7cbec325b5334098a50fa954fe8a314b4ba13318d7f352f94aa1f3ad1d45b23b4a4b604b9d88b111d4a5ac5188d4be04a3" },
                { "es-CL", "f6e11a4b1b1d4ba409dcff73bc53052a2528cd8bab5a4b3b81d89e227157cdf0dd39fa27ca58dceafb760b8924870f003ff8ed0ec21faf3f7daa7b772dc730b9" },
                { "es-ES", "6a495435417fb86463dc755b9ef977b57c653d5bf48aca1af0896a746146681040b6a03292cfcb84915686def5455a8d4f444925a06efc46f005251517366745" },
                { "es-MX", "63d96653abab8a20c5987625282d949eed5020e29b8041915e25dac724a2965a6fc0988c9327144f18afcb7091a8e8b71a1be2dc10058454519c651f7343268d" },
                { "et", "64a04f8ade4c00e75ba0722c1b6e41ed53a393d81d5035004d0006b8eaeb2549ccfbff5c936352421672a5af1777498dc92efc375e90e4c65f43cbe89a17c856" },
                { "eu", "2ed07f8dd3135ef880a59bc71db2fb123221462e15cc72ce7d6e42e707392a3b80a4f7d791400fe0e44ee7f0f0033628c96d6a8f6dabcfdec84c1465a1e99b0d" },
                { "fa", "431db89a42c59f6feab69065dcbcceb8e3fa56c33b6c58a5d6d08f40619b9655f9ed586db75fc988208f6121bfb46e96a412475d46e6677f47107c5d7482491a" },
                { "ff", "a2ff71be9f859ee01ca4e1c21a48b3506ed327a21e164cc169cb1f19c1cabc3ffa3c9e4a653cbfb43fb62e78fa481241f0a9176c1213c8b8c0d695a2a23f1e89" },
                { "fi", "af00f85c16fc9f70b844c79ff6cadcfe687dc6204a22f569746609a3b8a5a2399fdb21243665d21fbfd5386e2c5166f8a5abc530fd872ab354a1062ac3674af6" },
                { "fr", "35a904be7bc3172817afc047a8d098b2d699a4e61f3a033a4d2ef5727e07d17624e970b61cb1af0239e55b304bae80ac229d67e15ad33f95d30ec216a1cb8af9" },
                { "fur", "6f98c09cedbc7d839ae1c3721bc4e1ac1302cd3745ddea9c599ac0d8cd960689b36da0e98d8aa8fb554624e1ec35ebccdfe368b4bc8fed78109ac44d590285e0" },
                { "fy-NL", "29fa9732533dd3c17855d3224fcfba51bf22966cfdf51ab1690cbc4375cdcce5194719443292d73737b0bf55e9dbcacc7022bb1f28151c57a6b006708dfdeab9" },
                { "ga-IE", "1b7ec1ae6c483fac9f5c964ffc2afaeeabff85363aad1f1cd7f734ab1c781596ec045c4644ed6a8ace0447482284b25baf8108402af50ae5bce0cb994b09f14b" },
                { "gd", "2ac029f8a9a17a20d4d5b978e90b83d7cc7f629eb92abe80d29174d5d8c7735889749121c6ced3a821c947eb7c9792d7826b3b8b61ffe72cae137549e564e0d6" },
                { "gl", "b68cbc8a20be469309a59755c70728b7c5e7d516348d4fc72134a4c9285c2892bc6b0a2b046b0cd731995f6fb269878cb5974059db84befab070945c1287d57d" },
                { "gn", "f6495406f47f123537c275a35dd82c71aea45a5201fd60cd3f1993c270b6a396e66ba3a918721d14f68d3ae71d4173db2e5ee85c528c679f559ae4581e285ba3" },
                { "gu-IN", "923455049c93b3cfa80e3e04de22f06ba28f33588346e1276c14b3ddf3475ccc41177429f2525f218168e0198d68a082a41ef10f10b5010ce4dcbdabd2a4a5d6" },
                { "he", "3cd58a470bbe0b6d02c9db4a95b71d51134995853ad89aa25bb14b0ca03d997cc2dbb86836de0735cb5cdad9f55107c8ced8666d3b8b3f82bc72cf2064aaa1b7" },
                { "hi-IN", "1761bae6807bb1b528f63afb23bc26ebcbd041a4521adb8c468dea108436f01ef9c4bc5010911d3d434a5252b3fe8a31af83b8911ac7a0132cefa5eb6fa7dc4a" },
                { "hr", "a8392efa4ddd927a3e6f4ee5f116412124dc4a51ed25651d597c7a352f28cfef25a476c1023bdd58676aa81394d7f4a0c978edcd0fdc3deb2002fd3fe300b807" },
                { "hsb", "a151f3923efcedba78bb820ca3e7aadadedb5792c591db5c354e21cc737e9d00399475ae9c8640e3d7132fdb91d7f9778cef4547e60b6d5b99b995a3c7ec5102" },
                { "hu", "f4e9906d2e4e6a0efcacf29248df4b2d546a8866ed45dd772fdadf171c119bd10d708342ac23f4719a898082f3e1d13e876cf77719fa4293a6805a5ad6954cab" },
                { "hy-AM", "613a874787cff6183d7ae6d95f8aaea381a0dd80bb1b41ce0eda07007084ddef5a3e5e026b38b492ae3c7ab497d91f22393070c6853407f91a9c251ec4157852" },
                { "ia", "dfe26cd7485f9d1075df6c77ef0047e34fd770ac186c36a759c5c6a2c2d4bdd276103d43a56296b4a535c5c75894bc2f4f2800551c661b6e61cc2d94d7dac7a8" },
                { "id", "d0c0b4fa16efd06e7cfd4d18270f9a0f3a787dbcd94ad367caed6b2c354cf88736a0e721cf8e4211adb6bbf2be10d3861c0ba4738654e784761724e427a01aec" },
                { "is", "a912d989a22311ddaaa3abac05ebb4e45351302f4141b4507cc548cbe8bc94b3ad96e12af952eb5cb36e69243f3a7f8ad476d276b0bf3428e94e42db3cde3df6" },
                { "it", "e802ae79b44aa898e1752ea5dbd51a536afcdc2a79c166328c477d4f73766045e1eb0da9f902029c91de58a93c388dff3c7d717859cb242123ae9cf3e107032d" },
                { "ja", "3393b06dc70704140e2b5fbba0e97f3cc8f15ff12a83861958ff214fc14c6097bf6800df583706e2ae281b1e5b1509b01c593557c61d4489d6df4fd36047a270" },
                { "ka", "9d308e06379bc39e1dfc2c5fb40dfdf8c5965fc7e13d7bf2847ba81b503a55f2d42ba208c288c9715c17c318476750c8ba4d1ad3c9cd488788f191cffcdccfc4" },
                { "kab", "b09e1a6ef9553c2b89ebc575c84ca463bc29bdf7573a2e800498139de82cbe606acddb5702ddcb41a7dbb06ee153355576294bb76afa8c5f2042279a485f4bb1" },
                { "kk", "d0915b2628cc4c7fdc77232dddf1fe3180c8730413c6c3f1b44dd35de6d09735b0dc2ade7bad95f0c28de6d962888979f043ece188286fded3316b076b8dede3" },
                { "km", "eb053ebdd7ae20c94149c1f126cf8646d6bb37393fcba4b4da265285dc0b8e7fe22080c32d9ec7cd74451ba5279f2d9c5b15fe0a66cffc96465cfc6765754993" },
                { "kn", "f6b5c40cc78271395c72721c852e455606be41298203e70d07cf867696d29e4de07bc264018446c00dc6b9c4a58e4f7adab770dd218f109dc9f79b72ceb3c405" },
                { "ko", "d16a4299c61ba1b79d8df8999a97f78eede001a4124efe2e717e4531279f09ee952f43f4ba29abf8fabdd3e1922a6ad84511db3e8fcc48f5f1cd10e9b5b01ec6" },
                { "lij", "d1ce5907a0d21e3efadadbf758af8ccbcb3228dc9c64ba36f29806b05b551075149a60e75fa53e73395c884d237d26b9f4bd00377787af2c8330f00e0a16c2af" },
                { "lt", "4dcc061948d1e735c05926219f57fbc35dda14fe64319838bee9ff50ba27930977af0fac8f894d0542483e4dbe82d2df57778a0708ece1b5aede536473620646" },
                { "lv", "a1444669c3c1b99258c2416dbd3fcb75bfcaf5aa5fffbb02e8a85af2a2043db39298aaa4d123d5408b59ab368440a136ac6f6894e24432e925060a283ba15fda" },
                { "mk", "adb3d79a16273306a65d2e5b003b67e262679f274a791dd3e8239d7eb6a1aadba655379e5ba0bc5af993a4f81612c7a61fbdc84902837a2353ec97aa14bec1aa" },
                { "mr", "082fe9bc22a0e1c6aa1f8cb05e5d901b3664b915e2854e1b38b30100265cc09d662b73cd6b16306e1fddb85cde40741c53b9f01a931d1e042c645232af6ae2eb" },
                { "ms", "60bf630dcbd30b148d14af5bc19823d979547871d2f2498d4a9601bc41805e7b2efda3ec851dd57791a74292adb5dd749626035d22e379553b28f52a1f33242c" },
                { "my", "2ce36254d35ee325ac8ef4b58bf5b1966d01ed59cec0ef2ff0f7f282f48248f526a192b885d4fdc2f8cd82c5e8b218855bb4c09610ad28c4d57659d20b4e8ab6" },
                { "nb-NO", "567bc39df490a30618be31d436cd6713f38c14f3c59ed88cbb0c04cb8b10eb3cf235ae77f4941ffbe2d6d68429a07459e92bd1449f509a79604e07d26af1db50" },
                { "ne-NP", "1a640efd7ed7e4d7302b3676a3f68903e7e5870de0943f24027e231a5eff054edf148fffd08d98fc6330716dafc59612b68b8d10514a33dc4421568d196b51c9" },
                { "nl", "364bc3ab9358fdad7e59e9da28400d1b149fc3a1199a2b3535e1a12b2877d357cae83e00baa1a559d874989e3dee95fe812d01bdda5d0329877a077303cc149c" },
                { "nn-NO", "3e9e4c4068691bf755e2f2f3b83051bace584461145b4924fe62037686260eb81e2385e9577af39e44944981863ba35d71ce670a8e61d8603255735b92445a7e" },
                { "oc", "121e106aaf969e4bf6bc4cbf9a5908b83dd126cbba9dfc1fd3e2f98220dec16b02d3934e59c847d223b36ae688a749fcfcf9a4621098bc825bee03f79d29571a" },
                { "pa-IN", "bff528d9ed1d03c3c477d0f1ead7461922fc6ff25919e049b16c9e6aec9fbecb1687873f62a8bf4941290d7a682341da717973a8d835a557b9ca20ad2a84143f" },
                { "pl", "22f17aaf2fe11d02da23482517db9d52aea26c143ac62fe9318358c2eac497f5d9ce04943188cc6e6ec3401f15753e2ceac8bcf7eb3e24e847d2f6ac99d9dffc" },
                { "pt-BR", "361265af6804b5bb4827580bdf8318d8f4698797e057524de49733dc78b20505758406b20965697fe7007c017825e1cc00e54b266ba907ce8cfb1a903b5283db" },
                { "pt-PT", "748c892435de06271169206d5c2fe874f0faf4c5df12f1905d52fbda2cab014f3b9f25d5afcbd1750968c07642fa44ab34f5d26d9b22705014367186692b07e8" },
                { "rm", "08756266880e4335e36ed5aaecc82a41a70b84c32549ac62ca558131154f0d546f00cf05c142b56bca13733ee5d55aacb495a75a50eec8b53e70e70af61b5b0d" },
                { "ro", "9e6e0b5d0cc2401c24b5c338c9a4d7bf8479e70846448a240480b1a55c604a94571a24bd205f1587cb2bf0d3e1bc759d531912339f5eaa4947e87bdd3a5cd5da" },
                { "ru", "436a6cfa619f3cb96ada6f64d468cb0ec545eb130ca274466dcc4463d261aadede95ea74b3bfd55996e4815def4a4627ce2e6ae33479ee683b646b98485bae9b" },
                { "sat", "eb219816b16fb435dbe2f6e844f0f68c890d69c66bd47709f6047bd9fe40c5780b4fea98222a054f4e18ee77330798695ea8f962e3e8aae34d9e677019dffa52" },
                { "sc", "1f3b53833000670e2f5e792c1a7d51d4cc95d3ad46eec2364780cbd37d8a81b2a081d18082e70d3efd2ac8e333c792a2ee384af4d909968169a06294cf9cb6fb" },
                { "sco", "04838c9ba6f6bbe40c5871a557dae8c20096b0b90e7132f2ebd7aafe95898f1805bfbde8fcc40c11e98dff2e304db3a110e68f42a1bea1b1fcf7e6103fa32234" },
                { "si", "2b07e16a72cdcb2a3bc4bbdf1fcd3e6da6eacacca63a83721728c0a31af1a112487bb502475f55d5cb1b5df69f47333c23508b9a7210434c9fdaa25b2b418fa3" },
                { "sk", "225ea1769be033033928e1d60ea149c5868304631a42c4cb12a04ddb59ab2f434afc13711f34f344e48650c8ac17fb7419c3d6e46c3c5bdf4097b504967eb6e7" },
                { "skr", "475cf0ec0c00d7be630b507cad830b2330b4d53ebf152a18dd90720312207bf39c14271d6969fe8a64ab023bc9877a0c3e8793614b0907e7d0561efcc1d79187" },
                { "sl", "89429802095d1e58549beeee15e467ecfb905b6824b0f7a6f82ce5ee077aa20aa39a8423d2c637da3764ddcacc81e803cf5b119f6950860a340a150748d6aff6" },
                { "son", "8204d0ce161747763c749c3a6879fece99dd08ee5d65ef0c1cf4ae510d000bb6ce210e6685855811545ee115653f6b7f805c2e9f32ca7e19341cccb0df53d06d" },
                { "sq", "6961ac83f938873185007f0391de50f2076056896aa7582a7a94b44f92956d1135261c7c3347d648d946257dc024f628d027eca9419eb29eb3211cf8ad97182c" },
                { "sr", "4601cd4035294cb050a7c79380f79560a9b6b5c000cc34e1078fac56fe424d53535fe4e40cd34511a03e7c5c81de5a43e698afb137dc9677f6879bfc4a2d10d0" },
                { "sv-SE", "29ac4da1d08a9149f2f50b45b5cf8741d90fc36dc9df9d9cf917cd861bd647cffa2c8c2fe46d29db5440ed922a203c2cba5151171fe0a061bee8f6b3bb24c04e" },
                { "szl", "a60b39c3c314f3b93dfa427fda7af7669e1c69b660165062036da335596b714eb0dffa3d7398f09990994e52c05226eabefe903db10de82cccf02b39c971a241" },
                { "ta", "584ee60fa4dab276dbd1d56cd3aa722455067a42bfdeff6be809a54eacb0b0ae600aaadd61f7d566a4190f6f0da689c779839f1be57a1b26299acd0d6c90fd6b" },
                { "te", "4a09836a14f504a96900d8fcbdc575010f17f20a0a11beb6f3b74ae9307d0a200548fe4b52f11e6c988bdc2828dcadd71503279057ab6bd574c43d088d4d303e" },
                { "tg", "cd0e67bd81b0f69da2f2e88ee6647665267b397748cc62525dfe5edff0b642484e743873b2249f12b6eed2256924abb7d3fe958923fd61c680c465ab597684e5" },
                { "th", "dab522b030ed085108eba48f6e54eef3cd81193aa36bf8d5d18d7007f87c6242d1f420dee3c7080457e6c450d902917558da98d909a0dc048b52f072ba0d42ad" },
                { "tl", "b14b54d39962b2b92db1b68ade30ad95dc07ac9a1a9f8889c1a2c3d71c370d5f3e9269c2e3233e0d45d7f8e64c7e14ba2e2e8d85b831a85302c6d3539d9c142f" },
                { "tr", "27d030b0a2bff17443eeda7597092818945d84f64ee53deacce9b2dd9f088047ad217e1bb76adf7e646bd99fcae182c2492d2c603206927a73595950335bb132" },
                { "trs", "3ed63953f96ff7d99e147253b4d081916c9b2839f808017cf642cf29e0135756d1dbfa40bbd44e7ff1d3a0d4b258d3b7e7d246c34a02fc394de07bc6a094b6fe" },
                { "uk", "b0665cc27b36d9bf6463692583815544a9f28e8bbc7ca256cde8c4a0b6e429cb2fcad5d12544e2276bb9b681602c4ad028e26a7a6eaa4e7358eb3215d4fc46f7" },
                { "ur", "91a4ada6c698e6ca3e915e1fdea2b81d8e9e19e15524200a0cf4156dfae508b9e0c9ecadb08a1902a55fc246b8ea6076f8321376dfd547507ae090cf14197120" },
                { "uz", "0592a24f059c05889cb19d30a0126bbff9a6471b3551b1f3fd6f60bed4682845c87f4cd229909deb13fc2f59c455e3d0d3b8bba58e8350add79a5f7dc8231c9e" },
                { "vi", "87f641404a6ff702ba6722415970222082609b2af508620f1773d2e17c6fae17589de288576118798cd7191094508ff78471b4c05112a2001df6edffc87b3d2f" },
                { "xh", "1851580ac3f2722d58cefee467e1fef96ea3025f91a8bf2a7a7cbf97a98ed9036762d939db97c1d53d9dbbbe1461eb8cc24c2f88e0dede05818f1f3191b171fd" },
                { "zh-CN", "778375b85f21f019b280762b5dd8f5c28daac2badff014bb147e8642b8b0ef8e20613ecce51469f40715352d587535e7ca9e85530356975d36e64a315a7a8b79" },
                { "zh-TW", "f1ce0c0a0a2bad6b8750fb5ad7c8cadc9e5d49b26de63c8d6080207951c11955bfbf440b95daf9056837a5facce5920a655e9acdd7a5dd189f774d4652af782d" }
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    cs32 = [];
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
                    cs64 = [];
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
            return [];
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
