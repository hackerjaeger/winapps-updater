﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Text.RegularExpressions;
using updater.data;

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.8.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "b16309600d4c4a6f175e10e3451e49849ea1cb80afe330981c2288cc491812e16ea6335741e219b86ef8d5966b53b409e7f12ce9752be60a2ffc1360243bad64" },
                { "ar", "a7f183825eca99f5d3602915b81737a5f8a9b1e3f07503e81375530a380f8dbd18c2e9be27021d8e6f23d54f4bfe95f7df4318c83d31addae76290bb5335d625" },
                { "ast", "a10a658c9b775641e79a80be6a15b83da025189c67200a5b5df62f604218e5da5fc907fe294fb291cb505500cfc8fff535a856f13af416e4aa2f22f568dec871" },
                { "be", "77963225c67c85377569f1c5163fde3df9dc253ae7cc7f58c1ba5c2e61a0da49134eecfc0f2f6e668fc89cae634cafbed90936b3994f3f26fc6da42212f8ceb4" },
                { "bg", "e800ca829c81b7d826caad7cffb5e0990279e7a86664a22c8bcc82707b3506ef518e45f122571ed11e9db68b2fa3ced72cad1a73046877b6ba4918f3d7287a4a" },
                { "br", "ee42e721d07f4ba96a511064e46f196fe2dc12a63432f7426fa38baad5fc0034b6c9089b4bf90501fde7d8bc2f2e502f436b208f5ab14c4771b08f54c7d67778" },
                { "ca", "df88251f619a50d5e574497b5d74320424cfed331879e340ac72722c893853daa5c5c669a486362c56875009f8505b73be7d4e87a19c15e1dd3286174d1c423a" },
                { "cak", "95238690270133649ead4994a920d1b781809a3fb7006cd0d0996a34f739f8107aa605100d1eac8e707fbc74e4cfade2d0d8f99f32ecb51fc02a6731ac471188" },
                { "cs", "86783ecaa62d05cbcfd455d4795ace1e034ef91609a2c8461a8977b0b30bd8586c4489a117acb2195efffcdf6e0817d879c23d5b350e1ca1efb3b17db4363be7" },
                { "cy", "240e303d1d1147cc2242c491a0c7c771209f36abe32a827bef818c3e2883d23a55bfb2e1041688ce98ae20a6e34c5ea13d491352580f3f8368a58d7bfa0b9fdc" },
                { "da", "c9695434965f5bea555b587a51c3cc77e696482df611599306d50e40ed9e0876efa0abc7704282d6390633fb494eea8a2a6032809f30ec4ba0885f2f9006093b" },
                { "de", "bc04f556aa4c98c4c434219a5cca5d325fc23b213a7cf6ef554e82f3eb4cdfefe1de544689f072193d0b0f11917aa74e5f9678331140f66fe65458fa1a531815" },
                { "dsb", "b05654bd3338d5e7dda3a59ed92a6a515e8d251947f34fcb4eff002d66a95d772f4339c9c33c45e87b4d286a87e0735f774d2dfdedc1b597c713b9d307f01485" },
                { "el", "d009958300de23412b782a2b02147efde8f3e9f3e53cee2b732ab3833064bcd77dcc347753322197b695a1adc920fd5dc0a7b8c6be9937873349a8b50a0bc7db" },
                { "en-CA", "33dba02698e46aa6722d49990ceef74ce15c78893b42ae196a01dd911d61191b2bd9eb7c5c8a3065781f4095c8a85e4bf9d4ea8482a762f065cb0e1f8745de44" },
                { "en-GB", "ac2b6304ba3083739451f94c15f6ca1be75981cf120f1d591e853850472c2279069d3c53f9efa958824d1bf454f533e4ac5cc5f06d17f209a4152c9fb0ec2351" },
                { "en-US", "99033c3bae89affce7e6bc3d2ace863660c448f2e3c3b892ed61a88c71d8b99c591e8cd5291e0f0f5bbef8113de0c0d1d2e991fc54007be76e42ff21c4f19ee1" },
                { "es-AR", "86c07551b332a0001c38504e309ab0bebce9fbf5bc0508cf77af9e6e4307e30ae2f1b8eadec424e97777d7257d4f9a84bbbb7e8094074b94167c84a45cbf928f" },
                { "es-ES", "763d733745da9f7f7dccd6efcb720aaa5aeec065e0a1cd4860922c55071d0fbf05c86d8e91a2ae8243316ffc453f25cba9230d153f6b469598732f33af5ad8fc" },
                { "et", "510dd4b64a9d73ab96fa6839ffc1cc9b6a02304dfafa0913b7d6c3bd81073c533848b0964f550ed0744cdb7463e661dbd4a3c189d19ba5e5337b0e7cb7ddb320" },
                { "eu", "cf6c201c485dc7037be79142689e5eab8604e3f4f8c2583766378363c5e20554c0d65142c20325ad686d48c2e203fcc5e21ba5bb30a54255a398426fe7f061b3" },
                { "fi", "d58d5b2b795f1615d117afc9a725dde35b0740dc1b1a5935f92cffbebb6e0f304f241421263744387ba4a23f1cae723e3190f969aad0e37ffeeff28195fbdf8e" },
                { "fr", "78832b6be2df30b79572731d295b90ded062d455bceadd55e0377b626a32d342661c9293c285295440db6e7bcd05e5267fb945af1fbbafbedec73f396127d20e" },
                { "fy-NL", "378b1642c0d4b83171a8d681667c33501c38430ae1d3a36fdb951cd35fb90e4f4f967ec23661c233dbc32ae7d86a0fbb646dbdb77f5bc2a9e18173d02c3b410b" },
                { "ga-IE", "bdf3b34d25efe2ef7855c3a6d285c8a8ce515f56accde4605f2175e5784756410822f6533990a414db07988ba5323dd4fefc7f83954053d77869ab35ecfa25f3" },
                { "gd", "8fe5c59591e6d048c5199b9f6aa167c47cd6726b04cc127d5fddbe2c27ea3897440671ed5a513703858f7d8d65e4adc17a969b72c3d22c8b20be6f02a1ed10d4" },
                { "gl", "68e63e3267f7800e0e869c1acc06c7d5b12ebaa708781efbc29d8c84d1e7b0ffa1c658ff7f6c9a7f958ea1a9731071234459f4c09a605b57a880846ab41c8629" },
                { "he", "65e8603bd14c1702c5dcade3c757611d1f93bead8e40c902783336a319339860d0ad961a4d9f37b2fa06accd218ae915e2c2787604c6fb16c641f3e142708c67" },
                { "hr", "1e7213a3671f6f4fb74a7ee4421b4cb2a04a3b44b6eb6cc2297f7f36d735e633cf7c4841fb5ffabc8c794817dbd1c5341b22a4c34e21f4f9aa871199673de822" },
                { "hsb", "8bafd06f1a8c6c2731ee4f2c653a9071b94132cd9e149de3dba4e7b3a8d71e492b78aa80e865c6fc254d2afb2cde43926cb90ef6ef8bf83a556a5d5f089d58f9" },
                { "hu", "4d54aae479bbad73da40bdf6dd9bed729290b15d6dd9d110ee5ae27fe5ed54c910c27636f8dd8368314b01672cfc7ee189a56db2aaf4a332293ceee78746dfba" },
                { "hy-AM", "dc11ca8ac11785ba23f586d3880fce6cd5a4131b34cfee72b71823a4e05840ed220699cfdd8f08508a2ad1884c327e1f1d4e73fdaeec5eca805e1891ca6c3b69" },
                { "id", "66196bd59ba7a53d47a036f3e7e0ff32fb6c1d48fd7a7e2c93707daee7f442cfaba759115631006ecd3b3f471b659ec1231dd6808079f295f747a77a4b1c6278" },
                { "is", "e567d9f5680ff6e5500d2f77bef735bdda9ada974940a8888c964719a76d0ebeacb15d435d221d88693e25a58e9c8ec89acbc5369bcb0180f5726f4a0f6f2cdd" },
                { "it", "c9d0f66ea5a1e54c15b905e5adfe7f14ce1062dda510163a67c84add8afdd6bc05efe89e3cc31d71573b0b416ca03ce7c0e7a79a85eb873c210f6b219e23bf95" },
                { "ja", "17a1b7a7bf3030f50ccff3b5f10ae5f415835ca6016a74e976f7b254ab398a64d3c8bc0f74d1b379ca38ab36ec1545f65ac6e237502ed20921678c378a936210" },
                { "ka", "16993a44c493df961647ff0fbb959ee33f657851741b67c21dce498e05d761ad032b178b224ab41bff1d98bc532843ea5c284f639d3afe32ad68a728956d5764" },
                { "kab", "15aa84b6c0099f34878f0c00d99445f63fd3235c235fb60120fceafe6fb737d30bd8198d5978f414cec91e1cc22062225c383735d4c286d6260864225a984abf" },
                { "kk", "e2ea99dfc32ec60b469cd5d6dba65b280799d2fdcbc7a8659254aad0a87d0a54c54d033a64fbe1ca20b3e4d8ee2683b30688236a20d452aff75b2411c1b138f6" },
                { "ko", "12c793dcf5f7ebf95c7afcd373d217de6829ffac58708a1edb2648b48ea16327f3f6282e91b868d6755e71f26662b436343b4441eb2df0b689cdd8a1be708353" },
                { "lt", "fe4d779794d355b028e7129121585982872649fb8e637f48041ee8bd78ed26cd8c9881b3311066248fdd971feb1c4c7b708af5a705d3aad89cbd91d2f435a0c1" },
                { "lv", "e3f4a5048ffd67cf5a28074a48949bff134f25c25a0587a727c5718cf758dc57d4f8cd69085057f1e00400371b8062dfe992034d16540db1cbffcf87a33daddc" },
                { "ms", "f8f82810248b3074669b0c664d31366e6d462d3792c663a773975fc6b13117f2f68dd0b5c83c985b7f6c90e3980aa4be7fe7114c3e0a7363409052f4aae3369c" },
                { "nb-NO", "bdedd0a1c4efe6283f1d6082049bdae67ed67e550bb2247b6ad11981e63eb7d781ac490cefaa83bd940a62a611faed6e8e781afbc8839e691d57e8ffdd8c49ef" },
                { "nl", "18181710b3f314d831150b8ee355830d2f78d495bb60e109c8e116b8492f43808a38582fe6211ed514b3b669aa1b316fae6178b440e421b524b6f920cee047c7" },
                { "nn-NO", "e30710087b90596d1d36af8ac392cd3d5d1d04c4587d11db6288eb7707c441dee701d71f555be0c2993df6d9cc9249d1b343bf9c76929d29e089835d45b8d1de" },
                { "pa-IN", "bb20721a97fb38017e15634699d5ae626426a093a1831cf144748add9dc7dcb877f9e41222217c2eb907a529e80a8e7c10ec97169d15367dd75945e23c79d78b" },
                { "pl", "64b8befbe829a1be2779e0e4e724784c66a0009dfd232b3ff5c149d3e5be393cb7efdbdc52244e89c0b7d5c6f1d6974b07ac299fb5f33ae0be038838210c300e" },
                { "pt-BR", "5111d3212a9855ae56f7e105aedc58f83d0a48fb33dda03836780088d215c30a3fc00582bd8461d384180109f1b166858cf253760bf4be1a68d01115455584f0" },
                { "pt-PT", "158f061e8ae74b92410f18238857f193097cdad8e0b2a6593d0d156ddaccf44ca783ca2c8d9c46c791397b9414675db5386fea12fda70798561b579a3236e885" },
                { "rm", "f01d4a7190611bd9f89c6afdff052154239ee7cd04b0807f493eb34c93d1644e2a7ee7a190988453ed9aa9ec3ec5fd16401d257c407df86f34017dba98981af6" },
                { "ro", "2150870c4da7d551e916c8fa9840f5f9207b0b7405eca77fb10e872f0c598f895ab47b527370bb876ea33f54318de13c763d903fd596f0e682f7ce3ac892a44c" },
                { "ru", "52ba45fffd195ccb7b9bd646105f0c78aa7fb504e0aa1f238c8db09501d63861fc1bd6866d02df3962ec5c5cd386fa799f32a24083310a7b022ffc45a22d63ee" },
                { "sk", "9fc7dfc910b94df424c9fb844073efe3e3d76aaa997d4add12ea6e7f9f66076a2fde9d53396ca10e1b5eb871bcec841f8eb5ea97f2e9c69c65671a5894e40d9f" },
                { "sl", "71742d161fb4a4c19d55a23eb7da2d410e3cb06a1ff8a46cc2bccd2474703613c50f7a521ad65c594e3bdfc5685ec608905f3e8e2a4a8dc63ec8cda5640d9f25" },
                { "sq", "57afa6a00d60018662d16c6d63c304ccc2d146627f98000e44b37d94f881c05f2986bc4d6fb77795305bc0540c89271831e8ea4df305ee53dda7511b6d047b17" },
                { "sr", "4bcd63b02f62819f8d61684f97f4a2c05d69cce4c7cc08e3dbb23a212fc281684799f11b2656b29babd72a248d008de2a021cc9fe30607ec6cc65a2e8679bcdf" },
                { "sv-SE", "af7f4887e69fb063d04db573d3b9b597ce928ca45ebd1caa3d04644e8b5e2b962d1a986c342cd5f64fe8a69df3785df9ebf4eaf3b6513c1d972fea36bd92389a" },
                { "th", "7e74f4aef40a01d635035c7e334c989ad95c94d31b2eb2f66a4801f44758ba10d39d7d04a624fc79c8a31621ea7b8c62ca670090012461cefebc1ff6b4629fea" },
                { "tr", "627e4323db24ef02609c8fc278796a95b0923091f182a0b7c6e6e8d794b0b63f3e3a38d9fd40f94e50dd81752025297e27fee99669966fc288d0e13a63c90a58" },
                { "uk", "d1646f6d6fad98d889d2a5ed07dc0d40097526f2c3bcc749b7bcbaf8becb3801fbd745ecfd920b9ef55f46c8541efa51aa5ddfd51c373756335f744c9e31c797" },
                { "uz", "615f4df67a8279ea84bfaea8609c223aee3fd5edb96ee43f2db8e53e412830734b915d6b6db1df67ce86502c73abe0ddd87901ec35ac5b8f03cd5dc82c0dbfd1" },
                { "vi", "bc6a66659b97ab8cf4c8d2f64b7ba9ea03659ec07839b2ab661edfe070ce9611a90d11112ca44bb87bf96e595771f1e0cf6fb4abc550d9db3a81bf25a9316759" },
                { "zh-CN", "f4bd99ba439ced33f9ba38c564f9c319103e81dd207672a66d60f4f5f7b51cf3224a8aa2a3aec29ea1d5707a7f8906eacccc733da6a8828053cac4cdf424ff7a" },
                { "zh-TW", "65d2dc39aa3f715e579f98c281831aea7efd6b8ad1997a7ac981762c98e8b5bb2d3849edc3e1aabcf3befd03c53c7febfb249cc1da74f244528246fe8c4de043" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.8.0/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "34b95a00128bc9d83751794ac28c0ea61d60d0cec3abc8cf4b7ee75763c8e8d673384e8ebc397970f3d5fdbdac108a82b83dc9356341a8b9efa7a64f5fba699a" },
                { "ar", "6588f15679ed7e495b3fd6cb843911ab80abba4adedb5c2d317f2840260bb863dc2c9c7cb4b846c604f0484a12d1b228790a1c9f0426f9ff361e76fcd31e586c" },
                { "ast", "99e31452b85600820f0a15facd2ecd7b91c1b4db19a5fad0c9f08eafe2846185660c391e2318f1135b31b1f9268bbcf83c3a5c7c10b232d01af563548b5f03d5" },
                { "be", "48fcef1f1c721d3e808ad92ed7ec27f1699d76ff6e55f05c2c112cb40e7c32172115413d158754ab8cf9c523671f905f3f98f2235e2c5c0225e84f326e1d93e1" },
                { "bg", "a8350fb5db873caab935c1e3d39ae91a4b442258db55d30a49db4ad1c8c0e2da4eeb4ff53023f98776975638b32175af4569e35aab8b4bf067823bf471b30077" },
                { "br", "1072adc8fa8c320f8d56c9f365dd06abddea6e9dd3b2e1a5d139acf82510d1a41a1c6573915835d1ab8570d9f565499d9187f0118e60103018de5082dc9bd616" },
                { "ca", "ef7e389ea5ceeb25ef74428a837f1af9e59e5a2e5de53ff35a35c36a165b59cb6d163951e6eedc13c1fb5b179e0f5ba50dc63e06596744bd0635f61b774ae4a1" },
                { "cak", "2b3373bac6126bec7570cff4af082344e2e62c4d9f87d2eff3ae861e380411f2d640bb4cb7b6845e4e7df918816c97f532bfed19aab90d63080c07205be9a2b7" },
                { "cs", "406c1b440ad87e4214bdbdd0a7bf76273fb60a6a4bdf0839b3aeff8f0ae40d9aeb70b9e42ed628809d1595674f3156ab92d499590cc65c80e1ff5a410d92161f" },
                { "cy", "03c803b0efb827e43a8cdf3dd90cdd389535bdaccede0b85ae41084db6857e747f3f19df13c517471e3966d9f674591fc1f39313d4160a083711c36ab8c5ea5a" },
                { "da", "88c26cffa6e4ce220fa05a171868bb00ab73c4734a1af9f9a12564ee5522404a8a18d831217ddb9ddbc9365dbbe2e1b9093365d82cae440a0a752fd7c7fc3440" },
                { "de", "1827042d168f8d5cfcf16fc5489441ad0489ddfd38254eeb25d818e468d77fbb50b37bdef8e8c79d9b2bcdc74978e51c7d6a372d66636a50c4c4f7ef5f875dfe" },
                { "dsb", "223bac1ee3ad2bc342a327d2658a776ecf72ce15be30c1391614e8c44dcf5cd413341d63b6589c2eab448d0df8961b73b9e59c8458293b78bf79a667d2dcbfec" },
                { "el", "5bcd9592545a8235dc00f58a09b3732174a6591abea7a02a1c0ade20403b0d2f268047170185182314612981ff44a77d1c104650512baa2852cb6ff6b6039cc4" },
                { "en-CA", "ed4b5e6fc290b2641882fa91c611c40678680c2867921e8a5db8c0a0d283825e1dbc36525700381c3ffc883b88c1f65016d1ec0a4c861390b447c3573d8a475a" },
                { "en-GB", "ed062d44eacf49085bac81e6e30da4a58fc188115acd74140521289d257bc3f68630b1b9671196ebbcd27dcde782ae47ca3b3f9110379c19c44d9b0063b6186e" },
                { "en-US", "07a185c21a94b05f68758d1a7cb8ef82c816216f1cbc5efaf3047fafc733fa0f220cc36e36e6d03cbc1d5da773be3a1061b124881bf51cd8c2a69610fef25985" },
                { "es-AR", "8d42e9be52462ee30f3c3b4d6575b408b8963f0a6cc56a3e94716c3e5dcef62389bfb56eec3a4f2b6e1485921210fbb23dda2e8c917517a15b11be2a88e8e7a1" },
                { "es-ES", "4a7554a14caad939f59bff8630514f227ad2c9ec95511ace870a0a4b1f377fbc28ba7315cc284d7dbd21f1486f993f8ff66aff5e64dd72c6da78adb7425ce804" },
                { "et", "bb327f8ca52ab71d500915c1efb80732b9b9e8e0b3809fd8dcfbfbf34eaf45c0ada626dbfa44cef75bba6b78e0bbe2032238d8c4fcab0fc27c6f7db6e755acaf" },
                { "eu", "b25c6d38dacc541918c67bfe8361e68c1ecfd88bf1123139447fa04f7d440079f9d999eea18857eaa089054b229d44806e6296b43413c72351117972eff22f73" },
                { "fi", "4356ecdb059f80f092723ee2219ba3d0826eaf7862c1092db0d505c4225d0be44694b9cf7fba7a85db336af9d55d4276489823e14cf9d861b468d865d5821b97" },
                { "fr", "cba0370f4bde5218697f795b58118e40ae65fbfd39809355b72e8bce7f3cc5b2da17b0d6f9bb542d4b029e7bd0d6186fb180ad6e22c750cc4b942bc1e748cbe0" },
                { "fy-NL", "41ca47b3067d969cc7032c341f5041cf591e4328a4f9f3567430ace90a6eef932fc51137afe37c4a7f01dfd546f8d5f28d02bf04cec3a973f592384e1e5406e7" },
                { "ga-IE", "7e5e0105af9c4571ad776d74ae6c47ea0ae8277debf7e77e89c996d08a46e178f283e8f3e7815731654cbc49bd1762822b677ce60c2d1be0556a7d8e33530110" },
                { "gd", "3801884c1bd74068387961dbd492a5c2b9a4f49778b518fd598ae314ac1571d103ae09bb498c0c2fc25d178804820ede0e4b1c8a0003fc2d825fff27c232d249" },
                { "gl", "25b3a86300292018e1a8011de59d34b440f7310afcd3dec8c856ffe111a8f1543414fd81a8e70a8c5bf5bbed86bb6773d0dfd062c4df39f99bb882c084024bd2" },
                { "he", "e1350f1707bc343a2bf7aa49be67c0bb2e3d342cb35643d2b83ca635e276187eccaf56e23fd1697af29065a938bcc961d2b06d65cd47536dda16c413e9fa56c6" },
                { "hr", "2218ed0597755d57d1ab80d325a0523dc29feaa8de415dbea63cc8f5d327f168d9d9de09a3be38e80d55177b77448c79e57f238b148fc45dc671389de223c1eb" },
                { "hsb", "34b63ed995351ae3331484fc33d9c92facb96a6337bda8511c6d13ef0ce57c187f878b831d8edcd483f247a7902ac45c793de0d5c0b3f732f99caba56cbcf8f6" },
                { "hu", "5e1435912934c605f84e38e3ce123e30d747ec9a51d50fc4a006b646ae47cfadb1133ddd148f61e7f45e0043672f88ba95d4b9e128106ae6f4660f1fdc20db2d" },
                { "hy-AM", "0ecb47aec19a5b1d1fe6aa9ca6a53d36c1017a2ac5e82e2428c5162c71a5b29a9519ccec99cbb318366f111e4be78b9c3b0402006c809da4ad72cea83e286018" },
                { "id", "d22ea04d94c9b342ec2b107165680e3eb9131383d8a6cf3eed4e87c97c08a1de10136339cecbe8f755231e28f2630edcd949d669289b7a08046ba5072fbdb01b" },
                { "is", "786f2164636c142e257a361c26b74f5c8ba89c0f101915ecec8953b91c33f9849155edb006b58464645c8b20bea52636a2438ddb1ec1f94ee8628490c22b0efc" },
                { "it", "e5d9570f4dd5fb22cc9b04d329fd43ac9bc0811ace2e14164bcd72edab8fadc92a266f82638c20fc6ffb0fb0d52b598a367c28813919a3a2b651aa716b0028ac" },
                { "ja", "868b140d0fc1643e44b8c2e763088e4629e41322308c1fdc18e2fc59de4f8f95fe1fa6b2d3bbd8b8c9d441f3ff69e633cc1cc30432b83d3e8cc3057d20321eac" },
                { "ka", "078e567929d9897c9abd3d5be19700d748b544773f17f1a0020049c3438fb7fc6dd22ecc4c1de8d792f35b584f2c57c1044f03375a08ce8a4c0467ad2bb8ade6" },
                { "kab", "97e2b61cbc06967547717484d71e9a24dec3cdb5eb6ab4d1d48a4b21068919fddf74bfb278b92b3e331528df6f3adb58238f088ee3d5e7afe1934352a3478009" },
                { "kk", "899232863b686487556a7cd956ed75b2925d3dae5e1b24c0fc13b49b4d8e57921c18ceeaff7350381806ffcee12724e445ba14a8e86c1d2f2894d71594793d27" },
                { "ko", "feb8497fee119e9e87f1923238bd9799dc43dcbc4f8a6a9b966229683b8fb32b4dd18c54fa53205305165f9979291413204e59388e3abe96ac4b65a86eda2c1b" },
                { "lt", "5b751e430189343b9939a0319c183964f47f23d4e82d07ffe8046f07971adea8c82f010b892b019bcb10c76ec3ecca67af1e2c236776e7b4fc6e24bafad23aa7" },
                { "lv", "24091c7cce80783cedcb0323f49fa1ef22264aa85a3a94c8eb212caef801d765745e1c6eb307bd03d0c77c0d14eeda19472d5f149f4306c4a114066407d43c57" },
                { "ms", "4cc2d57faacede446a456c2eb47bcf0a2929adb30228da6b45ca77eb4db4fafd982419c18bd8f3d63c62bda2866b74efcef62a82687beaaec602630e82ee0e41" },
                { "nb-NO", "c05daf219564d35279a6b3c320c9e9247c7543fab81a3a9e3e8678dd273e184f0603afa66df75f59d51e7bb3aa435771f735821ef4e575bac4750d845bd098c4" },
                { "nl", "e8388377550100325ae0e6cf2b3bb2a6e9819e2ae33536d2e68dd1360b8e124e6c9b261c7ef8786dd2fd9699a734030331e2c1d663067ea6bbac5b30f757e19b" },
                { "nn-NO", "f655777b287c8598bce36db1febba99ab8fc4b3274f60ecac2ad99f45395ad6920196a04862ae4421cb0b8ab0e0c71d00820522512f1c9d62a8dcc2b6d9dd29c" },
                { "pa-IN", "28d920510707a8607a605721e1a64c84fd4e112d91c5ea315aa6724598eea85fc600ed51f8b928c71c30757004a026858fdfb0abd007945e9395f4cece1f0212" },
                { "pl", "c20b2948dbbaad21f44541ac7969c489003f915af9e4bea8ff179eb8553e5cee85b4ad36748c52e6206bf698041033f81c16135727bac61e3b1cb1d02aa559b7" },
                { "pt-BR", "210a5c6bf686fa2fba64c41c33a1060408fd74ddad39d34b35d2c747cb1a216441e6b7052c8cac29ebbacfff198a3c56d4bcd71d38ff9fc0497fccd8bab85cf8" },
                { "pt-PT", "e5d2e8a4f878787604100af297f5c8f144186d0866283469f7bd8bfffbbc28126b8a23b39a89c744b469e7846d92ed966b6f41b62357fe6d1a8e0c6c30e75c7d" },
                { "rm", "a6307ecb663675f10934f838d7c8ef2bf4051fd605b194baf897a59bcf5df75400b5610ba9aaaefc9c21affeb59f59f6df7cf57c69d83d8c340f43bcd738d71b" },
                { "ro", "42ff11c417cf767a380dbe6e65b12e84a7a4cc4053a3526f9d9f43413d548bb4c45f3d14216ad92cfc67e67ef2d2c6b9544f1b476bc923aa290faa3cf9038b81" },
                { "ru", "b3af033a90182fa7fabffe8ddc515c6a2523142acc92f447d7ff80bfc94ab8d92bd6ae5902a13b2a875f73d95274af86ab02ce8d8d4173d4d83b0f36f64bd8ab" },
                { "sk", "88306c5b967e706525818ad6ee107031bfcdd746b2b047c3928e66f8d401054d5490c9d971c0fcda9f3b5451f5a6d501955721a81da3e73ae5de889ecee2e762" },
                { "sl", "942d44dc09713064d8aa7bf63805d18f21ffaacc105b552a2f127af5b9a21e4a23d73ca04c3236a73765f3dc88b763eba103cf90e40aa1054ddf0aa06a92fa8c" },
                { "sq", "5d52901ebbe84609f742e4b47b284ad2d207393e20b416cfd5d2e589522d6ab05339d8ecfb4244552ef2363c9959caa46466b374486b59eedf1d6f9c340049d2" },
                { "sr", "49ac7bfe850cc6f2e7faec5f6690f0583a9c96df383617edb5cfae9ea15bcd2f29a17176110d0419f2e2b601e8a54faa731ef2e16814fedd1a302b6800d75fb3" },
                { "sv-SE", "983e43f437584f334d0f523283af494fe562ec8ea53cc0a4da696a418d00da7002943f604aa876cba678a7d5818de6ce2a34b6dd38779ec825d36dba63de698d" },
                { "th", "dae19e580e932f682ca427b907b97b695833f47a2358eab4faf471aaf6fe219c0fed49c9cd4d429a6e9098320b0bc8f3c84c6d3afef338216f3f376881836e95" },
                { "tr", "46310d87d28a04dc987cc4ee61e70d93734470ba32daa753428b875690fa3ba550ae82504a1e5fc3defc39da177f757487bc07f8fc5d94a1641a05760ab4bdfc" },
                { "uk", "abdd8b366fde4eb3734cc3b9d3f98aa5aaeca398afdd49e11d83d2cf2284774a998beac09e229357b724a4d993c4953c60134d540043ff1badda09fdefa4f09e" },
                { "uz", "985a59bad8b49394e277d54ef63732c8bbf4601fcba6ccc129b6d5665e68543192ea8d48f8743f6d15ebdece66c87908a77ca86107d7e0ae35c983c991f87163" },
                { "vi", "355756e83819a47883cfa0f7ea508a5378ae65275512d87b834c6aecd782062b61e1742ffa7f382816a8217addd455dc68956c35df7f53c6d09dfeb202f6802d" },
                { "zh-CN", "1bf107cb7e7bf69f86e18a27510bad8648b9770c32bc75ccc69da00467fe8ee4141f1e2d3f5801655a2264af840eb37763fa52a018d1f747f244a27890a56d39" },
                { "zh-TW", "830b70e758bf6a5ca29be2dc1c1a93303eabf4311cd40387a1a0d19756c2c1e4f79e895b55438b051bb4ab26b572e9f77750dcab8c363ae5cbeec54eb8e867dd" }
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
            const string version = "91.8.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
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
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
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
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
        /// Determines whether or not a separate process must be run before the update.
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
