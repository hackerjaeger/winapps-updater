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
        private const string currentVersion = "138.0b5";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "0970da3dc7f62c1aa5542b647c264efe4cddea4efc7cf99c7b79858efba3d69c4a7181fd8416f3241fbc04edf628bddf00be7707a5014d693ad0bdb15997849c" },
                { "af", "a77faf4294c7faf05d83a4447d8dc55f005b070b783a1eba219d177d3fb1c953beb8f1aea618ee8ffcedd456b6f7b50e81c24ce750e1d49b09dfdba9946e6537" },
                { "an", "0aeed6a57d8f95a2e74aa207fea07cc059679264564e7ecc14f4557dcf4a72a9b6eff58df6569ae3c29b3ca8bbc02bb125822d7512935143fb0b250bc721a3fe" },
                { "ar", "eb1cdccde6ee671fdd6ed18ebefab62fee6c805bf209a00c9256d4a663c627dcce5a00b2cbe7047036b10ae06acca042454f73c7c7f09d567b49de36f30d133e" },
                { "ast", "bcc6456eec7f46285f0062b44d68fe771dca35c839402e492092c9e43bd984d689053aed70974588b4e0a9604ed7b325e4687bf3a92918c3b4bd0f6c946b5290" },
                { "az", "d1d87dd9fb903b62ef91ddc5c96b023a3cb5d3b3b68fa527235e94fe3f82012bbb7621bd36420c660d5c8178475c0578d33d95a9ba561819a442bcb7ef73a4ea" },
                { "be", "48e450d51e077c19a9c54d8ca58d7dcacc95e59410a3d123f7597ef93eb4bdb4cce71b4512762f51a900241cfdf6fcba774c7e1b57e52c9a6302089204a11445" },
                { "bg", "23cd9586d8a0503f2df958599565d185de9c2ae4172f32efb11771f3dec49bfb8c0771caac499de83f8196df144e22e424142db265aa5ea91568e6bbd05f4d04" },
                { "bn", "f610986ee7241ac30e5935ae783a6632d68c9bb52f1719eb8364b450098b9d8aae088a5f9769b878b6922d99cba85155d1c7edf402f5b42f6ea482a42f9cf1c8" },
                { "br", "96641d63eeb3f8b2e1bdfe9d0e55f6514315c24860957856bcb82e83cc4194f086b2c6914be155a22ba5ff8eeffa2b7796b7c6d90a6be55f321b5a0ab67ced4c" },
                { "bs", "d6fa4c168b941d6751a63ab606da8c483c3d723f29722dad5fcd2f0ad0a8a1308141deaef0c07633790dbe83d61213e3d3efc8c8da0d20d26179e53a591d9e1e" },
                { "ca", "11cc384b527989313c9d64157d85a8ba566015ed8fa48845dbfd884c29860dc7c93073f5eaa534e8fed2fc06b9e4ee2dd1c495b32cc280bc1300b26edc2cce92" },
                { "cak", "3410207f7fc530284cceb38601193bddcd89f69dbe06f2f349277271e8bcbd5dae87a78a57592bd18e17d3e2a3d369e38f8a8d0ca5592aa7f86e476b71db9446" },
                { "cs", "8cabcd30f6b8875a7b210fa758353212e3d709c387c077ab43b4245a35e6fe94cffc323668888de3ac827ce67a16614dd5ad14b4b371207c99569efbd01108b9" },
                { "cy", "17e66b88f768f41f5a250d1c9b330190dde965c8c32095a87437a249228370d2e38668ef135ae417aa12d0adfda5fae96983fd452c99a018ec839072073410f0" },
                { "da", "d065c4fd1aa94c535d1767e11072c6a481bfa42080e21457f114185a1d45d575c9dcb692ea7801d8e4bed118421336486cd014d50a7deb387c0eb7dff55b84b6" },
                { "de", "7d55d08a53b220071c9667e062e3f2487dec50e9a164cdfda4bae99841005dd3a09d9db3b3c563deefdf33eb08b00d8e86077183797566b980505f94f12282d5" },
                { "dsb", "0e40964e5ca9f49983d37eb9f0dd0fdbc7bb656fbb2249c173e7df8035cd768796d44b65287c0be002cd8f58d3cd9fb24eacc5a318d241cffd2345a2322ff60d" },
                { "el", "3551184bd60d3182bd1d62a2a9730d5ce8978d4ef1ed94fff1c77f0bb2968944174fbf74b0297eea3b0e4b3f0f83709768759aed7c14f33cef9e76967dc2bcec" },
                { "en-CA", "1073fef976f54151fb5bff8557dd4dd6ecb9fc40b1fcc2c68a002a4fab40a88bcf7133f953c1f40e5e14db7e282dd02d8f33aff82998d038663b9468654840fb" },
                { "en-GB", "a1301bc58586a43b4760465709a711e057d900cc1d500e9b7a166efdb9fea31152527b27211f204f1b1f24bb76ec3c637bbf471173bdec2e6b7af7e4744578a0" },
                { "en-US", "2db0566f5b37cd012bafb110aefa076dc29d82a9a8b523205660718c13e9648c9db6a3fc184c32fef06c41a934ff465cec08a10667436a816354ef04183bb826" },
                { "eo", "ead507685227addb11008d0118dffb734c0e7e7565bda6211379011304ee66b0be576efd9674ae205e5d29901468004227bba2d36fd14de74c390a19a9413158" },
                { "es-AR", "e3533eaa2746e9d9ead73c374faf8f715f585e42ad3cca3d299fd0943b98382f5387ce7ff9a3b20541fc448b331f23a2e532ebbb8b992b4e9a4834253fcfc61c" },
                { "es-CL", "eb63c26d0678cdd1e30525d83ed23aab8373236416c7223e43c7eb95aa4154b33538692afeb520466b1047ea75e60919807abf223937a55759925fbab16b7e7d" },
                { "es-ES", "7152ff226282207b683d2d100246b11832cba62b564c08273fe7cfa93cec4fb1f667af6048746b5048266c976a0ac6543d177edcd497164ed0d0268fb4cc2b5f" },
                { "es-MX", "fd205253d0d7e78c177de5a6dcbc53c7be0ec31240a1a49d6befd36e4e12bc0cb00ee8f9875caa54133aa40fb386fe93642a1dbc57c4225bd06199611b2192cc" },
                { "et", "88e1ade44c01dfcaa90c3b0109b0452d3d4f3647be5971fd2097548af1dc775fd5b42061c08f2082e6b7fa5df870239fde1bbc021cbc4f04dca87214c31b29b7" },
                { "eu", "1a7812770d70d46b0516937866cf6cb042a8062f712d87d79a3868dad3442acd427ff1d62e6d48c7d341b5d5ae89138572f398771975448cf590ca873dd65559" },
                { "fa", "c48667078a41f84b208cfa7bbdca7d1464cf65eab1af22cf37402e715c7cf3cbb2c83fe9f9729a739f4069038bcb4edeab0269b0eacfb50dc96eead70deb4568" },
                { "ff", "56910ca7fa8c88e3869f69e4bb8c09cf3c2067d7df9edbe253e0e3010666075a250f77095c9fded427612ba5b69439e5983d61caf4e1c6aabd42bd8e54b66248" },
                { "fi", "329e5d2b33a4751f5deeb8cc7f18f6191906f097f5d4c075fe4a1bb63b2d2483cccd157538ef1bf88e581248983ca3ea169191f1577cdb7adf522c8eebf6dcea" },
                { "fr", "0c03c02d107b2a178eb56390197b94c4b6ed1c3d5f6e1be6f50b00377d63481c0854d21a8b686d275bee9e05b355a95c8b16d63cafecaaef866ad69224497bef" },
                { "fur", "33b8abd8d1143d991de8cada6ae89d0dbd96a89d605a9e1bcf6c30482a5155de10014cd3385722d336aa40c6ed33e8fa8e35f12e4879de4575dccb36b79a4d75" },
                { "fy-NL", "79c8f0bdb47028c3854863590934fe5269b66e0ffb4d198db26c8eb3b85d08dc62f601332c2101288272b5db8d5bbd57c35a8a2c3ccac41e41855b506014e57c" },
                { "ga-IE", "bceaf2492022c8cac3d6b1ae563b26d567edd33d2024d6d41a801474e4e18bea0db19f58a2bcfd637a2f789c7c10eade2398b7cba1f6cc87db1c70b83c8d69ce" },
                { "gd", "416ebe75d2364853d2749b4a65fb2255cf4e379d1e95c93d76bd3ebd2ffbbf375058ae671086c84b36dfb972b363d7b181147bf65bfff40b059145afcddfd144" },
                { "gl", "4addf41e2a8f4e54dd891c3eced94d094ec20f31a1a7674244d5f14e6d178d00526cab8a0b8a6341dcbbba2dcdb29786ffd77ec0ff33feffd27de50255687068" },
                { "gn", "417f89b2be1228410142505dc940dda76b58d66f6f250414c726ecc0813eca41b3e5b8452ad7ad82dbe379c9ed5d9add646066099ecc5b123e8f39fa4da82cd8" },
                { "gu-IN", "40cc8e31be8a9d52babfbb9d7c5dfde124d0d5856bbc3d941b8d532ae0bd984ec456f9fff8a24c5690abd602f0e5b67a5406d75f9cc074d6f90a45d1dd5b8197" },
                { "he", "551baf7c698afe463b082bdee673dc90dd3b0d20fd83727e4608e906f1629d0eb326de7681e18d8825be27d3ceb6b674443eed93e86893ac154ffb46e8482ee4" },
                { "hi-IN", "255dfcfe9b868854f8b70213503f234a7072759b7ee4efe742917f2ce8dfca282528647f586aa1bd8a5cb5787c6d9ab516c363e548f2bd72b4e69798c7cf9840" },
                { "hr", "7ba18805805379f1e676a4044a915b8e5796988cc102767db8e7474fee618f5a226fa82650eb6d93b35654da79474cad9e99a69f1f7f65eef6b77c0b25d71789" },
                { "hsb", "ffdd8f914cbf3f8b90cb1c04e4467a0905244dfdef3b5b20ed19541c9547e6df2ed65873fd79579f2649552d8d4bac5b048a75ffdbfd6d7aad9f839b9bd3ab97" },
                { "hu", "c33974c77c9a82002d4d832473857dd7b29e71951bb358b842eac2671167519bf2736668f2156a6e8481df76217b06d59790c775490b83cb5469258f7d012159" },
                { "hy-AM", "86b27b98540c8d5a0ebc2163930224824fa6f218a70e28e3fda49c657ad8cde887427ebe60b71a99bf499f5b2bc2b17208557441656209680dba1c0c27d9004a" },
                { "ia", "a5a5ca795ddb56c95dfae7ceee2ef3390f3fbb5f95223e3d433171a756da283a771982fcd6601a21796668f204e05a597310a5dc3c5dd5a1237b3cf5773a80e5" },
                { "id", "e31ea7df70ad492dc27ed33a711ba18ba04671a5dbf1de1ff9d15cd2102b94cc91233745349ba67c89385b96155d629da92c51503437833a2569dbfba8a8f4b7" },
                { "is", "29d644bea4ae519b257e4a6c54e03af5570136ce73e5798186b734e1d04f86ecd956a163caeb9f543b4f6a355cc41b509fa7df9a26375a53dddaee6d195fe8a5" },
                { "it", "d8aee2d34477aa3e219881644b3dae5b38265b9a069c17da0c1ea83a53ed9dc0b0c638478432401d07d1cf00cda3873b8531b6c4cb753466941e23ea4b321fb8" },
                { "ja", "e4d129212b47f37d2a8603e36a0e4d272b29dcd07f88ba3f261e8af7d3d9f5e3b757e8f9c68121bc2b060732bbaf3624e7ace952f03e385f29bf996b4e0dd7ee" },
                { "ka", "cabfa8a7a20720760b8bacc831737348b389ebd524bf16289beb6e934df8ee5903e6e86a4daade79059f2d132f88a261b255fbd29213bd30bde7dd05611bc044" },
                { "kab", "5270bdf5d14529846aa3225c7fc6badac7df66f8e4e7064abcd1b988dd65de3db1b54e9d777aa6fb64847def418485be8b61d7eb16a8c77bcafa6276b693d79a" },
                { "kk", "ec69f081bf6c4b0be2562d574ea5110256122e52c9606161d432df9281e3b25bf180b93f1b5c77198f718026b0043811956031997d4f98d70eaccf076e84dd24" },
                { "km", "983a38be0fb3a41f304e35aad53d0e5f6431dfe215a7bb41c72697633ce9a831a719b67cc5616a450f6409ac4221e5c5b9417d3acb354198cc641abcb0f02304" },
                { "kn", "effe7d6843da909aac93a0a1d47372764fa8154bcfc15f88887483a93ab295d2bb1cc44da02704f68bf59fab102eecaf489b13352903e828758e56011aac5502" },
                { "ko", "39f840f889f9d5e8ee0fa1a0449f77cbf79c83eb53b48b6754d980ef2a275a3109d709a62ee8081325f0595bcae15669464bb920627f1f36e6c357fbafbb94b7" },
                { "lij", "5c4d3a9e659affece84128b58e6567e944ac94af364444cfc3ced26341b8ce0ad6469efc1afda630c787483a6d7a1ebdb46b543e78b0706c323fbe9051d2aca4" },
                { "lt", "860eae46a198c574a4ae7d97926864e0416e9b96b27779327999e5eab15ee0f1219573a230998d02c5bf56e3aaed6ef1b072c3b6ed9fa9ffe3734139163a5a38" },
                { "lv", "b194d50f3a8093290831788ecc8986270c8771180f5cbc5afd57caeaf10ce9ff56d609a476b0e9bafe458435535e73a815ce5edb29f12ce361d04ba2add837d4" },
                { "mk", "5a98adf4d1415f411713904aab8b5abbc2aaf589681f24a6a91e191e2e52b8eafe88245b55fefd60ff3f45c5cd8b398938b4ae175c2a834af406f4630e7bb738" },
                { "mr", "ca559d24c7b5f66871f6e0962c7ccd90928617127085fba267d5f1aba0707d4e1a91ea893e453fc5f4321179f1909b44935e3d244719307778e84c1346efcde8" },
                { "ms", "fc2619721e6613c493194b545d3d9eb38d72eba70879510b831b661f7b36cd44a87f2d6baacb80f3def39de08900461eabe8ea95168c69d77fc8ee6dcaff7543" },
                { "my", "de7b16c30129965d2210a64569a1fb3ef59769082acb8aedc6244c25cdb60144cc676ce1bff67c3938002be1d4727b9526bbee272130899f9ae78f9ed88b7f8b" },
                { "nb-NO", "70087a26c5c1c42477dbca7c348c720c4566cd8e82179224a85b069bb25aeea9a9330112e053112bd266774a8967d4b16a07124cbcdd99c523d10ef2fc46400a" },
                { "ne-NP", "e88391c8de627744ff9455feb64ac9eac231e948f9249d904b034fc97300032b607227ce222f0190fac4641f804ca6bebaed0134306dbded3271746d6c9955c3" },
                { "nl", "aee111f1fb5b7c88479a2c8887373f03615ae6f2352f73ac79d73c5c9e1a49ad8fe8b5e7890b5fd4ada15899187f2547f6e7443fbbccce9999f67a1413eff2cf" },
                { "nn-NO", "12067b6acd14f7bda6d6e4e7d305b52959d49c0c424f2763b5e5dce5b38b422a9b8c06e39a8a73518465a67fa95ad880ac8c05fc91c8b61389b7bcb53eeebd8d" },
                { "oc", "5d9f15f310209fa11ab820c10e73dcc057b23aae33462d996dfebd7b78dc8f72c561aa4601c8a572e96f2b3aa8150c3c1e5dbbda1d258c0086fb6fc329195426" },
                { "pa-IN", "7a86110315cfb4fe24aa9189e865ece3affbe91cf33f53f70828b86d246fbfe91bb8863697c921f9ea15658ab464154f3df6c7bb45cc1e3aeb85c144139dd55a" },
                { "pl", "7f519cd7d210194367b37bd6629a081ef3d23da3140fbaf2a8a5fa5fc50172c33988a2e55e5a4390f12769457a0d31862ff17e728c856a1ea3d0ef88432cabe9" },
                { "pt-BR", "c20bbfc71381b4e51a4b644b81c6f07c1814309684a0b4b23f6f70dc89e18e4ecfe7e6c462e7d93470adfd19c9fca4774e9e9a8c62bae9671592586a25e28862" },
                { "pt-PT", "0b1b4f3225cc5147efe768d2e2a8008bc72dce475b2b3c804151605528dbfc956cd5ff38bc1e22e03baf3e92f779c4dd84165ac91e5fedca5b7174509742b6bc" },
                { "rm", "4dbc6b10e8271d27c8f6091d432395b85b9977834c084260657d35f9ebae5cfb99dac4799df637f53e012fb22b99b13f5061ae28442287ad46d779858e638c4e" },
                { "ro", "8fed6f372dea0b6daa69c54e0c3d6df3965db4a7ab40ba22f97ffc150b6d2374916d4609bb13fb1f303d29d94cb9a7cbe40f7d4417147147d352a74187ea6521" },
                { "ru", "c62d897d2170360a9fe5d429c2af2ea94e8a5f174dc8076caff115cea7c39a90c067e1e63cf8211ec3d7571f23ad2dd228b36d657d0f00c02a6495b819c7479e" },
                { "sat", "b4e28f831d28f4a866e65027c924b1cce09f6581def7f41e8e4504298db549bf4117673241b8e3acedbfa9ab1dd27a9b302903e8fec316f3b72504bfeedd5356" },
                { "sc", "cf0da9d147cb4a2261225da4d8099ef8bc0268934b1637692cad78eaeb431bb9e4b0625a36f8dbd780c0b09c390812806c5e412cbd877971162bb2fa1b4e7d0c" },
                { "sco", "0e85f190a4df6e94b780931f13833db88cbb7ff697d07bc43ac3ec4e85aef5c3e85355e3f2809eff0d922744e113927af6e271bb03f03cb53d302fd9568bac7b" },
                { "si", "7162ff885341e3877ad5746966dc22b8d2cd5836b1b5d5ee7ec1e5863f3f45fdf4ada9495b48d695180f624b09b38a5fdd2b6b36946f0dddc5a83f142afd99b4" },
                { "sk", "ffddc2db7d151d851897803a169753d1586d040e14a9c944597284857b4a185b1345fc3a6f1a78181a1c97541589694fa1624f14a26286e6d0e5a415e15a0b49" },
                { "skr", "901cf3865802153f0210798d761f657b965ec1a377de0ec3820a83e2eb074bedf72f45232a2ea074c0dc1af851b2ac4c04f1e50591e27b44cdc5c30ef31e1cd4" },
                { "sl", "4168522e45753e278a5b35541768c2127552a9fdfc9b1785be49a164a47b6894f881c5595fb4b1cf7ff602eba71d58615e34781048864df87b61c5c33c1fdde2" },
                { "son", "5b4a94c02bdf89acba42397e322b5fb697b1968b96f39647aa257825d5d0dd2ff8abef0f9e4a11f78b972ec0db8d45d17de264bb3b789fef136594dd0f3d15f3" },
                { "sq", "64c2e0b4adc9c1fd5ad94590bd699486c87889655e30f631796c716624c829d902b3e0bed6994ab8df0ec06387bbf4841a772d74c3ef0ce86d7706885a541d53" },
                { "sr", "51dff494dcf9c0590a743040540f6f529b3d74c9c2e09844e0a113932a0046b298bda3eb80ff060d932842c768f26d3303eb82a83ffac947f25906ca648271ce" },
                { "sv-SE", "75ef9ebd96abb45a771c9a96f65f19e95eb5a01b1f1a1881d3230d239d0f57a26ce0ab728310b3bd2937b32452a129dedac5e2930396dc2fd2b83556e8f4d14b" },
                { "szl", "945d214575080f30cd06a6c48a4f28393db83bc3d016acecf9d03b3b8b3d9041a80434ada1a37afe2667c99373ba36dc7c2eef4be2667b3ae96be6c2e848b09e" },
                { "ta", "864403b8420d54c22154582124307ba24a3220951b8f84127e5a7535782412c5ff6b929ac62a1d78f0ff131b49c5c0f21095d7051ec7b6d31cdfb7605bc5a588" },
                { "te", "88e97a80a350bd5ffaa55e1545476d921b2e55e2cb6e33a264a98c8c7c429fb7a650af188906494694d021c72208d8ac462deda62abc1b9092ebf7915803ebdd" },
                { "tg", "cc15a4318a418e4eadc4fecd0ca35f3f94e9a26f3395483ea8635924b531dbce678fa519ed2f4dae64c2091e7cb5656887386fb08c70522b27f6345ce0d2d4fa" },
                { "th", "5dfa2130e3d1f4a5eef2f91a16af6514cec02f828c402f7ba6ec393e62f36de1063b01b1e09b4021f25c358925d615dae3ec4de6fc2a3f94cb539b18d598c6a6" },
                { "tl", "20e723b7a0a139bbe77298d88190eb4096c7d0147dbea6fd8d4ee293fb40353a185677c5b1b73eb681bfe9228e98659defacc333b6b677141a9af7c638257d57" },
                { "tr", "db9c181705d1361f1985d45e5435dbbd4bb90f5d82e4261af933d8804ba364d6e0ab22b16ba1598d458ff90d8fc3cf68eac20715a70a5ffb068e06abdc038ce8" },
                { "trs", "899d1310f969fc845f85aed7963542b0566010ed0f9287561e4cb2130c1a8fa6a353bdec241da2304df83599203fd3a91eb7220834fcbf4b08d718d97493555f" },
                { "uk", "8a93adb749141f9accbe510423a0887e66642d56632da8fa0f7416309c0f2fe0d4a81f35fa71924d971b2ea25c2c2e38b5341a53e4aa2d80f8aff3d97316f00f" },
                { "ur", "fc0db3ef15083c85696213b196f589cee8b09c8a670388dfebf50483b54eae358db0f84c6304e749038872171e8cf0b59c26feaa36f3c042d76041f1a101b9ec" },
                { "uz", "06b7ae1c2b6e92cf395851ebf7c6d4d4693b856c6ed67a70066aba41fbf16ff6ba997bf7d73f082a47c6c7c75a234e17cde6c09c31734f9261c43aa44c7527ee" },
                { "vi", "beb39f4bf2e8681169b79146b9ebf4f02b1a681f99d7eba0aa230b0674c800b382c26f726e397452fba36c13cc3330c397b776f3036315225c6ba5a01c1ef763" },
                { "xh", "a441ecb49f91ba6e223417a3634a2f7cb9d480a41b3d5acfbd457b51a3644fc25b9a3256480420dc537769a9018b74633c68853b3343c022c06bd28cb7283b1f" },
                { "zh-CN", "b349e29470e1dbabde95566fe1edd23dbd649d17a79f7e3f85fa89bf5da0de46218d67174b9eae7a3fd5010c46feb6c6774b9de427f5428c012840006edd1d2b" },
                { "zh-TW", "432ced1f69f411eaf30c75abda4e2fad8c28baf1fae030d0cf59e90d2ec909331f308f5b7bd5cd46f85bc7f3e8a35b9d8080d1035b4b5258ca5eef3027d10968" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b5/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "f183a2d2630eb0dfc795a76023da9ffba75924b3ca3fbc475161b672e0de932a6a372559edf7916a1e1fe1434ed084432c64969890a2b87cd555e55a7ae5de58" },
                { "af", "671115d2b52600d2dd9b009878ae172e6f32608c0be29b71605d0f6999ca20e0f2cbcf97a20403aa186a4f1763ffc9a0b2e12e45f4b25604dc5d5ab24caa76a2" },
                { "an", "cba680fa78575c050563b9968e03c239cfe20113010348d540c220215ad449cda45324701383352f3f3cfecb35e6f920bef4954242726bf3e61956ed9e48c078" },
                { "ar", "f403132caecd90d6c52dd6b3b5ad0af07fb28f5ead47f6b4f61f864e65f8122d7f005563e675b2a144d53aecbcaf73c21acb87852560a2358ecda8656ee665b2" },
                { "ast", "0f39e4428019262d78037faee8ea90ccef7f333e8c82a175dc1535baeb5856af86cc07bc75fa9c5bafbaca9d0cab9b9a9f27a4e3526d8100f985ed1099e594bf" },
                { "az", "7cfd520dba0b6ade2dca0ce8035e758568239d581d7bc7bfd1ce26f05fb2fac7abd42f544183dda8afaef006a9389c32659f185fa3c3f14202acce0ee14081a5" },
                { "be", "b17c457a850c63b0cfbdda06f3dadc6c520263fa6545c5faf762d814e00e85c19b58c2928899cc67a275abcf5252f9c18256efa89b1d79f69ce2ef55214246b5" },
                { "bg", "dee9b2ad54ea4d57ddba875cd4e4eb6fc4b8092bd46afda738c16d1d2a59d8351a3af41a8f2307d1a7ea7f3fe56d47e38586ab7a396461cbe687e06ca7ed7fba" },
                { "bn", "0db63bc2d9aa5d9a59539b3c5f3bac4657bde3f99e049119d97f4f68512efdc3970a60e62f2cd0cbeeff8bfe5ec703b3821cdc1fce0978bab4bf5d13a78aecfe" },
                { "br", "47dfaa2aa465d38f07b9904cee3013e4fd3f7de0be5b94621b7f479f6a8bd59ca28f45d8fbf04b4e287eb7f6be2adee67dc63934426126e143840167f1b47849" },
                { "bs", "eadd03eec8af308e6a907f78ce9cbbff0d74b8e1df1d894f2cc414cc631c9eb8c14f1ce0d282d81529be8efe50579e93336bc33e5220e3c69207e96f5223e635" },
                { "ca", "fb920fa8ef9e8e4247ae4ac6cbfc01a48c9d39a40f55ee2079454d2a707b7bfd3c78c55cbe35a7cf59e591cd55d07852aa1c5732206a4127a3a35aa257e43dc9" },
                { "cak", "e67bb78c7c8f10cbb4c1514a9d9113390b436c896f6cd66d7c28527a1144aa33bc5e3830753be5b9b87640c00287c8bec68a35991d5e425873b21901fc9484ed" },
                { "cs", "8c8dd8d1016bb33810d3d881b3591be63cd341e2b3bc3271da3eb4133835107cc7ce426ffb73d9fbf4d788cacd5b4e42f7e85951a28fac54d1a09e9b85f852cc" },
                { "cy", "66d31c7f2d031db5d39b3de64e9fe24bab5cfa0198ede0ebd244fef5d12b4e44d080b11e3ffc64c058004ce3ac2e070b9245ff96f4e9b9406956856be37af1e5" },
                { "da", "764c245e360749581d904ccc07ca8e9f169cdf8a5055dd10d1fa93180b7e17450d1606df9384d05f599c5d7202e56db1a6f3ebed0d6d76c065155184f455009f" },
                { "de", "87e63faf8e6ae6e51e35a06fe7e56e1b062a6d421a7d83cfffeb4c24f2654af63c166927596297eb2b7493c534d53e1d852ee3ac980e7ee15c87c12fee838e38" },
                { "dsb", "217f1b4b533b09d6a2177865f7a89c3bbf3d2be8e6d7f26e90f9e5d434b2abcb966197919f0e8cf3f28813c7e0690f61904fd5eb1211dfe392352b0db21e0dfa" },
                { "el", "9b263b0dce88ee20a5b40c48d90f975d262b42f982724bc935d958bfc240fb12b0bf5f7500429c37b7d296a0c90f4ef4143b088acf00afffd587fa6b22327eeb" },
                { "en-CA", "16ee0292c95e05f771f458729e60bf87b8302a35c0cf1b1bb9014318c22d05c9bda2750420f8be81d15c93a66b0ee552a036329553573a09c2ad2a810fbb3336" },
                { "en-GB", "cd035966900af74378578bc7d68bfb00f68a49da837bf25f5fb262f7e580ecd9ffcff1a13fab5e4db0161601f16901d5216e16f7c1f2702898e62ac499f7068e" },
                { "en-US", "156f00a91391df6e28660dbd2c737adbca55e53f622af4a4cde0e3429cb8749199c7f1ac4ba52e93403389fa59fb952c3bd8763117cd68d1039a994a8abb50a4" },
                { "eo", "12a0ffe3b16fd6731946031f58c563e291c07c3eb9482af334799161f16a79eb5af42232cd148134b210a5c6097f294287f4b0fcfe4519fcc945b0ebab5ecb2f" },
                { "es-AR", "2d152c29a3ef528fc6e750238d3066ce950b330cbda75586ee8329cfae0e0ec44072a89c34b9e9da35728a6608175684c23e02a2806ec32dc27e4334045d6db7" },
                { "es-CL", "6360bce7bf21634abca48ec55a0daf4f35e7a1ce52b934f94b63c2fbf2f3b04011ce72d4bcd13c9faa5c5ae50a7e42c27fd58aea5e6ca2d00d9050898e6cdae2" },
                { "es-ES", "18f6585bd79c833914c0e00e64397d24fd439a84dc07bbd82e43f3495b2053805c466cb245f2446b03594af9e62558e0b6c99203f4c0568ad3c7752d57b3abdc" },
                { "es-MX", "701c55d6313c1dab72605483fc75c0d67b8a1a541d3f911c6c897cffe8cb4b871c92656d33e95141ab7622312225850dc7ec46755909de822f3297902510aa2d" },
                { "et", "02b169b5a083c7b240ef9efd84645e307d896de17e55f13d259f2789c79f5ed9174d8cfafd0b4bfd7e957d8b16b496b40c9b67a1f4b8d99b5ad15bebb936ab32" },
                { "eu", "87fe66bac3816c74db17239f8d99d7228aeda0bf5e51110b78d8001c9538ffa928eabd004d370014d8755e17697adc1bc449f7fb4ecb885e311e5bc135db7a04" },
                { "fa", "26367272e37ae189a7e40f4d496d52bb71fe080109ad89a801104e175da4524b3dddb5dcaf33447435f896ee98770ba6c202a95d7c02a5844dbd9629d180a849" },
                { "ff", "97303733243c9dc423a774726c644479c4cc5ba393c2a907847c1d21975bcd1f527daafab08b58f443cfa1a8da564297a46b5c9bb00bb0e487377342dcb23989" },
                { "fi", "6fb7dedaff424b1a50bc9c04e5b9b7a1f1d0d20e173ba48d97f5dc6c9a693ab697f7eef283496320888368f7b5b9a2d3146b633f27a6ac294c855600c35e4b53" },
                { "fr", "8d85b7706f5f13811d3a28588abbdbdd7f877ee1350f946bedfe5b937c2aa8f33a73a0d0bb55cb8b780a74d3710c8e23166b17b3b2f1e973256c38cf81439cec" },
                { "fur", "3d0bec9ccfcf85ac02ee24512bb3f3150619d6722fc8feb810f8e3ee8fa9ee9484129b2ac7423057b6dcd70b1bf683beb53ced8c863d4e2befa28b011156a29c" },
                { "fy-NL", "8f1ba4cf5d2c5deddcdd86fb0474465e5b1a739d4adc74917f27ade51ac86e342b0c2a1541c7b431f6e36a3a3610fb6f2c3111cd1388cabbdf7a949dab53e0e1" },
                { "ga-IE", "ae142ff11e66596a0db012a016c4a245f970010b94a3db2f382af4ccaf56f0b7eb4315fdb3acd1775d55fa42c9ca77439b89303dbdb00dc5d61da57ea8d5b52b" },
                { "gd", "1f17c18f271fa421248edc0135c1f4a2f07972832f9eb3c99f7650c12d79d799ec8db52f8b4069c056e63ce8a0ced010e4fc279ed5f1f7f36bb0487b089a7322" },
                { "gl", "4a94f6431f1fc4323357c7541efdf602c6a4e2fdf28baa028d6c7ddebaf47f85cf3fbee3900dd7efb713373b8f944979ba67951ea6bb882a864f388bbd3873b1" },
                { "gn", "9e0363afc17d6b5cf7b85e1a1e21d77786f153bd9284279920520b37b9c9165715211de23584d4dbb7127f97fcd4de91f901e163e1ec46a5d7ed49245969a71a" },
                { "gu-IN", "b91008d3ac3f9ecef48811ca1d9f9d12161a80277c938332b4a7bed6643508357cf1cddca5ec47d3006ad50dc730c29503fb0fbd6b5a8dfec02da772135b130f" },
                { "he", "f378e97f700c5d0ea8977b26aac744d55884e51f1d93ff4cdbc9c3d6854a3b746292b3cf872fe23013ae1f1d3d3793e5fbcab9e6024bfe05c189debe9b39c701" },
                { "hi-IN", "6aadc4eba8559873c79860041b23e5e09cd585ee8c0208b35977251d477d4099cfc39bfc82b8e306320dac03f1014a65ba231a4c4a00488c3b86a01133143a66" },
                { "hr", "30c59db4c20dfc300d9436bd8ce30675d9b2809604db84572ea6045101c5b0cf721c249bd49c9941e0e60e828e08901deded1eb4c599c9169504a556bbba0f29" },
                { "hsb", "18feacb6c67b5cc53930513a420541750adaf479349603ea392f8a4ca193ca778567844ef46d6b622a80eb88f4de39db8e76ac78295678246c78966ce9b7c45b" },
                { "hu", "87dff4e34522436c5d4a3518733ec3d45eb0e94b167e6a173bdbf68040141f1105f97af6453186255fbc22b039519e914f8c701b57af5d4d17be45055fe16e5e" },
                { "hy-AM", "b947e9ca0b74aea88775ba81b279b3ec7206139392489351de5d8e1932427b98115fc59398806fb5ef0936e57bbd69ff06bec1399acf700dbaa67f3c98e13402" },
                { "ia", "a69cef18fa145328eafa4e777922bab29942091c9e37c6337fef0e08c925787a52a86b65adc516bdf3bfa152dd19ba9d02edfb64b449e230fe6969f147cdaea5" },
                { "id", "9ee2b08551cd321c468ab273284ee852536967851be8ffefa6c731be866524fa3abe1177c1634c40c254a91b41d321f69daf6b34f75207a5eada739f3be08884" },
                { "is", "e309611d715970c2228d016400b2562db7fb0821ae6123fc3664546eaca75fe055c4381ecfd9ad1f0a9d03cca693b176d558222e4341f8dc153b76f1c9b5f373" },
                { "it", "4a879e003967264b38eeed49640aa4a03685924036aae9accdf08f1569c39df4d2a590db0ea2da81521b2c2bb447572ad2f674dce08a25f09107aa899294674c" },
                { "ja", "0f4ff09b5ac6c1372063846dd3fc4acb33d191effb8904a2c0ddf1970177442bf60ca96959553ddcbd2fd3913013fc5f7f4dd4bfa9b19e6490f8bcd982f6f872" },
                { "ka", "39a4b3d335f93aa52b6960dc58db74c4aa411fcfcaf264567b96b7b686f7dd4020612ecb5a816ec632e7933d7065b1da888e69c2a015951241a49096d89a6c05" },
                { "kab", "848408eed06be0051831c5624af12941a4775817c465e5033c1cbe6d72c2c807d6f34308e301cc88ab08567c7c3f080da22ccc4b626c636c7e77154e2d2e6fd0" },
                { "kk", "52612a4ab5b779af30218d5b1e80a8060a0106498c5ada809e62fffaaef5d1b6ccaf0d965ca4ff1b25e617490793030aa24cb6aa5e06554beac7735844f1d8de" },
                { "km", "2b3c2186b84ea7bf8fb06f6f6b91bc6027641f9fad0b7987c285404f7a5ae11334c7818b2fef1901ed79569a2531ec7ee1c3b0b46dfbc2944037fa1af50b95c7" },
                { "kn", "3cee8558a0cbea61aec0ddc009fb98214dfe7feb66fe614fbd8a92bd05ecea172cc408ef34f27581be0dfdfa77a9791a0c3c85aa36d9061b3e202591eb78d38a" },
                { "ko", "efae5f7e98648746b9a4b03c43027b04866a3498478d8d4aa122e12af9569e904524c01cb270aff738166390fc3e319ec4e21579aef621c917699103e9e967be" },
                { "lij", "a1c6881e418345827daccea2a9078d67da557c0a20ba5358591fa0fb0c8267431617e55ee21b09efce620ee49f65321c4826f3e94a4b636c65ed3d73992d363b" },
                { "lt", "5cca7b60eeceb38013c2365e5214ac7168c9cb9fa21237a73845f195686c36dc05f780d2d86e3a0fe23016cd2bb19108dfbc46592432e15a4b59e6c3a1758b91" },
                { "lv", "0e1e0457b664b144ff367c6a2fee4cff6f83791a1da68e089b9e6564306b2d0e5bbbbcf4c6c7050319b4c8f219935f83c4761c847c331afb59e43b7d1b913a37" },
                { "mk", "d7df9b714d778cd7eed6657fe93abe35918cd1ad93157f3763937c4d383ee317293bf7e53066b33a9b08b94fb32ca52a60a80206de4249c2b74cb75b493c133d" },
                { "mr", "668120b4fca08bd7489befce900197c6b44e877c210fa4bf45207a05fcb3657be53ef03a5e57c8be898fde51e981ca00aaf773600f081e2f82d425cbb3c8badb" },
                { "ms", "26944c2be254c322ba22bab07a46abf0a701c342761c5881bb9211c50afe27b23390dcbc12664b43ecc29f7475dfdda7fd072a519b0c8310b8909999203a0c2a" },
                { "my", "331816af69410a6eadd7eb6646e87c379b6f8da32c0e32a646239cc051c50fa3b8afebb73a6ebe76f67dc0b4e65a8aa723c190dfb55104e3db6e0c0f27516155" },
                { "nb-NO", "7ff2cdd844dffb1f18c1b8c89a3dabe8d727a1877f78b08149be1a8ef2d87624f8f1ed35e528dacea7fbfac3ac03d33bb79577b2c8ec40f90e7e7bac22d05be6" },
                { "ne-NP", "f44c716c31db8826604e53254b8859cdbeedc103093d8890521ff41308d8a2cc9f372f4c5037f5990027c552127118b09a65979ade7d70f426a54cb5c9ac6eca" },
                { "nl", "bf9efbf41460bd8fae3840dd5fc55b70b4c2e0a021e128bb07301c18bde78fcca81fe1cbb874e3abc9339980f7234063b130b9c0638e7b1725478d23353c348f" },
                { "nn-NO", "1b5c7ae4811ce5722fc471db81764145161632cbf1711bf94e102d9119604c1760eaaa07b0488d1f3d3694cc02c60cc77706439f3ce061497c33f346171263e1" },
                { "oc", "f1d720750df4ddf9807289fe5d9f399e5816390ec3ac00062b2d44b87a19e5cd525fcba01ba2292f9ef4d21c175cf543914e72fa461f67ac623dfcea65e8a779" },
                { "pa-IN", "39e868fd0560d6f8baf0eff09c0ffd29db437fbecc3a4ec6eb7489a975d3dd9564fc2baee55f1268679d1052bdacaa49a57ad6a6c01df4cba0d2b79643d15d11" },
                { "pl", "732c7bd11c1c5f04a12a6429150afad4aec7299f1ed8712b4d74d0d4a10029bd8781fee67d87cde9230bc0e2f9c7d980748b79efcecc89b98aff28c6074d1467" },
                { "pt-BR", "601897fa0528493b2600048b73e55c9c83ec6008fc4d69da669afd4f8d786ff1ac1f63412cd172cfc810efa84881696659b88cfb87fed19378a6e685279be168" },
                { "pt-PT", "3f3b6a5a4e3cc4e93c3396586c9b7860d8559ba7312510305aac885a5862f89813406b81f5940a2dfb9edc61a7aa75a5559d7db3eb4fc29b76f8010bb13ed039" },
                { "rm", "cf6f61fd3a6f64945097795b3b0ba7a9c0b53cf4a7ce61514c171d34e006d4bdc18e906cef39e36093e9d413360620dbda5aa120eac9f60e4361569e91b1d806" },
                { "ro", "685fa5674e09e423c23db6349c7c1fd29b6584636041ed5b09e24fb83cbba328619a7af4749034693be26db336358acc2b3029f4b62fb634d4800c83571b20d8" },
                { "ru", "aa1054a88d7b67810a7f1584fd46d6dd91072a5ecc67082c0ea94e3d3af390f88b946228028c64552128e17184eeb5e35d1ba39a0629fd8a4497e31fb0721bb2" },
                { "sat", "75cd6e79c47df020133ce132d40a4f4f75bd1280e4fd295ac388cb75c62a3914049761d4b0ae0f65dd02645e12d75e73e7005a792adb832699f5aedcdeaff5fb" },
                { "sc", "2276eef0289d5b169695710c7a84bae6785462dbc07871b81007ca241ca8504c16a8206744dd930f0ce3052f2baa9cdc6b723fc1dcf2db208a0d693b9684845d" },
                { "sco", "95908a9d316f0fc8bf70362dfc1e6bf44c506d53e66d2affc23063ec265844d9b0f07f8e27e07d451550b1fcca063699b5ee9dd162d51091aed13fcfb33b2fea" },
                { "si", "d2b93d0d918c0ca024797eb2ad468def208b43ea2d4546c9cbe222d9e95138e914365863eb281009479b384ac6d1435935a39110938493c1c5cb3044221b5d07" },
                { "sk", "b6ed561d30f8d42f21995567ceac92e34a20d40b440adba49f0aeded73ae066c187211915467094cea9646adad447e3d94c6cb88260bb0f52899f57ee868b497" },
                { "skr", "b3c50c67409e709aa3d1788d28f1208b5f938f9ef88fe8a1fe0b1d1bcc3f51e2eb731702b485907fdbe754e524ef04625c4bed16de15330cb15801edc4911ca9" },
                { "sl", "95ecea11a63d0315dbbd35a9b2a169b4e3d601b7e003d62fd5776cd816c953d43c6e9f621656a802336dedc2626807e85c573b17a4b3c5ddf0bcc3701b9599b9" },
                { "son", "1b66db8c0bc3b202b3a659ce4724647196abae24533e1078b69c817d006dc34646463dbcc9be3d8d65dc4025fb2052841841fe938adb979cacc0c0972b5ea70c" },
                { "sq", "b66eb1ef53c8e2260983e7aa988281980aff52c7865b2e54db87035a111f53fdb50024f2d0b1b8e61abffdcd504e027901d79f6fd5b57c47d126b72d053634e7" },
                { "sr", "42d013ac183dccb4cc1b44f46ad66dae73c2859de0a2510a6317f6fc86fb5ffaf0080d9652f4d44b1b71a90cfd2a7c784cff10be917073d4bbda7cc59bf1e0fa" },
                { "sv-SE", "53bd5f0d1173ad14d2731b0b80809681c3a513f45081c1a96813921b38d18bc45bbd2b9b655106df1110d644d973e67f0389b630d072a41742e8bd7e7c0af6a0" },
                { "szl", "023c2b4b14754651df095dd06c30f74cc5d9fb57c8f473fed2f2f476563d4d287d5392bdada7fb99581484726b7f004d87b1dc8907fe0fd458137cacc7a3521a" },
                { "ta", "d12871b3911dc03c5b5d6a175e0b336e87cf7db2e4fafbbb42cc3dac98bcfa8a778d7f5d132d3cf50a1d1735e3d403cc0669976758f57e8bb1a8370012cf3ef3" },
                { "te", "cffb4aab6dcfb006e6e2e438bf44ee933a93edc28214d901b1836aa1b4ad363d4450a2d3876f0c826b9ce0a73d607ec27a9e4c4606764aa177c1add90f90da8d" },
                { "tg", "e16b37adb590193e33ba0e59e48ebe83c1c6efd519c3f22a0599d5371fa736a9dd57e796b1b399afc4132a845a676876776a82811d3890cbd2aadc7cfb74378d" },
                { "th", "650f1b0c20188c27f1590b63e33b7ef2cc69c5dee7992886cdcab1da97829a530878f021b271cbe287db73d46b7317f9376a3018a07ebd367ee05407968d9711" },
                { "tl", "659df1101dab1e9c5ed213174322d003e28baa46b2e0ab665d812f15bf5ebc87ea6383eac95699411675f8fe044bfc0210f5cb29e8a2dcd81c5372c83180e1ca" },
                { "tr", "f6bc1fd734a718793b1cdd7b51dac262b8a9793d54a6b97d3570df23c838f89d8ef824e943b49aade07f759ed77efac8a6e5e4b195899d5126ccb0b280d02333" },
                { "trs", "10ebdcceaefab5d9573e86a7bf0361ec20fa4653d7552b12f5c4197c6aa8ca17509a65294efa103024d35d61791ed8a7cf6a560bd51552cfc5c2e4e6f524d3af" },
                { "uk", "22e05f9aff1b5f2778fed9987f5c200ba82561b22f80005467c95e8cf8277de84e30db2caad3e8e1164193b5f1e3b411795ad20ffd0ea38ed9e6992a006fc45c" },
                { "ur", "260c122a37874323a48dbfcb6b228f5939758b3fd93f4d8217d4863b0d03275951a15181c8388ac3d66191df0aaebfdd610388aa4fd95aff6f4b0a0117613d81" },
                { "uz", "93ccab10339462b6fd8d15d8742491fc0b8e5b0868ca624a0a765af77f403c4ee61dbaadea242163f369936084a26abc19478c9137d1effaa4cfbee250ebadf2" },
                { "vi", "4a529783fcf37a8988992e759c6fb86a18059987cdc46cc8a2fe3f95ae351fbde70bac67195d8feb4f1d5f04c698b9bd985f366a508d724f9501d284ad2def5e" },
                { "xh", "b632774464f17b9884df476193c19d5393a7e104da4560399d1957608aa1cbf66870396aeb3d312dcdd8bae5894cfc0ce90ad2b178ef5a7a0f1793f6ce3d90dc" },
                { "zh-CN", "fd8476986c68d2f6f4f63530707c74ea72e77a2c23075753976aed902c6e9570152df48c883ebf7a19f53cc47e16105369a7efc5756a15e5b70942790314b3e0" },
                { "zh-TW", "b9afbbb0a2fe871194d41f0da616a0f05d0b076b4ec67c06567629827f99e0a9025e08a2ce4a52da9a651d4d1b3caf378723906c124c4f8fe5d7eaf6d0d951bc" }
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
