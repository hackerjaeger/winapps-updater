﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024, 2025  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/137.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "52ea9fff94a0f1c8aa0f8f7a1869d36ead328efa70d9183e3f12e28841a7eeefba1de8ebfc3ea36f0e59db2da39bb76447e5f3184cd8a90c59af46d6442031de" },
                { "af", "f974aaeb2ad12d8f029df04e1b77d962d58b8499bac93c61450b98b9c11d375c75b99ee83985128eb6fe6ce667667d8a5fdbb3608311477b5905e7778ad45ce5" },
                { "an", "affc3617e558487ea2d7a279803a65a70d83121f2daf9c33ec7a3d4deb010f84eab0315d74ca778ddefde5f870957d3fabec03d795fc1c57d1e9db250d8a4772" },
                { "ar", "655d2c5e4142ba7662262cdb3baa194d9251ea855f7c1979468694a16136497c53073ccf9b9216f1ad4e3e3b1c8764569b36fdcb12fdac25390ea516a33586cc" },
                { "ast", "95dd9d099c87400ac41b94403ed2acd3f95baedea91857e581f3273255082ed310f512314a0b4ead53fccb301c771307d4a29c9224c6a1b19f2665230491c102" },
                { "az", "3d6f7fb4be5e919bedb3c4d1a75b130a7b4c6cec2db9dda39309625fc84c747a23246148e91d52176db378acc32f98263a64cb07a7eb289d88bc7d732c0e68b2" },
                { "be", "4e72f5e3b5526224d78e2bc392cc470e95b9367223d7cfbb86f5c2b2731e625604196b90e620e8dd3a17a958a2c12bdf7c12c795cacb31b88286cc53407fa755" },
                { "bg", "64f1afb0993aaa0fa3dd028651c237d9507d569d36c5e6b8647d1b3c41fa07fde7e2f6dbeeac24610dbc02c3d40713fadd6a9b60be6fa37be863010cec338927" },
                { "bn", "3fbcc880efd61575c3fb0427c45d2178ee21a797777c55fee45fb4a12ca2f20fd580b4b05ba309420d2519521862b7c125a2863f1a2d213918e3613c3e315c38" },
                { "br", "d53217b61d44d1b1b5bad15f0f498fcfe4ef58bdb2c99d77d3ffeeed4a16929ccca5664560f2967f0380b663a1428ec7858d2f43d20dd1a82848424d31cf25a7" },
                { "bs", "877dd75c3ae274870d86aed10b4b142f795b8ae1f8881bbee01b563771fe7cf9bd102cb375f77cfa2bfb7f743464035cd6706702c60cf0cead57b29fd2622f30" },
                { "ca", "7ebbfcdb3ef7dbda53e2d60925dc958e71c7cc358aa8cdf9c07e69da4b013a8a5758685ead4c02f611c055d1ad30490b001db4098d032cb69cbc8f15c9869d77" },
                { "cak", "8f7a8d05c74f2acb49cb940863d21d6cc7fa33d212526166135155ec5bee8eba50a911004ddff865f8144c54f5edc3d49d599084ac358981c9ca92bea381cdcb" },
                { "cs", "e83e6004825b5668a9eec92be17e7010dfad10848fd853de63578d5da23d6a7a3b2f6cdedbe789786f075e3c45185446b3275c6ee8861dc1139b348ce822fbb5" },
                { "cy", "6847cdb853e1cb98a491fdd9b5ab3b0a7d33a8d0d79e425242ac4d12ccd6c4d7e758650fc7450a506327219e06f76bad1173eb97ff5822ddcaf1198520ef65f5" },
                { "da", "97cb49dbfb0d8e9274ad7d09454542fdee7d5e858a98dfbd8cd0cb5e3a012aa789c7da65e10c57a2c458476f9e5dbe42f6dbcbf553fd60904916eaa946565522" },
                { "de", "c80b35ad245348106621161c318a127af6521f0f18c4223d5f15b877a71f7fbf519cec3f58e98c5fc4a2c705e360a16d99a8272c29f020ace4d5c0f55bf98589" },
                { "dsb", "4cf281b23c980a4c11e0e2564f6f4f948d60a16a3b33cc80ebf2c7a9a9ba68853a1faed1ec6ffd4568826c29c4e7ed41e26d1f1e5f03fccbb88d6d5bc8e1fca7" },
                { "el", "3f6f4f30aa04f6150064b429eef6e27782bf79f7bd343ea21869a65bfeb4c2013d0a2b40c18851b3e77fefbf8da2f317123a7049168ad24ef1dab813bcaf63af" },
                { "en-CA", "96357db942d80533337a903a37d92a2d2f110f44c47f9f9e896702924c383a9ab1e173ed5589dc5b401c49da2a5c6f3bd7795b28649269ef6da719856669444b" },
                { "en-GB", "63ff79ce48a50d9e6fe123db943823987d07e35a086dcbd13745c73c73ece6aa0043544b90b5740807278923a0af7854c99f1f7efb90bb9e2573b86109afa3d3" },
                { "en-US", "8edfea2822cdd552174f3f9043b707ae05925de0ff6bc78f86779e9c4fb2bffbf1b37fd513a090038e7fb6d4ca41d9864c087dd06b1fe009185a45718e0dfa3b" },
                { "eo", "68d41e0ff6729b14b2b986af521810f62a8f0dcfc755ce99ef6dbf9df7953746233db7a36859299ba5a13dd1515f7d99ef6a7a00ba8d9e4f64c6a6642d4f48a4" },
                { "es-AR", "5ca3d64e71f2e63b3dbe48c2df4a54d3cf4b3c7bb3c45be4084b0dd2d70f07bb6e51c1ee6420376b95a0031ba07b5d3d6beb721610c4bc27b2c4373bdedc5d74" },
                { "es-CL", "74914c299cd389ea7f2d584fb4482c2298e68bc859d32a14febb15807b7ce836b34939aa973fff4264ad47dd6b65167f889753f1d922188f8b7a17dbba451f7e" },
                { "es-ES", "81b9f229e0203e8c45ca52b562919f9792da856e91dc05517ea2a8ec8982686ba9b7967dc76cd84a43e3b5040b8ec2fa1af60c6302069e91de3a9480d1361f7e" },
                { "es-MX", "f0a52f83507ee2018a95b6c4cbbee3ff2c9e085873da2f9425c5b4c4e559e6ef7e9c1e7215c3f6f4f0708a82cfe4b5242a70dd1d495e4ec1b42d47eebd7f2753" },
                { "et", "86199dbc292414741cd3a37d520c5f11daf2cc36f94b457daf1a7ff22ecbe66a87c4a0f48f20cb34c6a07c1198b260e6477850e47a98c0ab8938fbe40ddab0c1" },
                { "eu", "64501b9ac20c0f1d69c6403234d66824784de188d5057a8ca35720dc02fe76af8b1dcee7f4c9fe384803af11b2f85396ffd5c182e2fbbd0d83e01dc6fd962667" },
                { "fa", "4ce51125c92f47f0c99e3ea340e135bfd0409a009ceafa20690bf40db0776240ef4caca0d233c1df0dc7481a8e43013638a0faba5654e87b99839887a0d03295" },
                { "ff", "2b0ba4aa1876ddfdc6a55eec282fa58ada95a69cfb8bdaf8cf44a3b9236c13fac5af4d6d77e6df66a33d40213fc0c18e00271dbfd3515a65ecc719becdf1b56c" },
                { "fi", "41b783811537a213d4b32db0cab6f24f11556aeefb036158d027ec4d6b85611bf03178efbbce9ed0e01e9979562b988bf44307427fb344b8d79e21541cb8287a" },
                { "fr", "6e7d212ab9be8c5e34909d93389a4e6bbefcbf960916755a94526e66b69650dfe7d541a3ec32ee9a324bf403c6969370dc65866f0c7521d9da3141df566b8abe" },
                { "fur", "797fecdda2be254415f95091e92f0e069408c67909b84c90721a7eddfd90e3b603a82072b353718aa4c502e2fecabe8fd18304cb4529cbfe3082ca723b1abdb2" },
                { "fy-NL", "c0338b52863dd369a9e2a69039e4b0ff2c8e94d4497631d7c0b85a74a29973b64fd70f0e4e90392297181658ba5e6e0a47345e8b1f1dea226c00ebf587cf9a55" },
                { "ga-IE", "b4ae1c224affd97014f8adb2166bb34048ba40327eb5b86dfec336015ff8dbb73b68f9b7042c156ff6cd93be75da57b48cfa1d3f68483fb97adc467a8b5c888d" },
                { "gd", "1a5c0a0607605bfc086fff494b06c5ad9d398b99ca56c725a33beb9c4394a63ed6699f0a654c9cb693d6d99362e17a97aae6c0b9b6f5f00f5f0292cc2e9ebb4d" },
                { "gl", "5b6d465dd12723dab0f1904a092e8a904c143696288182d14bc79949f78ac6310f5344e6755fe5e53bddc8c564430966f0fb3df9dcc39a21d57486c822471f55" },
                { "gn", "2b6ac01ab66b7a69cf888cf0c5aef48cad5a66e13a6c622b03c9cbccbe9a602b0cd4a29c9813f5982e49ac82b18b4a66f7582760d020501174e2316800f59202" },
                { "gu-IN", "7dfeb73fe9567a003dd06039ccb5cba57fecca802eea7483756053386e768a452e567138193048f99b560e7018a1572622e1be40d0e4bc3d8b89bf05c4747536" },
                { "he", "835a092792c6bcf51ac77d956ce813f332bc3e3633dd02fefcf528ab9fea516a2ed8d2d69ff95cbab446a0bbae94ba2da652a631e3ce152d8e166b7749dbd252" },
                { "hi-IN", "58967d8181a383d0ced713d3896b59f34b197bf2b607be54fdd363d84b72905a293d43bd6f010b98a35c839940237e79edb940dbb744fedfad9a87b34a3e0244" },
                { "hr", "9e8efe3be5a9141198d0985695eb9d8eb384bd94ed4d8d59a3d96c4fb88aca1db21afcea5c9be3272e3df2b09cf480d46e327187a1190bb71b92b4c35fdd9166" },
                { "hsb", "7a941c038df8101a866d7dc7271269f1eecf90c248dc49dc6157fe43d8bdbc6b97dd339d3e0c411a3a8eb593948822790be27fc6eb05617c9b47cfd669b7c7eb" },
                { "hu", "ca3d63e86f51ce18792726a9bd042198855a8ef50b99ee437192fef4e67d47d836f153f7d80e0ca3f590dab316593dd6d61d5116f98158b7d84a04521b488d63" },
                { "hy-AM", "8e484e654064c10a852fc0df03ac5e368144dad1a39fd4f2322eeace01f2d89e5a2ae44e27dff208f9c52e197de0a2c7fd0c840a0c5f3308cc0c50ff30d6d9a4" },
                { "ia", "af48ae2087917578e0872948c9dbd90d9abc070615b2b05b6ab63004a71f31cd2dee69a0c827e65379096e7cddc04bc8b9fb3f36b58d2251c5f53af9fd394ec2" },
                { "id", "d5cae565ccfeb5e62986218d04c8bcf9c44c986b4f8918b3d4cbee09528ec755cea01d2e83239e0b412342a45a57cf420c7e009262c41241f0c4210a66b65a0f" },
                { "is", "f1b42e91985bb4c7269f04f4980757df35aeb23617c2af69cb564ddda82b09bab725fcef723bfc67d5c4e7567d8860088328d865ae07619d5ac4b9973288dc6d" },
                { "it", "75bdbb6ce1991482c6e77c1f1af752652df4a411372ff343cfd98c3d5ddabeedb8d034b757060515b3a04ba54225b5b6a3776cd834f0d99fa978a59a0475d8af" },
                { "ja", "28d5b3eaf76fbc4a93399dec0bbbb0f747d58a5415437d00ea3bebdf128bf66f5b2b8bf175cdb468a62574f85d6c27c85302b177390717684a22354fe5140ce0" },
                { "ka", "82bd64d96ba37363941bd20f728d0a33b20eeb08139148993cbb68d606c7c3e1ec19bfcb5c0cb8891137d1c2cfb1e08528f87bb44d7b817dd04cd45a19281cee" },
                { "kab", "5d64582dbf0be4f3ac149ee83627dba13fee6b90576ee081ea08b5f5f65b78c8c1030d862a3712b50f423354d761f89c4bc494cbc0d9979969a8ea67de2bd94d" },
                { "kk", "c48b848664642d5490b96427d2c6598e4bfbff6a3ab31edf5203b39f330f1fc25295bbac921ea763897d7e8ba40bab74418212e17c29f30f3ccfa1885adf9bbb" },
                { "km", "0dc705d43857cca0147d733c7cbc47702279bb23805092a036de227e66b40cc88434a3322f47b0fe267a738c6cd0a19923fa13a4aa41ab380aca0ed77d82c3a6" },
                { "kn", "574216d69108438361ecb59da53f1e2d96a2ebbd3534fa5d79910d92b0166bf1ab3af0fd5888d24b77951fe8885e2ac9834f3125533157bebac8ddd4e78e913a" },
                { "ko", "afbd9ce410cf0deee7e91a1db495cf1bf468fe4e719fc3cad658b00a5d7a97a7d527144b043b8988838fdacd49d50d3d38c251ee69dd0269407094e3d3d996ac" },
                { "lij", "c898ed4e652e43937acbd715a41bf5f77cbf539d975429303839086b79bbd701a40bf468fd7685b420da215dea8ab5453847453cbdd1e82750e17c91168096c0" },
                { "lt", "463f4fc4c585dd03c10f527c9d51852c1d47a0f020a43f429f471e13e840c03248bc39969c804b76087805689173a0eacbb780e5644aac5bf8a552548e2c47ab" },
                { "lv", "2435f5b9bf7d3466611d5d4bd08ff50d348bb240d7fca68a6b21cc8e0f6bcea173622050435874dd00ee1d38b10fd8a002598b7a12e4412579ff15000879c271" },
                { "mk", "91536c5a626faddd7e743b5188a6cb04c735cf0375d2387eac6485258f7e2e593793156a5301014ce365cfbbf764a74d9511bfe929e15162736256aedab0f2fb" },
                { "mr", "cf460d7ff98ae8a7c0cd843841d856967ad63f3291e851b4b07efa112bc488e4881f4052af4f062c1c94b4c512f907633f624b9298eb4c95ae0d749467e7dc27" },
                { "ms", "f4454836c529c601ba3ae1399b09bc56463a74d67401a52f3918d4731f1efb4fb7ef88af752e6cb4fb3235ad55c5c33d97e7b353a10ec51abff13d1fe22f9549" },
                { "my", "1db0a88f112f209315967c73bb8c3d1709fec30c4592676bedfa90674b7896f6819a02a5b83b098d407339604c6c234311abdb8b2b32553532f765c6acab5214" },
                { "nb-NO", "affd5638ea28f257f078520d1ba230ff879fdf6127e2934d4178bfeb78d2a167b6e30a28e3ea1819b0782aef2a2ec9777dd2c11e7a42f7a025455bff6a8ed5f2" },
                { "ne-NP", "e15211afad43eb984d3f7aa8a04307739476957236b582017db19c7468b9ad9452a83c845d0dcb2f45fe11b75dc1264925a82d6d81eac935d415a20cceede26c" },
                { "nl", "aa4f808adb5c9237208beac42003c19e3d3549a93ae6987a50da5e6a2a7952b10c7f856820a20156fa6541f0ffd172b14454c601288f5f3fd20915bc6d85f58b" },
                { "nn-NO", "b8c4a66e71e792d4e14097e18ba1898e45123937b8de7ad5a42644860f53634503648559f5c2aae2e53f941bd5df98e9e15ea42f0243f14e4db3fb39c0b67ecb" },
                { "oc", "43b177c38769bdd992af384e49a8fc3751b487b6c3a9c2353b7bead6c238d71d7c8c6c67806fa009323d70e720ffd591de8e38e922ee5b2bc4a399375e31b05c" },
                { "pa-IN", "ba8059b81830861197ab631226916bb7c41cb9a01c7aaa4de534857e779be5d690c64138d7935036121c1285cd5999887e0e00f29888660ceb8d71032613df3f" },
                { "pl", "80ca845a2296a73602965876da01bb9bfa995f9a96b19013d62494fa4f1750debb1fe964ba4cf2072ed593319adace94c85576a4d3e47aac0fcdffd60ff3c874" },
                { "pt-BR", "2e162a474282383d088c251e35f4ab4f82202e404ebffa57da9f290623bb9a3b0379b77980f6d990115406f2dbc9c98c3e810603361f64b90273db96f5de4e45" },
                { "pt-PT", "f173e08821763b95edbbd7d8e7de98f6b69c30740fb2aa9f645c8c12140c72a0a272f56c991b72bf42afeeda4ac795e301a01ee43066364e10ab1a4e211b5f8f" },
                { "rm", "663938e8923c234d60fa19c8fcde6b60f85b3a493c57696d2349cc156026cc9553fd7d1e5e2e576e23dcc64464106debdfe91daadac6faf20fca4f103c28f3c3" },
                { "ro", "18fe1a4f7ce095e227188f165d886e1a72b4a22ebb69591b79cce929de54b4af531fcf5287ec4b5c9f4323753dd2975f8614b7642206dc2b09487e7f6e2640cd" },
                { "ru", "622eb74e170f303118fb8cc1ea5a9fde486653ef8af62a6a985ee9f6fe201f1f260b2e7cce71672f28c0fced6679c13c17b0bfd108f88b138b2ceef8b2b6095f" },
                { "sat", "539b97a7af04bebd421eb8cf0c3e89b51f9946b103951ae34437de441a5d38c2071798eca0c87e8d04d86cb2daa675c94341a5454c995c623521adae74306c3e" },
                { "sc", "da6fe646679560c5802dc3f147f902bf52ef4f171b5f0fc771b3b170ff2929c49cdfa85f30e35b93b0f940c9f76a3a6bb10862b557f6f8bdb82648c5cfb135a3" },
                { "sco", "501e5804bb96253f05ced4fafe7d1772a8792aa5deb7ffc429a3290705573915a5f38e2179daec4fe16346b93e2090569fd5ef92a8754e45a40da75ecad4ff54" },
                { "si", "7be07be9631ab9425049b497c80f96713297efda71ca6c7d8855c1c6e496572d62ae55dcb38c914a7c55542b387f395882dbdcd05331e400e0ad259045662ba5" },
                { "sk", "21fda2ed1f124d4a0f9bafaab323e72db66dc28b087f0857055615180730172235273b818fdf877e6bb1446348ef5bbbe61549b59352138bcd0ae7759e1d8f60" },
                { "skr", "6953cc1fc7a133f76f60594ea6436ce40e19d51eb2cead7848c67993f321526f372954ae9ee514ec8bc744bc0b2c04acd34527a549cec8d2951289ea080e9062" },
                { "sl", "02070d4ff684cd4d1770be6b80c65cbc7d84d619c89dfc59beac0a7575460ed147536a83ec1d633821e53dd7ab6b49470f50a0dfac5a813d4436dd3c1893f171" },
                { "son", "c7bc192121f5de8859b0607c32ed573323011b06c4add2727c5a9b7a7f1d9886d3e1688497db4a826e3312e247464458d16477a6241e94ebb2730bae74349fc4" },
                { "sq", "7dccabc97053d42e5973b7b67006d7971b2fa7886bee7e9f677462a3449fa31c46d63d141a33a471f4b8c9de8deba8a74f0cca2ab7cf4b46fc873da0b4d2235e" },
                { "sr", "96126cfc73e8a2e1950cef6ab940907b1a87b68edb6b1fbac0147a57bdc1052ce611c7ba6a6af7786ae831158475e289d081948687a63664de5a46db5ed7e86c" },
                { "sv-SE", "41dd36c14e58309481c7791f27e5c1133bef722858aacb36578c9bd0bb8b71af73a57426f748f1208b48d119a24ea077aaee9c2e004134ab13c7831c9a26215e" },
                { "szl", "a71e2c227a29ca6a0ef5560e9b73f9603406a9746220b7a990a5711c2b7865581db6cfc6a267524684f838f6db95058c6d16d46fde580e5b8775bb73e195239b" },
                { "ta", "086eb08a56a97d31cbefdce9eca4bca496c3d498a7084aa91705b49f021289ddea555f30b0e481a9d5450dad17c588d70241498a524c3c17f8d85a22e2a87762" },
                { "te", "ee70355f3c5355cc10da115af41b88fc066629351059bb2c5e8930bcc42c3d9573f9e79726b437fd9f3b7dd8a99370a57c21308d6462a8dfa420a9604ee02da3" },
                { "tg", "58c80d71ca97c328270f0201406b31919745320e098563c5f39a9dded95372c32e95ae523323435ba1a4716378cc19a3381b92229210acd49b722ab58eec5a69" },
                { "th", "f2c3d03f0910370cd5b35a56962a561aee47a02469e65ee3670e1821918995b518f25d20df16714bdaf15f5a088a304e03191bb6c0b2190c67bde2845fa8e5f6" },
                { "tl", "0b5c7ba7d6ba7c93e0a6ff1c39415a2c3d700761438a1ca02fd1e83d736b3a21b1764b13853d53cf1ce87c5671255c955e5936658225212b5790531fe506a4cc" },
                { "tr", "67d17cbd824f16a71bb72d70620cc0d561b40f6a3f7949615f94b99dfdb6956e9ccf51c0be043e103793f42c4d364edc916d653a8c8e5d5438510c3a2750579f" },
                { "trs", "cb20fdc0933262d5c37567a06ab6e76556e810fd33843112724b68feb62e098251c5b9bc4684e7d8fe73e89cf0c7000eedbd7484da6161ac1f1d8a30445adbb6" },
                { "uk", "50eaa366ee7f26a88bd4774f779494e1c09bd8aaded28b06ae62332ec7896756c2502c9bed9af79b531b7ae21c2f086ebd15c810511e2a3ecc1b241e5979f045" },
                { "ur", "43b048800f1f244bd916d08bce061a97a174ef6b64e5b79bdfeaee295f64a5182705429ee83f695e7d8a54dfa142a2a3732c41d71eb9e412bd098f85cdf41d86" },
                { "uz", "74592df0dd41d7b5f06dc459e7a8ea9faa98d68bad415ead8042a05dd31de5f1cc28e56dee765c05818b54087e55c3b461e0e0c5ea18f0094198e0147bf6c6e5" },
                { "vi", "5b6a05ebbf850e4ced0a96577c74b09118b6d927460bdf045aea5af4a68714ea9bb137f761322e1bd50b56d6c2fa6ed635645cc4800576ccfed24415b01d88c5" },
                { "xh", "36be50eca4a7a58023c61bb90544602f8447e375c38e2c28c63b3340ee1d5f4f2694741f02e111738f22db76bb03887381365ed2f5667678a9255de662a1efd4" },
                { "zh-CN", "d86e178eb0512ead4e3648c3570ac3bf5423fd171d8f8cd40f74d15f5455cbc7064cb543e902296a6af5870610fc20f39531e4013747c88da25bae46b0d4502f" },
                { "zh-TW", "8c28f3c8e0eb54c5d25ca1f00fff78228b3dadf1dbafbeee220bec1e448ecc6155d42136cda17d3d17ec7d2174d493d8bab7a979cf5a612ca3e0c7c7f631c525" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/137.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "37e2324e15d8e0734e2f6d1e219da92f08a1b7356a6ee995f74e21d5211cfbb80852d53b4f8c5982ede6641fa951f008f79575eb35762572f8db47735a301e96" },
                { "af", "9b27423a72bf4b948ce213b04e5613793e5d517dce7b78200104afc8dece3f15e2cae3171b9aa5271f2ca036cede46d106c781b6f71819b859d6cb449ab78dba" },
                { "an", "b52e8cc3e132e89fe5edfa6047ce4e6825045774ab6edeebbb5b2ba98dc5fcd64b26efc44ed709814a0d6ecaeaceb79a43cc43136e3e8f011ba4e081be9dba7b" },
                { "ar", "0feb756e0b769f526e6229b91385e194f6cbb62a177f725f3d4d3ac03455a0d46c0b146db1b81ae7639685ceb870a49275e420c72ded33c6ea5fe1d0d4f6d227" },
                { "ast", "d7107658196503811011f06dc3af3bed860f91288f60f2d9c4d770f5e07c4e60094eb22c19e806ca56ed493c50f7d80387866f1460cc5f9a4cc826ac2004dad6" },
                { "az", "a2783366bca30ceacf59e0c9b3cfbaf5e5ea51839904d8257f35f899496d566bc88cdeb9af0eaa8dc4522c7794f6236d2dd1c4dcf344a514b56405e0be7cb05e" },
                { "be", "69d40a77da5c196f09a8299d873f56aee76771c818e9a3215f9872db599ff4040e9dc574c404e29c813cab2a761350e7d137a3c92dfb71c00c78456f6ebcbbdd" },
                { "bg", "0912576531e6a80e05fb420cc9498bb163f376f52e771fe8b33220cb2f578d0ff244f2b6d2ed99511b1cda35933ffcaf4a1a47c61ad29da77fcc1c37223700d8" },
                { "bn", "ed51b2792d799335a31798ed73130cfdb0508589784dee02e0b4a41b1902e7d2588c9c1a24ffc24536549bd6d25b942df0a4e362721859d1960bfd15291ebc82" },
                { "br", "7dcd420f8e2d848bb0d3789a6927d8ba2a7c57c0511574069a3c74f84fb77fbed904a92d383d7527cbedabbfac6700d3e497e6502c565308c3bade9568aa16d9" },
                { "bs", "5bf410ecce6ef6de0dfee01bab0a0c1d3fa64683e8d012729bae6c882847e3ba2a476110f938e702e685f5a467f328b1ae699bba18762d60dc6c4616fe5392c3" },
                { "ca", "bf76f8e4fc1093bf304b56e9a437f0469b19591039a2c0f2457a47ea3113579f5acbbf4314d26bca85684ecba183ce5295a4f02950174f0b3d44df31e7d3b175" },
                { "cak", "46dc74c7328d47b6bc4a8a170e5d7c870fbcf3cf3f3f1c31902a5aea25d29d2ab367634340d84908ad46b58b5e5b8f41839de6ede402aba6437f41e678586601" },
                { "cs", "37d36f4aa7ce720dd8af09bf41f43239c66151dd6b7378c43e1639900696aac9ba4774444306cc64613dde74af998f55818da44a5bfe0b30b2f0471bfa6b66cb" },
                { "cy", "7573af3ec08ea65b72cb5c052ab47924ff1d73fa3d1b771c5eaf1672a4b3265cb77114afe7ebd40dae570170dd380fbfc0ad6fd56a070f87ea05484f6125843c" },
                { "da", "39bd504b80e1dad4e2f359e19617223a996c56110d677e92e132d87f4cfd6db9cc9eab6e74c5fc737e06c24cf41fe47dc36325c936b8a7b8c4fc1098b6fd9fc6" },
                { "de", "d62350f553e076f7f70f613454125effeab8d752244448ee809be275cd46d2c9bd54784c372d0ca8ad8c43e3fddd9a25ed9166c5728ec5002b6320c5474cd39b" },
                { "dsb", "cd4bcbaecfcb4683e0b34bf59683a69e9b3383fb577a9799eff11831454ff4adc67b757da87f47e9ea6fe7e40c5770e6f30e907259f138aee630db8ede16e003" },
                { "el", "684e5d48034ba58b5a444ea974fc70a37dbe5a6fefa93cae799d3825fbd6ae3d03808ba1828f8d0e9bffda0c574a7daf160632e8d64e1539a9108bc5be9aba81" },
                { "en-CA", "9ab90a37f72e3f1bd4b1c70c45cff40ed914f8d6906c19927ce8785ec719a356b5e4a2fe9d65c820a9f03bde7cc174adb637d73076ea0d1e5fee6321b4e16cda" },
                { "en-GB", "f8e9c097a80fd8dce3e0eaffa2a26cfe9a1eaefd595aeb34b399bfdf92edc2cebe576780b64b02c9f7748afd80551d28981b40792b099e7e681dc20a3d3c458e" },
                { "en-US", "d51f586192343a44780cd7a0a6fa5c74cfb2482a38bcc9ab12e4d389dae324240e00f11f09afe2cf8e6e08be9db83a785e938954bb4b065c3550090c8cb2a687" },
                { "eo", "341116991f5eef465ac2df522713faae2f472b7b9e3fd18223e000e90597aff909c163143f26e5b02128b6dcaae33bc19bc1634387b461f8971850ee284b148d" },
                { "es-AR", "eb59e913e36b1b41a073b99e383a6d71edfc968ea875d9f38a0c3450b33915568dc1b8de97aa2d84f05d840183ef7e1feea6ad6a1fea5216dcaa715cdc8bf02d" },
                { "es-CL", "5ffb14e0c553d69fe884e60fe61824ea53337da1f81ab202a68f81aa16d4c432e564edae8fd2ddab38cc65d8c3b8170b718f70698bdd17f7d13549b68aafa940" },
                { "es-ES", "c499f69b8cd5413e8797929334676c53720ac63af1159da98863837876c4ee881cfc8fad755d0aa38c8704af807dce99e36ede527e960cff132d01e43d04c401" },
                { "es-MX", "e1c50785efa2c7b9bf65f69d8280db2bcd6ad263b8cc61b70452bca5442d886068da8252e148227085203019db11c7648615c4b032103ddd8f929dd7dd63c3a7" },
                { "et", "da8afe1ee71a27f45de91c972e477c303e65b9a14aec6a8a6b530403de9bf4f027fe1aa325f5f26025f28457b21bd6be4ffc0efbec59ad411819d6ea96decbfd" },
                { "eu", "c115f4434086f2b85ae4539d502f7af9bfc23b8c1afcc23be310d1fc6a30cee2b8da137778eaf94ded4c4fff53481bbd0b3576cd4fa749bf8fbb27cff8b2a829" },
                { "fa", "97115edce2abec1ed508b396ba835e22bab6bb5ca93a7d3ae72bdbf05c5afbf44868dbec696b0355dfba9fdd3487be5afefb8c28395f1ea78ddf84ab7a783994" },
                { "ff", "f48fa349e4554686d39508049a6120b295c45746a94143ff76ff7f1f37eda386de9ca4fa62663e49ce2a9cb3810b0354375dcc475a7995dbb90ebb6dd19243be" },
                { "fi", "08a7437e26b801a422fdce1ed973bae217842fb8c00001bbd069583ad6dee67898269f80b777ebfa14fe0cdd690f1d7f69eb659a79b05ddf0318a3b6b0e699a4" },
                { "fr", "1ab19f7c24e9f14f754edb0cd097fe6ee317f414abdddbc3f5cc23adbd42080a00a2f0c9ee1199d82469657f8d46b9aa2e7f99f190f591e0adee0d76fb1ba4ff" },
                { "fur", "7873ae2cd08d67ddd5f255685c92864c47b89c5349189491ba6e110cc56da911b68027136ab30c30117b4dfbccc851a0d291e840d3f97ff1704102b77ac11f0b" },
                { "fy-NL", "af46c88082fb59f0bf217854b026057042b5f16a3f60c585530db246a66e9e3daa60029b42cbc46990ee7140a4d99f5bafc2413c97a40590cc9679eda8553067" },
                { "ga-IE", "b38cfab5baab3b1ed7cf85e3c82c0c747f89e879757a726bc48a8a277a6350162f750bc132d00ca5b83c29b278675473257e892b2cebc21a351971c3878d0758" },
                { "gd", "730f09257537cc5878d07aae27b2b4728f0c12e7c53ce34fe11680aa9256d8b3f12a896a6538452f3651ca14344c197c730ec202e50cbe1a5823cd94a65d1a78" },
                { "gl", "f9b3d7f91a1114f0c217f40b1f7fa8bb352f7cdb8339998de0f24a28434971596619728fe023e6d8479a5364f7f8b95a09e882617cddb5068787392d629ea3c1" },
                { "gn", "9321432757aa9e8455209e893b792e07eb986c034f2ad908b60510d6c8145261dc46ac395dd12f9dfcb835df50c4f4ca3a1a7ab685af3c057451977856c8f59d" },
                { "gu-IN", "9961b304bfa3a0010b0e0fd6af7f0a78b8faf07b8235d8f34b957cced23455e0963279ee0b046ff498cb3ceace6bcaa72ef5809a7761d360b47781a93c0f856b" },
                { "he", "81d9fb8c652e63927040a99740b23bb2ee54949ef896166236a166c273b6360a1bbf7a9675b58ae2eaf063f51a6a093322393898665b706bbc29776dbfc04ab5" },
                { "hi-IN", "d4fb395c0760288f5e5b46061689b2ac28f6ce5af6eee955e3999c83107609c2d50e8447cb13953cddff73ca228ac9394508cbc8b9e6c5ead990527ab69d947d" },
                { "hr", "a83ab56f90d6e0c288eaf0551c02bc8cf892bd116044f0b5b3d97ecd29ba5ab4bbaaed18786b44ebfb27f46ffc401f30290bee621b24b59c38736b27224378ac" },
                { "hsb", "8956224236e29297edaf519d204088326060b5eb064a6ba04932fded2fa78dde156ed169ae698d96fa91ae07734760cf8be93c4b3faf1ba70c6796c3542d3ba0" },
                { "hu", "56ce75c41e524a5833dcca5061e9715a2e93aceef647de2e586e07d038a8b0c45326c922c54108f3cbc1617ef000c5d7736b212de787f1b526e08e9a0bd5050e" },
                { "hy-AM", "5b3b857218f723a72c71528799333bcb37df55dfb3c5cd84d288e4f8f80bd4cebadb7e64064e961df6a627b91518c061151ed4435604eb7e3da25cc5fe4e389c" },
                { "ia", "923e6b58892624ebd54975f582625d8ea68d5e65b52d4d19d1a64d24d51fe1e51d506566be22e2948e605192e9bdabc30e00d3660f0f69e6921758ecfb0e4a4b" },
                { "id", "479ddac2f414fc0814d613011b8b71dbdc0a1edb0eb3777a8312b4271fda9d3efcb3160d708f68ea08eaf577f5d8d5c52a9a6e74a15febab2459cf0b7fae5ef5" },
                { "is", "a6e77123b3a5b8099ea5982418b5414811ccc97fe836735d714f2ee4c69897da408bf8d31f01afdc412d6d6a2893a117a3b30e25e67997618d3013a39b10da1d" },
                { "it", "de0749259e133e0a2a1313a9eef108a086a03ba08e175dc212b331598bece139d852d137fd4ba0ae0952b806219448bb9d95cae1d1bf86514d6b085f52eedc3a" },
                { "ja", "2da3c9b0b98045e2cd777fea13db03c280d202a6db3796a9fc9625f43e81d172e7ab997d3737bc13206036fbe87a2095891cbfe907b25d086cc778d0e65e304d" },
                { "ka", "9b562551eac0d9cf40a248a61d8ed8a1c6ecfd77dd81dfc11687a4fa395fa761a15e8d6c661240727b4a05edc2d1ef6f5d06270c83ab366d103ececc2f977c8b" },
                { "kab", "dd998af9f70ec3d1cd47ddbfb4c26cadba2d5ae384fb0dffedb44fbaa0871995e6e3366f2a0d9e775f4392115cc99b37276ff3383ae9581ade3ab68ee6101c26" },
                { "kk", "0e22de2dc575244a86e61fc244226d441988f63ce3407fb7b319698f865827765f3c4111cb89e004e99070459144d77955f5b1076f9c680011f8cc3865e8ae9e" },
                { "km", "80f831e4871e635a8233b55f3bd97edb5dd326cb9eb4f1c790ebf3d012e683f205e52bf1060bff330e54fb50ec000915322e3806fd34dbe911e12aad5e495a52" },
                { "kn", "f1757b3d35793a519c93056751fa6f0c209d7fc96cc05445dcbcec95f037fa72cd0c48529fa3d1c0bcbed7bdbef3759ff3155c271738be2fa9b543937ce4d74a" },
                { "ko", "f5cf048868d28d1d9ec100f54a33a22509d6ca986fe08d3b2deca8454460a1100e9cc56309acd35f2fd5787e672d3b07eda3ba2484eaad3d74822df3a5a1dacd" },
                { "lij", "edd8b5cd28569d850a88435d56b7c3b1674228e860cc7e24476994db4e235e2cb8a366731a8d779b875584df25862634f249e3f8b856ef5c75f3fda11233cc98" },
                { "lt", "c345d29c230f1e3358d894d188e2968e70d215203ae97eb04dbeb7221ce28600c1185aaf39dd500dceb1968c2d090085666e1a59dfe203085cb24ba1b0035013" },
                { "lv", "d5ff82116c95b5ae90d2008ae3e206b26991187f3001b912a7d98ba3150aacfbc90022b13704de321d9aff5a21613eefaaee0a7a4e42246738e2ca0409e626ce" },
                { "mk", "aa279653176c13017d5fb96c0cbfaa93cb9c69a52588c0ab69aa2576f84e60392dde90b149fc9722673c4d41e37fc0e8972b0f5e1d29601ee174d318c7b85e70" },
                { "mr", "19df75d4822d90f6d31d7f89bbbf75af64a3efd9ff7c76006aa77836d3cdb4cce6770a503f48c65dd3e317c4b50726d3735f0c9be5750cbc50863251286a9d7b" },
                { "ms", "9abc92b7642e5c05bc4c66dd11fb68a3705d34884655141058169ecdaecffd5d9e376c42c8f0a7364b2b50b8b599698423b71aad9511a19eda10b1556c57e6c7" },
                { "my", "7631674b8587bf8f0481e8949bff3f834b535cc6b0cd9face01e7658fe3a089dc99399cef1437e882046a8e14747bda8c6a6cf95f31f4e232c6e921d37df4a18" },
                { "nb-NO", "d1f4f78b321dd6c7d3397ec2428bd7ce95e04622bfd8e1bb3d3b2e67977fe2586f693eb57d6a136c77cca770fda7430d31d685c97fffa81420791aabb3688274" },
                { "ne-NP", "16a91410504737e27ff481e7853668b19594f74a2e6d9f39dfe90fd371db68edaff0e5ac707e1689e521c4baf27e0546ddac0725bc00578060ee07e944c1d081" },
                { "nl", "b1055d9a1a9c427eaf949dc76da0d783eaf473681f873d07432c31e11c602e5ca9c42cdc88c1997b9d4c87cd3e0b59ad9bc5de13206c00c0131f96b9d0966067" },
                { "nn-NO", "64fc0140dfefc36d17cdf205bf39a440188441897b041f48c72e8e5658d6be8bde9628ffcaad8825a633f4f8ffeb6d46a26f4d794c9d2cc6f79157b01157561b" },
                { "oc", "ce2b797ad5e0caaa19ab6497e79ba585bbbd00a0f3803883ba3b59b0eef2794a08dfce5d84a197bb1b9011ef5088145a70642dbdd8f1d8e305acac284b38f3d5" },
                { "pa-IN", "85834fe26429d3c78b735cad102bcb53adf3f7bc3b79b5baa6a030ee113af994017e7b10d5a060d30c4ca5768868ac06b21a60ad828a6b0474ac7330cfee88cc" },
                { "pl", "19eaaca7a85ee849709991d76580160cafa3065881cd7654526d555854648088d5a88c35a2dea160f85f362b6282850618dc8955313325ba646339115e9083f4" },
                { "pt-BR", "7937cfcab514f02b3b74172d31cca16d1af504c90a42b66aa26bdcf4a9a4f00826d0fce32ebb42e9c4fa8e239901164a92e8d31d7938afb529d381180c196eb3" },
                { "pt-PT", "fd0aac96648a4420b2c87639e08a585c214af3489560c3b1948e8d2bfa9fd710705d6958bf6febbbddce296c59ca17f35ceb6dcede8e94628e62d6007361252b" },
                { "rm", "e239217e9789e8e1c6e9b02b0211abd047803159f6cfbf582e05376cd0c92b8f866e4c233148a878b5ee50ef19e960234ef0d237eecccc7fdecd9c7dd549a1cc" },
                { "ro", "c980dfed2e958c8eaaaedafb3bfedcb031eaeb80981515daeb9a2e46ddb5a9adef1fad82811b74d4a403c58efeaef9d0e5ff9cd13e637b927cb073aa4604bfef" },
                { "ru", "20ffba9c235d38247f2625fb9a22fb539ed499db9e7514ba04400ee600a77904b2256dd43a1f58c5ce08bab0006c6a33a5cd76df762927e15a74040ecde120ac" },
                { "sat", "6bd08fe1340cf76b96f0a946622430c28b5f5deb44b605487473efb98f54c562dc5cb57c827bb24b7e7dd7a7e7d74a64fb0f34e7d3fd9b274dacab6fdb5d67ab" },
                { "sc", "6f432cc33caf189cee510c52b9470f7574f7bb649ccf56454a5c72d0b9640d1ab5b76734dbfcd276748c4bd123cd98fd124b4ae3e9f12a0d4e6005733354ddbb" },
                { "sco", "c7cf55ddec3a72deb68732fe87fe64b2eef995a83409c8df7a79b5e676d7f47a1a5aacd4f86c794660d492b9e54ec62bfe08c0ed054ae3a4e4d9934f4128593e" },
                { "si", "7e226e287d03236d43b1fd098c7103028f0ab8316cf7ca633632da69103a69e428eab2f9074eaab08a853418b609354369d1e10b4d4107c866d7e43c644d003c" },
                { "sk", "9be0b94a98326c7aba33525c5a3dcab8c28ab0810c879deaeb699401a61f9bd8b5e2f0f45288ecb319f7a8ec5238e98dda4cca5dfe4a1cd7a28ad854e6f595d1" },
                { "skr", "8dd179b330196414a1d8789f9a4b7b2d3d83fb53a00c8820fef05ce8ee58fecfe6f5ae012ea3a19087fc0993e1ba7f9a5e5180ec2aedb7a5d4a64a6f80b08f6c" },
                { "sl", "5a73d75ff39ab62b20a41a6710bd844ae38773dd0c38d325958c7419d05983f0fb9350058b16c3946502243de8e6923dac0bb530989929742cb7930499a0dfba" },
                { "son", "c9a7d18d25164c2fdaa2e12c3227cc9eb1e35547bfba4550ecc20406e573ecb2ba9365a231aa0dec43f44a3852b6bcc6dc0a99b61401ddd52afab471b537b0ff" },
                { "sq", "d2dba30a9461f2efc03416f2bf7f808f103bfc74be5c50b03aa4f4fcfe3b20b9a61f421df77f7f9ff00399a0d52fbdd70daa7654a0e5227d19caceda67a5d732" },
                { "sr", "dc1790ab36e0a09af7c522aa35d92cb799304ba7baa6337fa4e1fe064ea655d546d36efb5e155dcc257562705788459eabd5d8ce7546c402b5ab6d64f36056cd" },
                { "sv-SE", "fbc9a12f8b7455f7e1f57cd2d8f1cb1631c85f96d790b8e6f06356262c845381272712c1ad22677a32072461b1756937d3fd18f0a7d5e572e4cfd2547211da02" },
                { "szl", "1049402f22ecdd4ca783b4a3b381f38db8ba4f5fe374688f867236821f00153fbfa304826f2177263b430fe02cb44f4e6c96f4b2d56b2b4801c3f39d65232998" },
                { "ta", "68dd74b40a728c47a092b16dce3b6e541b284b404e7fce1c6ebf81d2ce91e7a977d7e981f16f50fcfc220c09b9a0200711649d0008597e9e8d34a736de616c23" },
                { "te", "8798b4c773fb50c7620c668a6a55738ead2ae8d1859bcd18197d93eac3df0f04cf28b7fd1d3908a4683e7e3e5ba148bff2349b6ef41be75b9d552e1db9b53192" },
                { "tg", "dad26f29a086bc08471e9a48748014d5244441d610b1af40cc79a0ed2710539f8be46c2d663e92ec4e2a3f1721683a47d45443e40f57c2122e496d4d628282c0" },
                { "th", "7229442ef7efc72944d6e670c77375a8c15c6d709bcd34e051b76e3257c2a2b654a6436eb63069c60a7fa04b75ee9dd8c270b483597f257836683dcbc7e9df5e" },
                { "tl", "089f8c139452ddb443887aded2189e87b77962607455188222de00c34d9d0cf008ab2d4bcb6b6e6373996b60965af4ba28c08bc3bec9390063aad7bb736e1411" },
                { "tr", "073ec8cb5a9c5ed5dbf6b9a6ac6cddfc9d111f8f1ffd8d3cc854d1d7fde6e6d8ee5c224d68c4f86ef8dab40d6859d4f17ba413448b42812028a86a985f6033af" },
                { "trs", "87c467a1cc2fff3e687639cb3de0f921709768898ca326272b125b4e37bb78f6a00a8adaa46f66ddbecd093824c73b013594885558faba2c7c17d590b3bb86cd" },
                { "uk", "7b811deeb3db288d0191f15ba565caae150993a0b90e3aaa3268a0a1c1ac45587510609267b70544d4a1383dcec231a08796b5b37352e6b2cba40fe6c71cdf9c" },
                { "ur", "e0a06ef8ab477ef91cefa32e18e012bfc7ad87c2cca2694e2ae18c3fcf2fcaea39d747d7ce2a1fedca4842d75f56baae78bebb9bed16ee7a7579fca5057857c0" },
                { "uz", "16d9655650626e728eb38670202e2feec7d5fabd1e0c63135cab8aec4292386286e9c9bf91700f4c8a7f3bbd084db9ce43640af8d0c8dc91f9500df561de0cbf" },
                { "vi", "c8e9c603d06880db6d621ddb04d4ec1a5139ac6df5cd209f474414f838b33bb8411618bfd24290cd6740dc3b66a026f87f75f040a919c0a771883c3bc19de7f0" },
                { "xh", "a404a448cce809117057d7346d7a847a1cd45178fe3f4b79a16cff452543bc687664bbe3f99beca4bebc9d92d88fadfa42f1b627665c75980c72aaed6f7919fc" },
                { "zh-CN", "dc013519feca94d31c25a96c6712790e8b935a178211d327bd3508380718444e129fc3345338012d2321761ae87459ed81660c05713334a695c7fcf9c7c1eceb" },
                { "zh-TW", "1f5504686ef1e907db1f40e30403d18f0469aeb63cb4eebc11f785c9d8dbe6d46fbcdff4230f79ff397230cd987b181fabdc2cc4a4d2dd079abf182f2b40bf67" }
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
            const string knownVersion = "137.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            logger.Info("Searching for newer version of Firefox...");
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
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
