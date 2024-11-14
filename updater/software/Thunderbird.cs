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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);


        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// currently known newest version
        /// </summary>
        private const string knownVersion = "128.4.3";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit) || !d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.4.3esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "750efb9c0710104850b4b9d6b3ffe90e0a9f2f7f26ae109c307729717adae8fafb6518e9130f62680e48868a8a537cdd9f19c38e375c5c6a305d14ddfdddee8a" },
                { "ar", "f4de2c776783b282a60ae47d1788a28e35f88d8532c595c5f398ff74254e086cfab098ff12d13d7c6b3fa24b93d03a92c889d53c028a46ddc73a10fdb2a12c2e" },
                { "ast", "7231860b2eab32fed585470728b2d360b4b0eb75cf14f4de1fe739a2d50d01afe4174018403bc3faddc1ae13737c00854300fa51e4acadb490adbb38fc9cf32c" },
                { "be", "e9e54f986a84a6c3e3e91a9a275321f4ecb625f753f275ad4aa4ea39dfc17cfd2bf17bca8bbebfd20943867dc8ea870b725dfb49e05793faf5aee3fc4d98f068" },
                { "bg", "e6ab7e72b5823767613c41b3e218e1c17805e79bbfd95a38e979cd759100b87a3cdf8eb6014dd1b1fe967fbe3de4788deca6fbae8b911e65752f11604d166f89" },
                { "br", "8e808c5fe7ef77c10d085ab56e3c4b1483ad1e26f3b8f70c861180a9dd4d63581a5d456e0d544187765a6f18a86bce232ef0fc8c52b843742d7ed56e99236435" },
                { "ca", "33e519b10afe93d49589507b42c5c144bb4e3c39441a0f15255c17d017c3c5f10508ef9f3c753fc349a7af2ac610a1a84c0d772cb9b7b23a6b8b5f1de46c8142" },
                { "cak", "7fecfdced571677d8bb6a1661a8949949b6e777da44ad91a0013e97d267e956d7719960ffb6d76b545f81db5dee8b6a6a43aa0fb72e00008d5618c96b08fe253" },
                { "cs", "59e34295d961df30644a6150b18796945dd1e0e2d4cb750090309f72f2a8683f46873c6e964095e466c2f5e656b46173642c9c209dce7e9b970a50a0a412c028" },
                { "cy", "d400fbe4f8d952cb6d0db7c5d7b757894acd6a74c5759ff8eab94e6b499e79ee2c74dba10de94f968d4c7f38e613051fb4d4e3557366e3aa9fc59f50073be91d" },
                { "da", "0d2cee5a56a3a2894f304898b1de603bff17104056d3d4bb674e68696e69440a091e28b351751a683b0a23d35517aa0390e0920b4c1a67d63e9d7c95d5df2d45" },
                { "de", "a3a58c38a83812e8600b3375b26617c8d8234c486336c7a6e9df4fcc997a2056f38620c0ce4ea5018927670d3a11c4c7f5a904af03954b7b9154612c6cd5f1b8" },
                { "dsb", "1704895aef310dbc372b49a89ff05794b8fc0739f2813360047127c2ee6275f33ceabb82e5f4ffa96885240cb31f60359e140c8ba7a307d52dd51d21d84f64ad" },
                { "el", "dfcfb483e889fc5520a3acb4aac9d8663f0e05876337c3fafb564ac02cd082c83b0da564542eaf37cc253a9154a17ee277ffdbf325b9dfe5833af64181710777" },
                { "en-CA", "5b5ead380fad7c99d564e32097aa51fd739c79f61030cfc6549d411542c1c745ad8f822cff91650d6d00865ffb83279b0d9c1961259bdd946911d45a00728d57" },
                { "en-GB", "d290cc670be73285bfadeed0ef17b4fbb59eafe4744fd3d7c5e838ebeb4bef9379e61f83fbb0c17a10d17f76cfad06107b9946a02dcdd3d6362475528fbc5c19" },
                { "en-US", "897dda5c3b58a01f09ff5aaedc4e9ddfbdaeda9a630c089c21656d197c8f303ea29a475fe8213445d581d3ed44e5d8ce88d36ab197ae5d0d366379b0ed8aa990" },
                { "es-AR", "52d8fe1c645b6f536634f85ad75012e8db5b036a232cb89df3432b7295ee0a98c93d9fe3f48873f8a5cd0a0946e1e0ceafc630d31b681ffecdda0a321f8b4504" },
                { "es-ES", "c3a2ab778de0376a0d67b1eaf2efb7e1483ed7ff4850aac2ae36cb3ac2d7eea6e1ebfb86f9983d583e88e9485d7df1a2d43c67e5ef5f74770de35bb1a5d80995" },
                { "es-MX", "4b7ff0cacbe2a67250b6f54e25c9aba0b833b727c73b1a124de3fc02f70e824d5151d6941598681251e63621e69315c3d951b506e3e537d1199a25bd96b5ece9" },
                { "et", "3a08b5b6b89e1c0037ea643da1b5186412974ac25a1415cecb893b8fa8fca97e435e747f4c1ee9e217ee56535d92c6fbca786688726da2ed30c851a5aabd7214" },
                { "eu", "857194fb4bce7606e9b486a42b4cb1205628eb4811c10a8987ef07a2d6323f694e4eb7aec9617ce6759753f2e49fdf6b4f02db740fa0571c5360461e2d3ac1f4" },
                { "fi", "4a9bbc243723efc73f11e423472e288290808d9ffa8144b76014a233f174148922b1eb1ad2bf65970f3da13111760e2d2f3e0d9e95ecc06ecccbbc9ccc6f8220" },
                { "fr", "871615a793e0be033923c291aed466d8e6042f341750ac3cb5785ed047126d11b2acaa25c0ecac0f8bf7501e2f97df599be76eba9104f07d5a2e6e90749d4b0f" },
                { "fy-NL", "ea89c9a6f2482305a1fe6e34e68537da0e2d817e74d51064c9605e7467ec2c0e9d5a718b3592a68f53d06c1876d831722f999daba9487399f18f15d51c303a98" },
                { "ga-IE", "c916ac5a3baa87f7b7a9849d5817cf893be6f8fc083af2bde2f6c579fae8ec9e644cc410ace50e41a643de89242ae5bf304e20cff0f9db48feef01a2fb8fe080" },
                { "gd", "d02e330d44285d28cb973fa83094b9462d70f51ee144d035218310a01f436d056dbf448bdb5901cc682308bcc9ba6eed872ffe3e5a48a09c03c5d00d2d6da063" },
                { "gl", "16f02de4786f95147aaf9b37189b2025299d81f2f351a2ff587e184544e7a9793362a69b231e480ce2121307ec11d3775cfd15a26bd2f1bc8a55ed19848ecc16" },
                { "he", "1a958e988635d40177d2277f924cb9092cb0425de1dba1482495e09328b79910b156a0c39eaf0dad2e2099f0d1388210646b174ecd52a8ce04bd405f63fcbaef" },
                { "hr", "3d634964c3874be6eb846b2d45720155a70389dab27c2012b40a0ec5467f01f9d6ae5625edea09488d166b32243791cd026a591cc58574c3df3d6bcb4565f7ee" },
                { "hsb", "6c751fceabcc057cf0bd911f8f880b5352f2474f741ee7232c8a3c34d772227f716d1cf3e0a0003697f8e973c243f0ecd69c2d82fd969dfa8db4a172c38349d6" },
                { "hu", "4f62e8156fa89c9f92f6f0bbbb815955959bec884cc7c1aef8be25c115e376d0cfe41396be261d3420857042ca755706f040c561f095574e74bdea4b4487f615" },
                { "hy-AM", "dd7aa6aea20bda4a0a11a844eba4b3b0a6fc15ea14d7c0bede8bcc18400a89d3c63c3d19f9883eb0eb71ee73b632ebc8a77d7d1c402256b8f44d0c735c464803" },
                { "id", "9adc8f2aa1c6d31ce3ec3d340e1e419f50a1bea7f194add78444bcacb2c974735080b36717371dbe6f965399afa351a35b97d061432742277956745bc5961692" },
                { "is", "0fb569eb224878e379535d335a2ec65083446c77d2c35558faad8b31fbdcb2ffeb6647c8ec618b9d861600df265dd94923d9f1477a49e8a3c7cff3348294a02d" },
                { "it", "bb5c1fcce043f0f90e68b7bf5b8691fa9e51e8606d108e78162e5f32876409129cacfef1ca6a25507eda9f1a899d47346a06ae47e38ee98f3fe0b437e0b4561c" },
                { "ja", "11fc3c5ab7258adc38f1d1c8bdfcf11cd39afaba5ed7f97494d5f89a13fc57c0dae9291980ee846a4318664cc67c8e7c26d0dc526ff881544f4d34788fdf2f65" },
                { "ka", "0739cc7af21a3e8bca9a1a6353c0a4426c56460de17c2933227902947aca3505865d32d2c8158774f4fd854337e2aa485b65c689942dbc98bea2b4cf89f6dad5" },
                { "kab", "b6586cd367c204c97020a8cd48d1c7632609682e4766621ac7c1707ca948efe26bf51630230605389732f5f9b6aef01f90760fb2b66a27bf85328d627290f698" },
                { "kk", "774783ccc09f7fe7aba8a4bc9850eb8c1c8317a15d7f4ae427c7b5986489d6fbfebcd366c9da9d67cc9704d14d009a6f5d47d044b54cee96b33e0c40a9aa29b1" },
                { "ko", "1cb96b03c430299129a810588f746b84208b3ee2a2aa4227a530d85aee17ea272a5e0b68799a2680dbb3b641a19e9b4c216dcef56c2b1383823b2b3c28a2290a" },
                { "lt", "24bc8f371db1793389834a1a2acc7ab06d9e156d86fd0c69dc05969aa5fed044ea76ac5c2363085be34f0bf774bccaaffb48ded8b8294e4060792ccbf014e98e" },
                { "lv", "b98701feb6866d11fdfa468a97ebb5fe2452a87e784f9f0cba034d16f66603e5d7a3b2da7346ee18366ee3b6f9f474092e8c02625ea438d76b5cc4180a47f1cd" },
                { "ms", "e49b9fa73df5958857af683b431f9bdf5f84281052427f8ef76669ab9a84c32c87098eea5bf112d37b36d70b5bbcf569ae659d8d627e83e0f0787e32afe88593" },
                { "nb-NO", "b814f33d125d0311e0901bb7cb04d63909414da690e3e620eb2b26f1a9e95e70e8844df2999f74edfc4e3faf8f59bdbd003e4882e5d5e1cbceff2a325a00e026" },
                { "nl", "10a577297032f3e5be32a3287f8ba27c9254531553f374d3c24397c17c33e16766410de7aeed317dabb279d160d9e59ae35bcbae395ffc89753b49e88507f3fe" },
                { "nn-NO", "bf64770be445c0b4aca5a75eec61f738c0470156826de6dc433b85411c302f10a648f0b0d0843931e0dacffc8601a9c77c4b9c673aa89eb74d14e1d7c302ffc3" },
                { "pa-IN", "1bdb36d9bdc876b62134cc69d9b7a71cf5f91eb3ee92aab1a18022bfc45ec1d69e3748e0b81e37100a441ff38e3a5163b6b0ea2a80455340fc1965025b866451" },
                { "pl", "1b4aa5a9fc62620ecb36124f09fadb21604540a0769c85a48760e859c32a76f13cb40412eea2ac15570c58f82191845b3d76baabd5889347ff6d3b1f65ba58e0" },
                { "pt-BR", "3ec6f5a4a179b48f2d56d00c5e786388ab2ca2fe09bcf172d198843d200231af917c89d1e387355bc23f4bc47219e87e29fd3db68e441a1f90de932cb50787e8" },
                { "pt-PT", "1ba16785abfd162b58424da41c2f03d3595b3a82a3ecd68d2d1172f79fb6bc9ab4b36c8231568b078624bbee453410fb2e7bb89fd763a0d7368b418d0fc5e75d" },
                { "rm", "2d5e066b74a96eee8301cf4a0613849d17d5089478cdb650a78a1354879e394d38ac03d1056cc3c2e52a442c4169ecb7fa1310f396b1a65f3de51bede89e0a44" },
                { "ro", "45b4ab964353a46bb2e61a6e2dbe10ba709d0bd7846835c5237ff01d176334f1068dfd46d47b28680ef5792f530ffc26817b6b446dfa10d74ce7861e50af8671" },
                { "ru", "606a7379f34438de1893a18d626cd7cbc97a3097f26349b83fa384b30a8004d6293849dd22018bfd2b0f3a1be0e7a881fb0452050bf99c18a37bd328c1adf1df" },
                { "sk", "8ba04ec278cf1931507e791e692fa0ddfd70515383449b4f7f8e3c01b244e738d3e108ce29fae305f63bf55e3357ef72e614d55b87cb17f9d4dc0ba1cb42c318" },
                { "sl", "18723d1972e54c30ee8526e6da530ac09ae6766d275db46dae961e2431108b36bdc4f0ea26e5aa69fa7762bea5c9ef0c28b2e34295e23bc51d755a835b571517" },
                { "sq", "b3cbd0d046cc8314356fc44c0f025263814f8fc1f7f160897c6507fc48abd997d9c76301b2c8d4c37f8d8a2c680de280ea8eb6aca3f7480472b727401999c589" },
                { "sr", "65841a4f776dced9d53836f8a0604eac61681ac766fcb121a4ba298bc83157b9c8c885f25eb906a8d5d3037d55dfb6fa0df461e81496c2477c5fbe3bf6f063c5" },
                { "sv-SE", "1622cfc4f879e3581ae39d54550eb1c19a46345e7fba40324e773be3950a6481bc3d0df9610ced14ae44428279b906e2699660b884b3c55478e7fed17fd4c4db" },
                { "th", "1ca2751c901f90efc40075f310e3759b76678d41a6f46061e496b5cb194b19176cc3301fc51b0d2ccf85a1b39c982ec431c403a2c4f52677bbc792e9225e69a1" },
                { "tr", "d92503597e86db391b8d963877a992c5952d5b00ad7fceff34111d294606fb1f532196a3d1d04d524f30c159314a64d634f59a329b4db34c748edfcf4fb14932" },
                { "uk", "0bbbe062d9141f5b1124c34d1f0574fe304edd13d13f3a672f7da0976695def7068118d8e2eacaf1f91c8d5250a454f952d597f716d943352124b34ad42c8f2a" },
                { "uz", "76e7cd42bfec75587de9e295ea24b9fb8731b01abd8c729eeb85f02ce8fd89bf4cbc3bec2aed45a32740503ea6752f8be32227c990572b74ee629d7a65e7da99" },
                { "vi", "efbe678283d7d9129028b81b6c3eb376ab6ecf6ddefb74e695f8d26bb7c782689b59553c318ae316014d66ccba00268c699ddd938c1761f98b0bd5b375e5c04a" },
                { "zh-CN", "dc83fbcdfeb3a965b5bc784d415b28b25236cf5ffa7ec240e7914ca7709fd524769ef70b193b2dfba4654f58e35f14e8babf0cbe481bba817b9e37b703db2f31" },
                { "zh-TW", "1af6b231d9d8ab45180f49432837f952946ac0daee6aeab5f03e402b7a02515e3f16783ca495b48d3ba71e8f336f90e870af31a676deae84df7d9bb2c44950b5" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64-bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/128.4.3esr/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "077be3f3174667cb50e8aac8cd0ff232ecd15069cbda121f6277bbfe8937104be97f756b9f03849527a8deeccef9c6fcb97f11f970e56b37d7bca6691215ff78" },
                { "ar", "b6675880998516a28b61e55a03c30de1f29ff766ffbbb01fa435e01a978a36a117fe40928369027e4ff5b7bb83c7c188e8f317e9cbcd44063e02d1907247e5a0" },
                { "ast", "8b983a46833e8946711bff7cc9a59cb0e8375998be9507b1390b19fac0b61860e253d734ac2482ddd7dd3bb1af64383452aa620bfe4204b53936d0837e9993de" },
                { "be", "e4a14ef4ca019459ac7bc49eca33c2698efd89d78780d8a4425a6755e30ae884c5d19b8570eb9a4b08d35fb96f0e965bd989c7b1a6665d72196be272036dfc63" },
                { "bg", "dc973c625d3c76f83c4032584190e85d2aa10c4c8350b0b25d261675ebcdc61ebe923b4ef85ff01604aafa556909b3a27c8f4ce3e06d2d9b1844b8c27b5f13cf" },
                { "br", "2dcd3bf1b960b85f62af70101306d24f49578a9dc54f492013e77050f003bbf76818691c5a2bcc91af0e9730735448e5c0b2556a1ec1585602d44f16541dbd0d" },
                { "ca", "a29a66c66ff267bd3123109ccb3a89b61b1a765caf875a9ea1df19c81d70a6913a1316898aceb1c88da622700dd7374a1e77df128a9fde586d9982e9e5819ea3" },
                { "cak", "e8839fcbad97bad10e0dcd298c4779e600e1f4a651bec4549b94a298974f98fb5b816977ec8db45739e98b1e7c1502d5775313caf0f03e349b9476ceb0621f0e" },
                { "cs", "b27e487971ac157d22055a2daba02920e98cf252d74d5e0e1ace1aedc46aba4bc63dd14e3f9bf049bbc59cb2ce0a9b221a1b3dacfa9a6252e84ee0d6add5f61a" },
                { "cy", "44db7c00ec8b0450b1581c55481d932577c32b205f6b83cfdabe1f4b34297c0dfa63a5ed5ad8fd1dc8a3135052654fe9022df7b8762aa240a83578ec3fd83aeb" },
                { "da", "c3c34420b3f3343d689dba53bc71bfa9465f46304192dea38b8a84295c980cba4fa52d4c31ec3e3a81d35c7b352c943834037ff9ce6d40fd87d5db4f4b61cb30" },
                { "de", "6d79f743a68d0dc3cadd0c20c1dd81719e8175eee4de02162dcdf50655dc7eef19be710385d7437ccc30db2b2e70f31c3d1a7cdddb87a46464a48818e5cf07f5" },
                { "dsb", "5741fc9c92de4affe200e96e05aecee53fb763499da5a9792ac479a310d5a3bccc527af4ec038a3aae849c77d57a65643a7c9c4afc2292aa58e95fea952e5d3d" },
                { "el", "9d32e1da31c5318058d9804b5ba16526367263b34b45d39d8015299258e8625e6779433fa5c4f00ef72511a17a15036f655e330b79bc192174d50f417a5dc092" },
                { "en-CA", "3d29d907b5294db432af4a5bab83154338802b66e10f42c1f70e63f59a469a6d40f94276c49c8d59f2a5142d3d25d8e0505a2c7cc3e5b3e9b2e0b948ccff17a3" },
                { "en-GB", "b7f71e6297041f8386f02991e29ab7055aff1b3b6119389ae045848a31f1c217d865042496362f8d0f49858b9e8ec14f1ecb9189d5bb65618a54a2bb29bf9d26" },
                { "en-US", "e13716cce25d3b3ab5a78dd66a97a0e029ced7cf4928738c1fcfa54952616eedd75a53a89d284bd850f94b189b2bcd6da3ccf142cff7249001489d612dc966f3" },
                { "es-AR", "749e9816db0b9157d2ffd489b71671a667c1f309e5b8d5fe2f98b2d1ef67b4fc1e03d317d9c47396a1ff0b69531b7463aafb3d6dc1c5db9170afe6977a16bc6e" },
                { "es-ES", "c671ee5a55cf0a82eae757e71418580086fe0649684791221a85ff1a5b913380a9e2d3ac409a29a7ecfee51d3ae7d8bb280c93d6bb286fcd2b42909abc423586" },
                { "es-MX", "1266a19de853162ca9924b636bfec038c4e65ba7d69a2b00758b877f31fc992f503a4678a25bc8405c7eb3815609766d0913a393dabaa7ccd85a83ef692623c7" },
                { "et", "09589971a9eb103d267f77b66ffb1e767798fb3e23d965a5a32fdb379c8c65ef1ed9d48e23949b7b3c3d7121a99f93fa93362e18bd02d78284d23d26edfc14e6" },
                { "eu", "5abb8edf7d4112e0a69743a78374a081543bd09a16459d3597273b827fab882d4854528f0cd11bb7c7fee494ec1aa5c8c37741d53b3cd808c24b1bc79b251add" },
                { "fi", "07cd9df35e8c7daeeca62a66ac05c4fdc15d3ca073457ecf1cdb65fb36aab2d78b0207875eccdf1e2ed55f1bc86f246cbd0afb8c68b2099923f975d95c0e1956" },
                { "fr", "80431318c8451dbde199e0a2caf88b55b1a837f37cdcbd51ba28145e3e4574520abb7ab886b029a28dac442eaf9f1a3656d3cc4665b15111472de0e3005e6c60" },
                { "fy-NL", "6f7aa34563d42f8c7a0fc526951bed7bca3446e403fdf2de10cbbeba5582f3b46e912eeba4ef0698ff63cdf1a45b335d8943d04bb8c8728aa9468dded62738d9" },
                { "ga-IE", "371050bba1620204549cd91ceaef29ec0cef48e773637916237caca5df1a3074420f54d2c7dc45d80f30d8d8019be92ada4b26245d0fdf980a3ccba0aef95fa5" },
                { "gd", "7f0021d51e9004ef94d3799f320839a7c57e5c62672eaf5370ab8cb19fecb779ef37fbd5863e926fa5cb8d09e4335b0311c92ebab12d11a51795863dca2ccc05" },
                { "gl", "f74559503d98766e3aee87bd2e48fa887109b1f043ca6d209d7006ee3fd8f4befff952286018a3319a31415945449878254018d3024834a136b1259b0d62e91c" },
                { "he", "2641efcf09094d2b05852338459d77c0ef228f19662d8b84ea7215e5b7fcb8c9c995a48a368e85d58482c5823136f14aa5cb86bf77bce46e2eaa52b36df877e9" },
                { "hr", "bf3059f74312450fd89dc213510f11790416e7fe4551be6e22fbd54d175c2cdb9c04d27e31a2e952a9db0a05a71d723298f5aad457b8766158b811b91c71fb79" },
                { "hsb", "9c3f8b4a104294406b7829ce3cb5782b03a89c439f0d311454c844a34a3400bcc75b6d4db9e62dd717d6e6696712728f64d64b085835ec3088d872f032335a46" },
                { "hu", "f6ee62025624d60f232407026d4196628cf1b0727f20bf561fcae973cc043bad36cbf1b611cf7c97b800c7e1a0df57caedd67e93693dff61be57181b9647110f" },
                { "hy-AM", "25d50d2a0ae982644130821cdb84e4fe63774cbc07590fd43f22d434c8c88f101143cc184e01dad42427ce1fa02e9c04b96dd5e46337c3df18b0458b6e426f59" },
                { "id", "69e6838af9784dfb2a89341628048003c0298adb52441e81f2ae57a600f435c3222ab7921d14e9824649230208167c295cf1110931dfed7b8f50483cd8f2160a" },
                { "is", "545160edb1a472e874eaf8076cc170ae46b66ee8fe1f62c0f77cb5ebe5c1941570e3d72814d998e865ebe1e165ba0764356d6f79d4304e1b05bd86ba94a46a94" },
                { "it", "39b691270439c72f96d49de79dc3a22576f8e2aeeb19a284397b153c9e3cda38b99977be446d3047a41bc623017971c673b2364db1b67a0be6e0c01151307998" },
                { "ja", "033cd21934cf180b977c413f3d1ab8f18b9be6fce531e8d26e30e81e500463c20c6809046b6a1b029dfe3e178a6936df4fa9ecc295aa1d01b4b5cb74b311f414" },
                { "ka", "490d6cc4323a26ebfadb952e19e14b88443b7fec2c1941b9fa2df4908ba713560221713b89021fafb4b8d202d93999c075271fe999e769e7c68ba77174db8adb" },
                { "kab", "7460fe54cc1fa140d398b50bbd390ad906192e5a7f41b8d83e411d30e920a3e80f5e52020855ec234e1db816ec0bd0a10854bb1c58f5fe849932eca52f2df43b" },
                { "kk", "6c0e78b39f5cc3620205af672c9e0d47de574be9439ced037e9e139605e91613671b543a592536715571e59cb025e4bee17bb2b66a5b137af06d7cbb92b5b982" },
                { "ko", "e39f5b5e112e70b34f3efd0922f5b3bf7ad105350cd0ce8e20ea601169cf0b3c10429f42f3efba82c4bd95762cff6b9adb61620a4f8247595e2cacbf13c9dc33" },
                { "lt", "ca82ef7e11995dae3271dc2b25ee71b363fab24b72a9c10f7b4133e97a68a4f92c472119265968cd853765bf25362fc6571759045461068801d80a44bf691785" },
                { "lv", "c7659ac3c3b39ab1eceafb6ac9031627a41e041d893125f186f5c0c2519d02e11fd8a56f8c2bbbe33d86672ba695d16d4d270cd66a4e4e2d12b327ac31e9d981" },
                { "ms", "da0ca7898eea26fa9a95cbb75f9978a1c8cdf1170c4cd1a911eed627ed3a914a5e89443f4b27c1f4fec30a378ef5e7c2d553f4d5ea4a146aa375ce81d6991480" },
                { "nb-NO", "b6dfb2ba95ca7cdbc38cdaf2c3e62d9d1deb963f7867183efedc3a4cf363528746984469326a5b9e0c57c51abe02da5d77765819d11e965649de567be32c6886" },
                { "nl", "0dca42a00d4bdf6bea8a3dfc5d2dd39bd75bbcf5ab1a8944231d63ff2cbb37b8963506838d26bdd512a7a4cef8e65b4f370b519c91230c9c2ad23cb1689b60f3" },
                { "nn-NO", "233ef41f735e21997407b0fe9892e5d918a7b248cbdc2bab11b8475f72d48d01886a14c685198d7d80327ec7f725b7c5cba89b9d62f6d3a05e552659a516aaf7" },
                { "pa-IN", "48b993f4ab15f86df4230e1231900cd9801c2d6e169ad4faf4874aaa4bd48a68d93ca021a82d4ab5c83625c7ea14fcdf69205f205020e79aba925279a8ec42c6" },
                { "pl", "dd884f0cbce4b7017f64dc3c34b2b982b8df3fd9469f0ef341188ebfbf7fa6863a8adc0f3b1e13d741926572dbd4095f5783930f2f5794fbf2862134d492a49a" },
                { "pt-BR", "3441d0496533ce4776520e89b9669367fd513d4c8b5b541ffcf0d413da445ecc6c3c06ddf6413270b415a4ce4780318cb17c17cd2f7209229244b60cb57e084d" },
                { "pt-PT", "e38ff2fc35b26aaab1802a95501d24f7ead4d537219aaa31d49b0b7990d9414f54d4d9839b80eb6cfdbcecbe1cddc6873d0e2722ce97b46b024f5b0d5522e3ca" },
                { "rm", "bfb199b3e9d65ddaa6324b49496b29701fe607eff473532a5be097457a9336d313fd86cc292e7376d5eb5bc838e4c4d1f9118df2ed30fbb9931a3181f5840ee0" },
                { "ro", "5390cb0c7fadb36fffff8b00244dc42449bffc4a198c69f6a6065bcf020a4a0ce4f4d481f017c00c99eabd0596e8ffcbe35858177afc56471fc236d432b9555a" },
                { "ru", "26b4d3db02ad620941669ac66f6473d4bfdcee71eb25ac88d4312019c42dcca3a934eb6d9b2f7d8452e08caebb4a8219d4a7e155844622fa6591a8b5bb4072ba" },
                { "sk", "d864612495a0131f362a04fe8dd1655dbf8df2fe67ae6b395d0a4012db4e8af8de2ff965aec1cb489785532b2789f0c5f8a0d06061e6486501a335626e4c9021" },
                { "sl", "d9c40a1b4445e8ff57ceb4ae94076a524153515c1b495bce89a95a5503009cb48875f81e1191fe0bb47229f54c5dfefbdbe9541c9df971e6c4fadca32ed0d734" },
                { "sq", "f67154d4231a1127f19e6206b26b86d0e97d8a7e38f7bbeea13db1ba475ce660c4a1f037d1c37c1b3b9c9a0b1420e99bdf84d8b12fca89a37316b7fd8cbf6367" },
                { "sr", "0f3e08fcea0d05561b6a76e1cc2ad75069c8fd46d0384c9d8321c6cbd74bda14294dd192a7d9d8d4d11febb80450322d0f06c1e40a85c06772d318437546538c" },
                { "sv-SE", "1f1fc9d6dfb2ca7e29073e111b9f06c2501c85c8b006219adfc95c58e3aef2ea47b3637e59f150c376815cf3d6c1e65cff75595ab9f9ab5da8abfa2d5289b68c" },
                { "th", "d47ae67f95e4e0aefe69628fc3d4389096479283538c1d65d912c039872208027cd4a17e19082b559aa10dcc2690c79382d67f71e0c21696583b0e7916e8448c" },
                { "tr", "9b4e825461998d613c2149abbb57769dcc063f5196fb80c35491725e3e4ceedd267c98f71e76e34bf74e5b57a326a4160a56ab05bbd20ca42bbebf3af915fa32" },
                { "uk", "5ddb0da3bb9ab2d0302d805580c1af29db369fda172acb19e04b900b14e01698c53e5e59e2dfe4b6e66c34723d78c90dc5287a84e43eb5e393d98ef2d8f994f7" },
                { "uz", "af8375c4504b416b16eb349bf6145e798db472e04ca185daca157cc634364eda94f3bf50c7e70300a146bcdb59e69252f957a3069753a9e56c2c4b7659b5b2ee" },
                { "vi", "16e0cd3446a013538e7d2047ad855b02cefad9857db37598b68f73e8ae78e174ec6effc80d957df240c85f47fe149735db1ceef11f7d50ea8991a3df8433e4bf" },
                { "zh-CN", "e63ee5ac364880a8524a094ee595eaca6f48313ae74d518cac2dc4e27c95662493d2048845b0f49396c7c26f44d74b3f1d9e46787a95646a99cbf0f03a49f17f" },
                { "zh-TW", "af6a71b9ffe5fd81fe8efb3a7a3cf710fb72ccd31e1fb8507ccef25304819de3e40418c1a81ceccc85d6c5551be4a559e0e1dbff70a621db1baac317106f4e6f" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                knownVersion,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win32/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + knownVersion + "esr/win64/" + languageCode + "/Thunderbird%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return ["thunderbird-" + languageCode.ToLower(), "thunderbird"];
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-esr-latest&os=win&lang=" + languageCode;
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                Triple current = new(currentVersion);
                Triple known = new(knownVersion);
                if (known > current)
                {
                    return knownVersion;
                }

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/128.1.0esr/SHA512SUMS
             * Common lines look like
             * "3881bf28...e2ab  win32/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 32-bit installer, and like
             * "20fd118b...f4a2  win64/en-GB/Thunderbird Setup 128.1.0esr.exe"
             * for the 64-bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "esr/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return [
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            ];
        }


        /// <summary>
        /// Indicates whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
    } // class
} // namespace
